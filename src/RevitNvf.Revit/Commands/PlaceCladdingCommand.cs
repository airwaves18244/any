using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;
using RevitNvf.Revit.Geometry;
using RevitNvf.UI;

namespace RevitNvf.Revit.Commands
{
    /// <summary>
    /// Шаг 6/7: раскладка облицовки на выбранной грани с учётом шва, режима привязки
    /// и доборных элементов (параметры — из панели). Элементы визуализируются
    /// прямоугольными контурами (линии модели) до появления семейств панелей.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PlaceCladdingCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference faceRef = uidoc.Selection.PickObject(
                    ObjectType.Face, "Выберите плоскую грань стены для раскладки облицовки");
                Element host = doc.GetElement(faceRef);
                if (!(host.GetGeometryObjectFromReference(faceRef) is PlanarFace planarFace))
                {
                    message = "Выбранная грань не плоская. Поддерживается раскладка только на плоских гранях.";
                    return Result.Failed;
                }

                var faceSubstrate = new PlanarFaceSubstrate(planarFace, faceRef);
                Substrate substrate = faceSubstrate.ToSubstrate();

                Cladding cladding = FacadeParametersStore.Current.ToCladding();
                IReadOnlyList<Panel> panels = PanelLayout.Generate(substrate, cladding);
                if (panels.Count == 0)
                {
                    message = "Для выбранной грани и параметров облицовки элементы не размещаются.";
                    return Result.Cancelled;
                }

                using (var t = new Transaction(doc, "НВФ: облицовка"))
                {
                    t.Start();

                    Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                    SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                    foreach (Panel panel in panels)
                    {
                        ModelCurveFactory.CreatePanelOutline(doc, sketchPlane, faceSubstrate, panel);
                    }

                    t.Commit();
                }

                int edge = panels.Count(p => p.IsEdge);
                TaskDialog.Show("RevitNvf",
                    $"Размещено элементов облицовки: {panels.Count} (доборных: {edge}).");
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
