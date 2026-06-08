using RevitNvf.Core.Model;
using RevitNvf.Core.Reporting;
using Xunit;

namespace RevitNvf.Core.Tests.Reporting
{
    public class FacadeTakeoffTests
    {
        [Fact]
        public void Compute_AggregatesAllLayouts()
        {
            var substrate = new Substrate(3000, 3000);
            var bracketSystem = new FacadeSystem(1000, 1000, 100, 100);
            var cladding = new Cladding(600, 600, 10, PanelStartMode.Corner);
            var pie = new PieLayers(100, 40, 10);

            TakeoffReport report = FacadeTakeoff.Compute(substrate, bracketSystem, cladding, pie);

            Assert.Equal(9, report.BracketCount);                 // 3×3
            Assert.Equal(3, report.GuideCount);                   // 3 колонки
            Assert.Equal(6000, report.GuideTotalLengthMm, 6);     // 3 × 2000
            Assert.Equal(16, report.PanelCountTotal);             // 4×4
            Assert.Equal(0, report.PanelCountEdge);
            Assert.Equal(16, report.PanelCountFull);
            Assert.Equal(5_760_000, report.PanelTotalAreaMm2, 6); // 16 × 600 × 600
            Assert.Equal(150, report.PieTotalThicknessMm);
        }

        [Fact]
        public void Compute_CountsEdgePanels()
        {
            var substrate = new Substrate(3000, 3000);
            var bracketSystem = new FacadeSystem(1000, 1000, 100, 100);
            var cladding = new Cladding(600, 600, 10, PanelStartMode.Corner, includeEdgePanels: true);
            var pie = new PieLayers(100, 40, 10);

            TakeoffReport report = FacadeTakeoff.Compute(substrate, bracketSystem, cladding, pie);

            Assert.Equal(25, report.PanelCountTotal);
            Assert.Equal(9, report.PanelCountEdge);
            Assert.Equal(16, report.PanelCountFull);
        }
    }
}
