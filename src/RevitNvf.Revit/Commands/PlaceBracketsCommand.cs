using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;
using RevitNvf.Revit.Families;
using RevitNvf.Revit.Geometry;
using RevitNvf.UI;

namespace RevitNvf.Revit.Commands
{
    /// <summary>
    /// Шаг 4 (MVP): пользователь выбирает плоскую грань стены, домен раскладывает
    /// кронштейны сеткой, адаптер размещает экземпляры семейства на грани в одной
    /// транзакции.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PlaceBracketsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // 1. Выбор плоской грани.
                Reference faceRef = uidoc.Selection.PickObject(
                    ObjectType.Face, "Выберите плоскую грань стены для раскладки кронштейнов");
                Element host = doc.GetElement(faceRef);
                if (!(host.GetGeometryObjectFromReference(faceRef) is PlanarFace planarFace))
                {
                    message = "Выбранная грань не плоская. Поддерживается раскладка только на плоских гранях.";
                    return Result.Failed;
                }

                // 2. Грань → доменная Substrate (мм).
                var faceSubstrate = new PlanarFaceSubstrate(planarFace, faceRef);
                Substrate substrate = faceSubstrate.ToSubstrate();

                // 3. Раскладка в домене (параметры — из панели).
                FacadeSystem system = FacadeParametersStore.Current.ToBracketSystem();
                IReadOnlyList<Bracket> brackets = BracketLayout.Generate(substrate, system);
                if (brackets.Count == 0)
                {
                    message = "Для выбранной грани и параметров системы кронштейны не размещаются.";
                    return Result.Cancelled;
                }

                // 4. Семейство кронштейна.
                FamilySymbol symbol = FamilyResolver.FindSymbolByFamilyName(doc, FacadeSystemDefaults.BracketFamilyName);
                if (symbol == null)
                {
                    message = $"Не найдено семейство кронштейна «{FacadeSystemDefaults.BracketFamilyName}». " +
                              "Загрузите face-based семейство кронштейна в проект и повторите.";
                    return Result.Failed;
                }

                // 5. Размещение в одной транзакции.
                XYZ referenceDirection = faceSubstrate.XVector;
                using (var t = new Transaction(doc, "НВФ: раскладка кронштейнов"))
                {
                    t.Start();
                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                        doc.Regenerate();
                    }

                    foreach (Bracket bracket in brackets)
                    {
                        XYZ location = faceSubstrate.ToWorldPoint(bracket.Position);
                        doc.Create.NewFamilyInstance(faceRef, location, referenceDirection, symbol);
                    }

                    t.Commit();
                }

                TaskDialog.Show("RevitNvf", $"Размещено кронштейнов: {brackets.Count}.");
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled; // пользователь отменил выбор (Esc)
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
