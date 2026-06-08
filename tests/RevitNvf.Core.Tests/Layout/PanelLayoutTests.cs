using System.Linq;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;
using Xunit;

namespace RevitNvf.Core.Tests.Layout
{
    public class PanelLayoutTests
    {
        [Fact]
        public void Generate_FullPanelsOnly_CornerStart()
        {
            // 3000×3000, панель 600, шов 10, шаг 610 => floor((3000+10)/610)=4 по оси => 16.
            var substrate = new Substrate(3000, 3000);
            var cladding = new Cladding(600, 600, 10, PanelStartMode.Corner);

            var panels = PanelLayout.Generate(substrate, cladding);

            Assert.Equal(16, panels.Count);
            Assert.All(panels, p => Assert.False(p.IsEdge));
            Assert.Equal(new Point2d(0, 0), panels.First().Origin);
            Assert.All(panels, p =>
            {
                Assert.Equal(600, p.Width);
                Assert.Equal(600, p.Height);
            });
        }

        [Fact]
        public void Generate_CenteredStart_OffsetsGrid()
        {
            // total = 4*600 + 3*10 = 2430; offset = (3000-2430)/2 = 285.
            var substrate = new Substrate(3000, 3000);
            var cladding = new Cladding(600, 600, 10, PanelStartMode.Centered);

            var panels = PanelLayout.Generate(substrate, cladding);

            Assert.Equal(16, panels.Count);
            Assert.Equal(new Point2d(285, 285), panels.First().Origin);
        }

        [Fact]
        public void Generate_WithEdgePanels_AddsDoborElements()
        {
            // edgeStart = 3*610+600+10 = 2440; edgeWidth = 560 (>0) => 5 ячеек по оси.
            var substrate = new Substrate(3000, 3000);
            var cladding = new Cladding(600, 600, 10, PanelStartMode.Corner, includeEdgePanels: true);

            var panels = PanelLayout.Generate(substrate, cladding);

            Assert.Equal(25, panels.Count);
            Assert.Equal(9, panels.Count(p => p.IsEdge)); // 5×5 минус 4×4 полных
            var edge = panels.Single(p => p.Origin.Equals(new Point2d(2440, 2440)));
            Assert.True(edge.IsEdge);
            Assert.Equal(560, edge.Width, 6);
            Assert.Equal(560, edge.Height, 6);
        }

        [Fact]
        public void Generate_CenteredIgnoresEdgePanels()
        {
            var substrate = new Substrate(3000, 3000);
            var cladding = new Cladding(600, 600, 10, PanelStartMode.Centered, includeEdgePanels: true);

            var panels = PanelLayout.Generate(substrate, cladding);

            Assert.Equal(16, panels.Count);
            Assert.DoesNotContain(panels, p => p.IsEdge);
        }

        [Fact]
        public void Generate_PanelLargerThanFace_YieldsEmpty()
        {
            var substrate = new Substrate(500, 3000);
            var cladding = new Cladding(600, 600, 10, PanelStartMode.Corner, includeEdgePanels: true);

            var panels = PanelLayout.Generate(substrate, cladding);

            Assert.Empty(panels);
        }

        [Fact]
        public void Generate_IsIdempotent()
        {
            var substrate = new Substrate(4200, 3300);
            var cladding = new Cladding(600, 450, 12, PanelStartMode.Corner, includeEdgePanels: true);

            var first = PanelLayout.Generate(substrate, cladding).Select(p => (p.Origin, p.Width, p.Height, p.IsEdge)).ToArray();
            var second = PanelLayout.Generate(substrate, cladding).Select(p => (p.Origin, p.Width, p.Height, p.IsEdge)).ToArray();

            Assert.Equal(first, second);
        }
    }
}
