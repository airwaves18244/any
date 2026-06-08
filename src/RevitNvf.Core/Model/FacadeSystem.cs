using System;

namespace RevitNvf.Core.Model
{
    /// <summary>
    /// Параметры фасадной системы (мм). Абстрактная/параметрическая конфигурация
    /// (system-agnostic). На шаге 3 заданы только параметры раскладки кронштейнов;
    /// направляющие, слои и облицовка добавятся на следующих шагах.
    /// </summary>
    public sealed class FacadeSystem
    {
        /// <summary>Шаг кронштейнов по горизонтали, мм.</summary>
        public double BracketStepX { get; }

        /// <summary>Шаг кронштейнов по вертикали, мм.</summary>
        public double BracketStepY { get; }

        /// <summary>Отступ первого ряда кронштейнов от боковых краёв грани, мм.</summary>
        public double BracketEdgeOffsetX { get; }

        /// <summary>Отступ первого ряда кронштейнов от верхнего/нижнего края грани, мм.</summary>
        public double BracketEdgeOffsetY { get; }

        public FacadeSystem(
            double bracketStepX,
            double bracketStepY,
            double bracketEdgeOffsetX = 0,
            double bracketEdgeOffsetY = 0)
        {
            if (bracketStepX <= 0)
                throw new ArgumentOutOfRangeException(nameof(bracketStepX), "Шаг кронштейнов по X должен быть положительным (мм).");
            if (bracketStepY <= 0)
                throw new ArgumentOutOfRangeException(nameof(bracketStepY), "Шаг кронштейнов по Y должен быть положительным (мм).");
            if (bracketEdgeOffsetX < 0)
                throw new ArgumentOutOfRangeException(nameof(bracketEdgeOffsetX), "Отступ по X не может быть отрицательным (мм).");
            if (bracketEdgeOffsetY < 0)
                throw new ArgumentOutOfRangeException(nameof(bracketEdgeOffsetY), "Отступ по Y не может быть отрицательным (мм).");

            BracketStepX = bracketStepX;
            BracketStepY = bracketStepY;
            BracketEdgeOffsetX = bracketEdgeOffsetX;
            BracketEdgeOffsetY = bracketEdgeOffsetY;
        }
    }
}
