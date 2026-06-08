# CLAUDE.md — RevitNvf (плагин НВФ для Revit 2024)

Плагин на C# для Autodesk Revit 2024, автоматизирующий моделирование
**навесных вентилируемых фасадов (НВФ)**: раскладка подоблицовочной
конструкции (ПОК), облицовки, слоёв пирога и выпуск спецификаций.

## Стек и целевые платформы

| Проект                | TFM            | Назначение |
|-----------------------|----------------|------------|
| `RevitNvf.Core`       | `netstandard2.0` | Доменная модель и алгоритмы раскладки. **Без ссылок на Revit.** |
| `RevitNvf.Revit`      | `net48`        | Адаптер: `IExternalCommand`, транзакции, размещение семейств. Ссылки на локальные DLL Revit. |
| `RevitNvf.UI`         | `net48` (WPF)  | Лента (Ribbon), dockable-панели, viewmodels. |
| `RevitNvf.Core.Tests` | `net8.0` (xUnit) | Юнит-тесты Core. Запускаются кросс-платформенно (в т.ч. в облаке на Linux). |

- IDE: Visual Studio 2022. Язык: C# (latest для каждого TFM).
- Revit 2024 работает на **.NET Framework 4.8** — Revit-часть только `net48`.

## Архитектурное правило (соблюдать строго)

**Вся доменная/геометрическая логика раскладки живёт в `RevitNvf.Core` и не
содержит `using Autodesk.Revit.*`.** Core оперирует собственными типами
(точки, векторы, параметры систем). `RevitNvf.Revit` — тонкий адаптер:
переводит выбор пользователя в доменные входные данные, вызывает Core,
а результат материализует в модели Revit внутри транзакции.

Зачем: Core собирается и тестируется где угодно (включая облачные сессии),
Revit-адаптер собирается только на машине с установленным Revit.

## Подключение Revit API

Ссылки на `RevitAPI.dll` и `RevitAPIUI.dll` — **локальные**, из установки
Revit 2024 (по умолчанию `C:\Program Files\Autodesk\Revit 2024\`).
В `.csproj` ставить `<Private>false</Private>` (Copy Local = False), чтобы
не копировать API в выходную папку. Путь желательно задавать через
MSBuild-свойство/переменную, а не хардкодить у каждого разработчика.

> Из-за локальных DLL `RevitNvf.Revit` и `RevitNvf.UI` **не собираются в
> облачном контейнере** (нет установленного Revit). В облаке работаем с
> `RevitNvf.Core` и `RevitNvf.Core.Tests`.

## Грабли Revit API (читать перед правкой Revit-части)

- **Транзакции.** Любое изменение модели — только внутри `Transaction`
  (`using var t = new Transaction(doc, "..."); t.Start(); ...; t.Commit();`).
  Вложенные изменения — `SubTransaction`. Группировка — `TransactionGroup`.
- **Единицы — футы.** Внутренние единицы Revit для длины — футы. Конвертация
  через `UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters)` и
  обратно. Core работает в мм/СИ; перевод в футы — на границе адаптера.
- **ElementId недолговечен.** Не хранить `ElementId` между сессиями —
  использовать `Element.UniqueId`.
- **Выборки** — через `FilteredElementCollector` с фильтрами классов/категорий.
- **Однопоточность.** Revit API вызывается только из API-контекста. Из
  фоновых потоков/модальных окон — через `ExternalEvent` + `IExternalEventHandler`.
- После массовых изменений геометрии может потребоваться `doc.Regenerate()`.
- Команды реализуют `IExternalCommand`; точка входа приложения — `IExternalApplication`.

## Структура решения

```
RevitNvf.sln
 src/
   RevitNvf.Core/        netstandard2.0  — домен, алгоритмы (без Revit)
   RevitNvf.Revit/       net48           — адаптер, IExternalCommand, транзакции
   RevitNvf.UI/          net48 (WPF)     — Ribbon, dockable-панели, viewmodels
 tests/
   RevitNvf.Core.Tests/  net8.0 (xUnit)  — юнит-тесты Core
 build/RevitNvf.addin                     — манифест add-in
```

## Команды

```bash
# Кросс-платформенно (Core + тесты) — работает и в облаке:
dotnet test tests/RevitNvf.Core.Tests

# Полная сборка (только Windows с установленным Revit 2024):
#   открыть RevitNvf.sln в VS 2022 и собрать, либо:
#   msbuild RevitNvf.sln /p:Configuration=Debug
```

Post-build для `RevitNvf.Revit`: копировать сборку и `RevitNvf.addin` в
`%AppData%\Autodesk\Revit\Addins\2024`.

## Skills (`.claude/skills/`)

- **revit-api-2024** — идиомы Revit API 2024 (транзакции, единицы, коллекторы,
  Ribbon/.addin, семейства, ExternalEvent).
- **nvf-domain** — терминология и нормативка НВФ, состав слоёв, правила шага.
- **revit-family-generation** — конвенции параметрических семейств элементов ПОК.

## Revit-MCP

Для живой работы с открытой моделью Revit см. `docs/REVIT_MCP_SETUP.md`
и шаблон `.mcp.json`. Работает **локально** (Revit + сокет-мост), в облаке — нет.

## Нормативные ориентиры

СП 426.1325800.2020 (проектирование и устройство НВФ), ГОСТ Р 58154-2018,
ГОСТ Р 58155-2018, СП 50.13330 (тепловая защита). Используем как **параметры
и ограничения** (шаги крепежа, толщины слоёв), а не как поверочный расчёт.

## Документы

- `docs/SPECIFICATION.md` — ТЗ и доменная модель.
- `docs/ROADMAP.md` — пошаговый план разработки.
- `docs/REVIT_MCP_SETUP.md` — настройка Revit-MCP.
