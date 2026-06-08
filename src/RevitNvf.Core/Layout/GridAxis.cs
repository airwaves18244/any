using System;
using System.Collections.Generic;

namespace RevitNvf.Core.Layout
{
    /// <summary>
    /// Общий генератор узлов регулярной сетки вдоль одной оси (мм).
    /// Узлы: edgeOffset, edgeOffset+step, … пока не выйдут за рабочую длину
    /// (length − 2·edgeOffset). Используется и раскладкой кронштейнов, и направляющих.
    /// </summary>
    public static class GridAxis
    {
        private const double Epsilon = 1e-6;

        public static IReadOnlyList<double> Positions(double length, double edgeOffset, double step)
        {
            if (step <= 0)
                throw new ArgumentOutOfRangeException(nameof(step), "Шаг сетки должен быть положительным (мм).");

            double usable = length - 2 * edgeOffset;
            if (usable < -Epsilon)
            {
                return Array.Empty<double>();
            }

            int lastIndex = (int)Math.Floor((usable + Epsilon) / step);
            var positions = new double[lastIndex + 1];
            for (int i = 0; i <= lastIndex; i++)
            {
                positions[i] = edgeOffset + i * step;
            }

            return positions;
        }
    }
}
