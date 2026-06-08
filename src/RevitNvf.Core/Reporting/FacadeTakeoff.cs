using System;
using System.Collections.Generic;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;

namespace RevitNvf.Core.Reporting
{
    /// <summary>
    /// Считает сводную ведомость материалов по результатам раскладки: кронштейны,
    /// направляющие, облицовка (полные/доборные, площадь), толщина пирога.
    /// Чистая функция, без зависимостей от Revit.
    /// </summary>
    public static class FacadeTakeoff
    {
        public static TakeoffReport Compute(
            Substrate substrate,
            FacadeSystem bracketSystem,
            Cladding cladding,
            PieLayers pie)
        {
            if (substrate is null) throw new ArgumentNullException(nameof(substrate));
            if (bracketSystem is null) throw new ArgumentNullException(nameof(bracketSystem));
            if (cladding is null) throw new ArgumentNullException(nameof(cladding));
            if (pie is null) throw new ArgumentNullException(nameof(pie));

            IReadOnlyList<Bracket> brackets = BracketLayout.Generate(substrate, bracketSystem);
            IReadOnlyList<Guide> guides = GuideLayout.Generate(substrate, bracketSystem);
            IReadOnlyList<Panel> panels = PanelLayout.Generate(substrate, cladding);

            double guideLength = 0;
            foreach (Guide g in guides) guideLength += g.Length;

            int edgeCount = 0;
            double panelArea = 0;
            foreach (Panel p in panels)
            {
                if (p.IsEdge) edgeCount++;
                panelArea += p.Area;
            }

            return new TakeoffReport(
                bracketCount: brackets.Count,
                guideCount: guides.Count,
                guideTotalLengthMm: guideLength,
                panelCountTotal: panels.Count,
                panelCountEdge: edgeCount,
                panelTotalAreaMm2: panelArea,
                pieTotalThicknessMm: LayerStackup.TotalThickness(pie));
        }
    }
}
