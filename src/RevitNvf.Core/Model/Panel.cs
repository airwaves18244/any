using RevitNvf.Core.Geometry;

namespace RevitNvf.Core.Model
{
    /// <summary>
    /// Облицовочный элемент: прямоугольник на грани (мм), привязка — нижний левый угол.
    /// <see cref="IsEdge"/> помечает доборный (краевой/обрезной) элемент.
    /// </summary>
    public readonly struct Panel
    {
        public Point2d Origin { get; }
        public double Width { get; }
        public double Height { get; }
        public bool IsEdge { get; }

        public Panel(Point2d origin, double width, double height, bool isEdge = false)
        {
            Origin = origin;
            Width = width;
            Height = height;
            IsEdge = isEdge;
        }

        /// <summary>Площадь элемента, мм².</summary>
        public double Area => Width * Height;

        public override string ToString() =>
            $"Panel {Origin} {Width:0.###}×{Height:0.###}{(IsEdge ? " (доборный)" : string.Empty)}";
    }
}
