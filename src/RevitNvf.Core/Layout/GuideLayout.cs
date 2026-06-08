using System;
using System.Collections.Generic;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Model;

namespace RevitNvf.Core.Layout
{
    /// <summary>
    /// Раскладка вертикальных направляющих по линиям кронштейнов. На каждую
    /// колонку кронштейнов (узел оси X) приходится одна вертикальная направляющая,
    /// идущая от нижнего ряда кронштейнов к верхнему. Чистая функция,
    /// детерминирована и идемпотентна. Все величины в мм. Без зависимостей от Revit.
    /// </summary>
    public static class GuideLayout
    {
        public static IReadOnlyList<Guide> Generate(Substrate substrate, FacadeSystem system)
        {
            if (substrate is null) throw new ArgumentNullException(nameof(substrate));
            if (system is null) throw new ArgumentNullException(nameof(system));

            IReadOnlyList<double> xs = GridAxis.Positions(substrate.Width, system.BracketEdgeOffsetX, system.BracketStepX);
            IReadOnlyList<double> ys = GridAxis.Positions(substrate.Height, system.BracketEdgeOffsetY, system.BracketStepY);

            // Нужно минимум два ряда кронштейнов по высоте, чтобы направляющей было
            // что соединять; иначе направляющие не строим.
            if (xs.Count == 0 || ys.Count < 2)
            {
                return Array.Empty<Guide>();
            }

            double yBottom = ys[0];
            double yTop = ys[ys.Count - 1];

            var guides = new List<Guide>(xs.Count);
            foreach (double x in xs)
            {
                guides.Add(new Guide(
                    new Point2d(x, yBottom),
                    new Point2d(x, yTop),
                    GuideOrientation.Vertical));
            }

            return guides;
        }
    }
}
