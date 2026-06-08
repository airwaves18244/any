# Roadmap разработки плагина НВФ

Каждый шаг — отдельная реализуемая задача. **MVP = шаги 1–4.** Шаги 5+ —
расширения. Доменную логику (Core) ведём через TDD: тесты в
`RevitNvf.Core.Tests` запускаются кросс-платформенно (в т.ч. в облаке).

## Шаг 1 — Скаффолдинг решения ✅ (выполнено)
- Создать `RevitNvf.sln` и проекты с TFM из спецификации:
  `RevitNvf.Core` (netstandard2.0), `RevitNvf.Revit` (net48),
  `RevitNvf.UI` (net48/WPF), `RevitNvf.Core.Tests` (net8.0, xUnit).
- Ссылки: `Revit` и `Tests` → `Core`; `UI` → `Core`/`Revit`.
- Ссылки на локальные `RevitAPI.dll`/`RevitAPIUI.dll` (Copy Local = False),
  путь через MSBuild-свойство.
- `build/RevitNvf.addin`; post-build `RevitNvf.Revit` — копирование сборки и
  манифеста в `%AppData%\Autodesk\Revit\Addins\2024`.
- Проверка: `dotnet test tests/RevitNvf.Core.Tests` зелёный (smoke-тесты
  `Point2dTests`). **Запускать локально / в CI** — в облачном контейнере нет
  .NET SDK; SessionStart-хук это определяет и пропускает прогон.
- Реализовано: `Directory.Build.props` (RevitVersion/RevitApiDir), 4 проекта,
  `Point2d` + smoke-тесты, `build/RevitNvf.addin`, post-build деплой (Windows).

## Шаг 2 — Hello-World в Revit ✅ (выполнено)
- `IExternalApplication` (`RibbonApp`): вкладка «НВФ» → панель «Фасад» → кнопка «О плагине».
- `IExternalCommand` (`ShowAboutCommand`, `[Transaction(Manual)]`) с `TaskDialog`.
- Реализовано: `src/RevitNvf.Revit/Application/RibbonApp.cs`,
  `src/RevitNvf.Revit/Commands/ShowAboutCommand.cs`; удалён placeholder `AssemblyMarker`.
- Проверка: **локально** в Revit 2024 — собрать `RevitNvf.Revit` (Windows + Revit),
  запустить Revit, на ленте появляется вкладка «НВФ», кнопка показывает диалог.
  В облаке не собирается (нет Revit API / SDK).

## Шаг 3 — Доменная модель и раскладка кронштейнов (TDD) ✅ (выполнено)
- В `Core`: `Model/Substrate`, `Model/FacadeSystem`, `Model/Bracket` (+ `Geometry/Point2d`).
- Алгоритм `Layout/BracketLayout.Generate(substrate, system)` — сетка кронштейнов
  по шагу X×Y от отступа края; детерминирован и идемпотентен; без Revit.
- Юнит-тесты `Layout/BracketLayoutTests` — количество/углы/порядок, идемпотентность,
  границы, краевые случаи (отступ > грани, пролёт < шага), валидация.
- Проверка: `dotnet test tests/RevitNvf.Core.Tests` **локально / в CI** (в облаке нет SDK).

## Шаг 4 — Revit-адаптер: материализация кронштейнов (завершает MVP) ✅ (выполнено)
- `Commands/PlaceBracketsCommand` (`[Transaction(Manual)]`): выбор плоской грани
  (`PickObject(Face)`) → `Geometry/PlanarFaceSubstrate` переводит грань в `Substrate`
  (мм) → `BracketLayout.Generate` → перевод точек в футы → размещение face-based
  семейства в одной транзакции (активация `FamilySymbol` + `Regenerate`).
- Вспомогательное: `Geometry/UnitsConvert` (мм↔футы), `Families/FamilyResolver`,
  `FacadeSystemDefaults` (временные параметры до шага 8).
- Требуется загруженное face-based семейство кронштейна `NVF_Bracket_Load`.
- Проверка: **локально** в Revit 2024 — кнопка «Кронштейны», выбрать грань,
  кронштейны расставляются по шагу.

## Шаг 5 — Направляющие ✅ (выполнено)
- Core: `Model/Guide` (+ `GuideOrientation`), `Layout/GuideLayout.Generate` —
  вертикальная направляющая на каждую колонку кронштейнов, от нижнего ряда к верхнему;
  общий генератор осей вынесен в `Layout/GridAxis` (переиспользуется раскладкой
  кронштейнов и направляющих).
- Тесты `GuideLayoutTests`, `GridAxisTests`.
- Адаптер `Commands/PlaceGuidesCommand`: линии модели на плоскости грани (временная
  визуализация; реальные профили — далее). Кнопка «Направляющие».
- Проверка: `dotnet test` (Core) локально/в CI; команда — локально в Revit.

## Шаг 6 — Слои пирога и облицовка ✅ (выполнено)
- Core: `Model/Cladding` (+ `PanelStartMode`), `Model/Panel`,
  `Layout/PanelLayout` — сетка панелей с учётом шва, привязки от угла/центрирования.
- Core: `Model/PieLayers` (+ `Layer`, `LayerType`), `Layout/LayerStackup` — состав
  пирога от стены наружу с накопленным смещением.
- Тесты `PanelLayoutTests`, `LayerStackupTests`.
- Адаптер: `Commands/PlaceCladdingCommand` (контуры панелей линиями модели),
  `Commands/ShowLayerStackupCommand` (состав пирога диалогом),
  `Geometry/ModelCurveFactory`.

## Шаг 7 — Спецификации и доборные элементы ✅ (выполнено)
- Core: доборные (краевые) панели в `PanelLayout` через `Cladding.IncludeEdgePanels`
  (`Panel.IsEdge`); `Reporting/TakeoffReport` + `Reporting/FacadeTakeoff` —
  ведомость (кронштейны, направляющие, облицовка полные/доборные + площадь, пирог).
- Тесты `FacadeTakeoffTests` + краевые сценарии в `PanelLayoutTests`.
- Адаптер: `Commands/ShowTakeoffCommand` (ведомость диалогом).
- Полноценные Revit-расписания/угловые элементы у проёмов — задел на будущее.

## Шаг 8 — UI параметров системы ✅ (выполнено)
- `RevitNvf.UI` стал чистым WPF-слоем (зависит только от Core); `RevitNvf.Revit`
  ссылается на UI и хостит панель (цикла нет).
- UI: `ViewModels/FacadeParametersViewModel` (+ `ViewModelBase`),
  `Views/FacadeParametersView` (WPF), `FacadeParametersStore` (общая модель).
- Core: `Model/FacadePreset` — пресеты (керамогранит/кассеты/композит).
- Адаптер: `Panels/FacadeParametersPaneProvider` + `PaneIds`,
  `Commands/ShowParametersPaneCommand`; регистрация панели и кнопки в `RibbonApp`.
- Команды раскладки читают параметры из `FacadeParametersStore.Current`.

## Готово
Шаги 1–8 выполнены. Дальнейшее развитие — реальные семейства профилей/панелей,
Revit-расписания, угловые элементы у проёмов/парапета, теплотехнические проверки.

## Сквозные практики
- Каждый шаг: код + тесты (для Core) + обновление docs при изменении модели.
- Коммиты по шагам; ветка `claude/busy-gauss-PdcDZ` (или по договорённости).
- Перед правкой Revit-части — перечитать навык `revit-api-2024` (транзакции,
  единицы, потоки).
