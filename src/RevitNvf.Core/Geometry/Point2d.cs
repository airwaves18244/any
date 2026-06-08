using System;

namespace RevitNvf.Core.Geometry
{
    /// <summary>
    /// Точка на плоскости фасада в миллиметрах (СИ).
    /// Домен работает в мм; перевод в футы Revit — на границе адаптера.
    /// </summary>
    public readonly struct Point2d : IEquatable<Point2d>
    {
        public double X { get; }
        public double Y { get; }

        public Point2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2d WithX(double x) => new Point2d(x, Y);
        public Point2d WithY(double y) => new Point2d(X, y);

        public static Point2d operator +(Point2d a, Point2d b) => new Point2d(a.X + b.X, a.Y + b.Y);
        public static Point2d operator -(Point2d a, Point2d b) => new Point2d(a.X - b.X, a.Y - b.Y);

        public bool Equals(Point2d other) => X.Equals(other.X) && Y.Equals(other.Y);

        public override bool Equals(object? obj) => obj is Point2d other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString() => $"({X:0.###}, {Y:0.###}) мм";
    }
}
