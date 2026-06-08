using System;
using RevitNvf.Core.Geometry;

namespace RevitNvf.Core.Model
{
    public enum GuideOrientation
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    /// Направляющая (профиль) ПОК — отрезок на грани (мм) в локальных координатах.
    /// На шаге 5 — вертикальные направляющие по линиям кронштейнов.
    /// </summary>
    public readonly struct Guide
    {
        public Point2d Start { get; }
        public Point2d End { get; }
        public GuideOrientation Orientation { get; }

        public Guide(Point2d start, Point2d end, GuideOrientation orientation)
        {
            Start = start;
            End = end;
            Orientation = orientation;
        }

        /// <summary>Длина направляющей, мм.</summary>
        public double Length
        {
            get
            {
                double dx = End.X - Start.X;
                double dy = End.Y - Start.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }

        public override string ToString() => $"Guide {Orientation}: {Start} → {End}";
    }
}
