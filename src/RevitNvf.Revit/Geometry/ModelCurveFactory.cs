using Autodesk.Revit.DB;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Model;

namespace RevitNvf.Revit.Geometry
{
    /// <summary>
    /// Создание линий модели на плоскости грани из доменных элементов (мм).
    /// Временная визуализация раскладки до появления реальных семейств профилей/панелей.
    /// </summary>
    internal static class ModelCurveFactory
    {
        public static void CreateLine(Document doc, SketchPlane sketchPlane, PlanarFaceSubstrate face, Point2d startMm, Point2d endMm)
        {
            XYZ start = face.ToWorldPoint(startMm);
            XYZ end = face.ToWorldPoint(endMm);
            doc.Create.NewModelCurve(Line.CreateBound(start, end), sketchPlane);
        }

        /// <summary>Прямоугольный контур панели из четырёх линий модели.</summary>
        public static void CreatePanelOutline(Document doc, SketchPlane sketchPlane, PlanarFaceSubstrate face, Panel panel)
        {
            double x0 = panel.Origin.X;
            double y0 = panel.Origin.Y;
            double x1 = x0 + panel.Width;
            double y1 = y0 + panel.Height;

            var bottomLeft = new Point2d(x0, y0);
            var bottomRight = new Point2d(x1, y0);
            var topRight = new Point2d(x1, y1);
            var topLeft = new Point2d(x0, y1);

            CreateLine(doc, sketchPlane, face, bottomLeft, bottomRight);
            CreateLine(doc, sketchPlane, face, bottomRight, topRight);
            CreateLine(doc, sketchPlane, face, topRight, topLeft);
            CreateLine(doc, sketchPlane, face, topLeft, bottomLeft);
        }
    }
}
