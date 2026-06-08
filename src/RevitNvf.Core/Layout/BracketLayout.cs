using System;
using System.Collections.Generic;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Model;

namespace RevitNvf.Core.Layout
{
    /// <summary>
    /// Раскладка кронштейнов регулярной сеткой на прямоугольной грани.
    /// Чистая функция: вход (<see cref="Substrate"/> + <see cref="FacadeSystem"/>) →
    /// упорядоченный набор кронштейнов. Детерминирована и идемпотентна.
    /// Все величины в мм. Без зависимостей от Revit.
    /// </summary>
    public static class BracketLayout
    {
        private const double Epsilon = 1e-6;

        /// <summary>
        /// Строит сетку кронштейнов: ряды снизу вверх (Y), в каждом ряду — слева
        /// направо (X). Позиции считаются от отступа края с шагом из системы.
        /// Если грань уже двойного отступа по оси — по этой оси кронштейнов нет.
        /// </summary>
        public static IReadOnlyList<Bracket> Generate(Substrate substrate, FacadeSystem system)
        {
            if (substrate is null) throw new ArgumentNullException(nameof(substrate));
            if (system is null) throw new ArgumentNullException(nameof(system));

            IReadOnlyList<double> xs = AxisPositions(substrate.Width, system.BracketEdgeOffsetX, system.BracketStepX);
            IReadOnlyList<double> ys = AxisPositions(substrate.Height, system.BracketEdgeOffsetY, system.BracketStepY);

            var brackets = new List<Bracket>(xs.Count * ys.Count);
            foreach (double y in ys)
            {
                foreach (double x in xs)
                {
                    brackets.Add(new Bracket(new Point2d(x, y)));
                }
            }

            return brackets;
        }

        /// <summary>
        /// Координаты узлов вдоль оси: edgeOffset, edgeOffset+step, … пока не выйдут
        /// за пределы рабочей длины (length − 2·edgeOffset). Возвращает пусто,
        /// если двойной отступ превышает длину.
        /// </summary>
        private static IReadOnlyList<double> AxisPositions(double length, double edgeOffset, double step)
        {
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
