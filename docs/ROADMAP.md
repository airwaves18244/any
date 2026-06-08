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

## Шаг 4 — Revit-адаптер: материализация кронштейнов (завершает MVP)
- Чтение выбранной грани стены пользователем; перевод в `Substrate` (мм).
- Вызов алгоритма Core; перевод точек в футы; размещение экземпляров семейства
  кронштейна в одной транзакции.
- Активация `FamilySymbol`, `doc.Regenerate()` при необходимости.
- Проверка: на плоской стене в Revit кронштейны расставляются по шагу.

## Шаг 5 — Направляющие
- Алгоритм направляющих между кронштейнами; параметризация шага.
- Тесты в Core; размещение/построение в адаптере.

## Шаг 6 — Слои пирога и облицовка
- Слои (утеплитель/ветрозащита/зазор) по толщинам `FacadeSystem`.
- Раскладка облицовочных панелей от базовой точки с учётом шва и режима старта.
- Тесты раскладки (швы, центрирование, краевые панели).

## Шаг 7 — Спецификации, угловые и доборные элементы
- Угловые/доборные элементы у углов, проёмов, парапета, цоколя.
- Shared parameters и категории семейств; ведомости материалов.

## Шаг 8 — UI параметров системы
- WPF dockable-панель ввода `FacadeSystem`; пресеты систем (керамогранит/
  кассеты/композит); связывание с командами.

## Сквозные практики
- Каждый шаг: код + тесты (для Core) + обновление docs при изменении модели.
- Коммиты по шагам; ветка `claude/busy-gauss-PdcDZ` (или по договорённости).
- Перед правкой Revit-части — перечитать навык `revit-api-2024` (транзакции,
  единицы, потоки).
