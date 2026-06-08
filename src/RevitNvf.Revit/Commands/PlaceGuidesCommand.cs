using System;
using System.Collections.Generic;
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
    /// Шаг 5: вертикальные направляющие по линиям кронштейнов. Домен строит отрезки,
    /// адаптер материализует их линиями модели на плоскости выбранной грани.
    /// Линии модели — временная визуализация; реальные профили (sweep по семейству)
    /// появятся на следующих шагах.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PlaceGuidesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference faceRef = uidoc.Selection.PickObject(
                    ObjectType.Face, "Выберите плоскую грань стены для раскладки направляющих");
                Element host = doc.GetElement(faceRef);
                if (!(host.GetGeometryObjectFromReference(faceRef) is PlanarFace planarFace))
                {
                    message = "Выбранная грань не плоская. Поддерживается раскладка только на плоских гранях.";
                    return Result.Failed;
                }

                var faceSubstrate = new PlanarFaceSubstrate(planarFace, faceRef);
                Substrate substrate = faceSubstrate.ToSubstrate();

                FacadeSystem system = FacadeParametersStore.Current.ToBracketSystem();
                IReadOnlyList<Guide> guides = GuideLayout.Generate(substrate, system);
                if (guides.Count == 0)
                {
                    message = "Для выбранной грани и параметров системы направляющие не размещаются " +
                              "(нужно минимум два ряда кронштейнов по высоте).";
                    return Result.Cancelled;
                }

                using (var t = new Transaction(doc, "НВФ: направляющие"))
                {
                    t.Start();

                    Plane plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
                    SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                    foreach (Guide guide in guides)
                    {
                        XYZ start = faceSubstrate.ToWorldPoint(guide.Start);
                        XYZ end = faceSubstrate.ToWorldPoint(guide.End);
                        Line line = Line.CreateBound(start, end);
                        doc.Create.NewModelCurve(line, sketchPlane);
                    }

                    t.Commit();
                }

                TaskDialog.Show("RevitNvf", $"Размещено направляющих: {guides.Count}.");
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
