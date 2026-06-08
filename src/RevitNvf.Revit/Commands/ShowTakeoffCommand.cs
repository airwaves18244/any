using System;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitNvf.Core.Model;
using RevitNvf.Core.Reporting;
using RevitNvf.Revit.Geometry;
using RevitNvf.UI;
using RevitNvf.UI.ViewModels;

namespace RevitNvf.Revit.Commands
{
    /// <summary>
    /// Шаг 7: сводная ведомость материалов по выбранной грани (кронштейны,
    /// направляющие, облицовка, пирог) — рассчитывается доменом и показывается диалогом.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ShowTakeoffCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference faceRef = uidoc.Selection.PickObject(
                    ObjectType.Face, "Выберите плоскую грань стены для подсчёта ведомости");
                Element host = doc.GetElement(faceRef);
                if (!(host.GetGeometryObjectFromReference(faceRef) is PlanarFace planarFace))
                {
                    message = "Выбранная грань не плоская.";
                    return Result.Failed;
                }

                var faceSubstrate = new PlanarFaceSubstrate(planarFace, faceRef);
                Substrate substrate = faceSubstrate.ToSubstrate();

                FacadeParametersViewModel p = FacadeParametersStore.Current;
                TakeoffReport report = FacadeTakeoff.Compute(
                    substrate, p.ToBracketSystem(), p.ToCladding(), p.ToPieLayers());

                var sb = new StringBuilder();
                sb.AppendLine($"Грань: {substrate.Width:0} × {substrate.Height:0} мм");
                sb.AppendLine();
                sb.AppendLine($"Кронштейны: {report.BracketCount} шт.");
                sb.AppendLine($"Направляющие: {report.GuideCount} шт., {report.GuideTotalLengthM:0.##} м");
                sb.AppendLine($"Облицовка: {report.PanelCountTotal} шт. " +
                              $"(полных {report.PanelCountFull}, доборных {report.PanelCountEdge})");
                sb.AppendLine($"Площадь облицовки: {report.PanelTotalAreaM2:0.##} м²");
                sb.AppendLine($"Толщина пирога: {report.PieTotalThicknessMm:0.#} мм");

                TaskDialog.Show("RevitNvf — ведомость материалов", sb.ToString());
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
