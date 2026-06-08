using Autodesk.Revit.DB;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Model;

namespace RevitNvf.Revit.Geometry
{
    /// <summary>
    /// Мост между плоской гранью Revit и доменной грани <see cref="Substrate"/>.
    /// Локальные координаты домена (мм) отсчитываются от нижнего левого угла
    /// UV-габарита грани; перевод в мировые точки — через параметризацию грани.
    /// </summary>
    internal sealed class PlanarFaceSubstrate
    {
        private readonly PlanarFace _face;
        private readonly BoundingBoxUV _bbox;

        public Reference Reference { get; }

        public PlanarFaceSubstrate(PlanarFace face, Reference reference)
        {
            _face = face;
            Reference = reference;
            _bbox = face.GetBoundingBox();
        }

        /// <summary>Направление «вправо» грани (для ориентации семейств).</summary>
        public XYZ XVector => _face.XVector;

        public Substrate ToSubstrate()
        {
            double widthMm = UnitsConvert.FeetToMm(_bbox.Max.U - _bbox.Min.U);
            double heightMm = UnitsConvert.FeetToMm(_bbox.Max.V - _bbox.Min.V);
            return new Substrate(widthMm, heightMm);
        }

        /// <summary>Локальная точка грани (мм) → мировая точка Revit (футы).</summary>
        public XYZ ToWorldPoint(Point2d localMm)
        {
            double u = _bbox.Min.U + UnitsConvert.MmToFeet(localMm.X);
            double v = _bbox.Min.V + UnitsConvert.MmToFeet(localMm.Y);
            return _face.Evaluate(new UV(u, v));
        }
    }
}
