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
        /// <summary>
        /// Строит сетку кронштейнов: ряды снизу вверх (Y), в каждом ряду — слева
        /// направо (X). Позиции считаются от отступа края с шагом из системы.
        /// Если грань уже двойного отступа по оси — по этой оси кронштейнов нет.
        /// </summary>
        public static IReadOnlyList<Bracket> Generate(Substrate substrate, FacadeSystem system)
        {
            if (substrate is null) throw new ArgumentNullException(nameof(substrate));
            if (system is null) throw new ArgumentNullException(nameof(system));

            IReadOnlyList<double> xs = GridAxis.Positions(substrate.Width, system.BracketEdgeOffsetX, system.BracketStepX);
            IReadOnlyList<double> ys = GridAxis.Positions(substrate.Height, system.BracketEdgeOffsetY, system.BracketStepY);

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
    }
}
