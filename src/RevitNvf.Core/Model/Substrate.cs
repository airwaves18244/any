using System;

namespace RevitNvf.Core.Model
{
    /// <summary>
    /// Прямоугольная несущая грань (плоская стена), на которую раскладывается ПОК.
    /// Локальная система координат грани, мм: X — по ширине, Y — по высоте,
    /// начало (0,0) — нижний левый угол. Привязка к реальной геометрии Revit —
    /// на границе адаптера.
    /// </summary>
    public sealed class Substrate
    {
        public double Width { get; }
        public double Height { get; }

        public Substrate(double width, double height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Ширина грани должна быть положительной (мм).");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Высота грани должна быть положительной (мм).");

            Width = width;
            Height = height;
        }
    }
}
