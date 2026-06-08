---
name: revit-api-2024
description: Идиомы и подводные камни Revit API 2024 (C#, .NET Framework 4.8) — транзакции, единицы измерения, FilteredElementCollector, Ribbon и .addin-манифест, создание/загрузка семейств, ExternalEvent. Использовать при написании или ревью кода в RevitNvf.Revit / RevitNvf.UI, при работе с Autodesk.Revit.* API, командами IExternalCommand/IExternalApplication, транзакциями и геометрией Revit.
---

# Revit API 2024 — идиомы и грабли

Revit 2024 = **.NET Framework 4.8**, x64. Сборки: `RevitAPI.dll`,
`RevitAPIUI.dll` (ссылки локальные, `Copy Local = False`).

## Точки входа

- **IExternalApplication** — `OnStartup`/`OnShutdown`, создание вкладки ленты,
  кнопок, регистрация dockable-панелей и `ExternalEvent`.
- **IExternalCommand** — `Execute(ExternalCommandData, ref string, ElementSet)`,
  возвращает `Result.Succeeded/Cancelled/Failed`.
- **.addin-манифест** — XML, регистрирует приложение/команды; кладётся в
  `%AppData%\Autodesk\Revit\Addins\2024`.

## Транзакции (любое изменение модели)

```csharp
using (var t = new Transaction(doc, "НВФ: раскладка кронштейнов"))
{
    t.Start();
    // ... изменения модели ...
    t.Commit(); // или t.RollBack()
}
```
- Вложенные изменения — `SubTransaction`; группировка для одного Undo —
  `TransactionGroup` (+ `Assimilate()`).
- Вне транзакции модель менять нельзя (бросит исключение).
- Чтение модели транзакции не требует.

## Единицы — внутренние = ФУТЫ

- Длина внутри Revit — в футах. Конвертация:
  `UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters)` и обратно
  `ConvertFromInternalUnits`.
- Доменный слой (`RevitNvf.Core`) считаем в **мм/СИ**; перевод в футы — только
  на границе адаптера, при создании/чтении геометрии Revit.

## Выборки

```csharp
var walls = new FilteredElementCollector(doc)
    .OfCategory(BuiltInCategory.OST_Walls)
    .WhereElementIsNotElementType()
    .Cast<Wall>();
```
- Фильтры классов/категорий быстрее пост-фильтрации в C#.
- `ElementId` **недолговечен** между сессиями — хранить `Element.UniqueId`.

## Геометрия и семейства

- Точки/векторы — `XYZ` (в футах). `Line.CreateBound`, `Transform`, `Plane`.
- Размещение экземпляра семейства: `doc.Create.NewFamilyInstance(...)` (есть
  перегрузки по точке/грани/линии/хосту). Тип должен быть **активирован**
  (`if (!symbol.IsActive) symbol.Activate();`) перед размещением.
- Загрузка семейства: `doc.LoadFamily(path, out Family family)`.
- После массовых изменений — `doc.Regenerate()` перед чтением новой геометрии.

## Потоки и UI

- Revit API — **однопоточный**, вызывается только из API-контекста.
- Из модальных/немодальных окон и фоновых потоков менять модель — через
  `ExternalEvent` + `IExternalEventHandler` (`externalEvent.Raise()`).
- Dockable-панель — `IDockablePaneProvider`, регистрация в `OnStartup`.

## Частые ошибки

- Изменение модели вне транзакции.
- Забыли активировать `FamilySymbol` перед `NewFamilyInstance`.
- Путаница единиц (мм против футов).
- Хранение `ElementId` вместо `UniqueId`.
- Обращение к API из не-API потока без `ExternalEvent`.
- Утечки: оборачивать `Transaction`/`SubTransaction` в `using`.
