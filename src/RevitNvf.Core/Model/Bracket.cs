using RevitNvf.Core.Geometry;

namespace RevitNvf.Core.Model
{
    /// <summary>
    /// Кронштейн ПОК на грани. На шаге 3 — только позиция (мм) в локальных
    /// координатах грани. Тип (несущий/опорный, фикс/скольж) и вылет добавятся позже.
    /// </summary>
    public readonly struct Bracket
    {
        public Point2d Position { get; }

        public Bracket(Point2d position)
        {
            Position = position;
        }

        public override string ToString() => $"Bracket {Position}";
    }
}
