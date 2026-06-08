namespace RevitNvf.Core.Reporting
{
    /// <summary>
    /// Сводная ведомость материалов раскладки (количества/длины/площади).
    /// Все длины в мм, площади в мм².
    /// </summary>
    public sealed class TakeoffReport
    {
        public int BracketCount { get; }
        public int GuideCount { get; }
        public double GuideTotalLengthMm { get; }
        public int PanelCountTotal { get; }
        public int PanelCountEdge { get; }
        public double PanelTotalAreaMm2 { get; }
        public double PieTotalThicknessMm { get; }

        public TakeoffReport(
            int bracketCount,
            int guideCount,
            double guideTotalLengthMm,
            int panelCountTotal,
            int panelCountEdge,
            double panelTotalAreaMm2,
            double pieTotalThicknessMm)
        {
            BracketCount = bracketCount;
            GuideCount = guideCount;
            GuideTotalLengthMm = guideTotalLengthMm;
            PanelCountTotal = panelCountTotal;
            PanelCountEdge = panelCountEdge;
            PanelTotalAreaMm2 = panelTotalAreaMm2;
            PieTotalThicknessMm = pieTotalThicknessMm;
        }

        /// <summary>Число полных (не доборных) элементов облицовки.</summary>
        public int PanelCountFull => PanelCountTotal - PanelCountEdge;

        /// <summary>Суммарная площадь облицовки, м².</summary>
        public double PanelTotalAreaM2 => PanelTotalAreaMm2 / 1_000_000.0;

        /// <summary>Суммарная длина направляющих, м.</summary>
        public double GuideTotalLengthM => GuideTotalLengthMm / 1000.0;
    }
}
