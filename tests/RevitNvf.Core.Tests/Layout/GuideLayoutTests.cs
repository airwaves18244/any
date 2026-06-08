using System.Linq;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;
using Xunit;

namespace RevitNvf.Core.Tests.Layout
{
    public class GuideLayoutTests
    {
        [Fact]
        public void Generate_OneVerticalGuidePerBracketColumn()
        {
            // 3000×3000, шаг 1000, отступ 100 => 3 колонки кронштейнов => 3 направляющие.
            var substrate = new Substrate(3000, 3000);
            var system = new FacadeSystem(1000, 1000, 100, 100);

            var guides = GuideLayout.Generate(substrate, system);

            Assert.Equal(3, guides.Count);
            Assert.All(guides, g => Assert.Equal(GuideOrientation.Vertical, g.Orientation));
        }

        [Fact]
        public void Generate_GuidesSpanFromBottomToTopBracketRow()
        {
            var substrate = new Substrate(3000, 3000);
            var system = new FacadeSystem(1000, 1000, 100, 100);

            var guide = GuideLayout.Generate(substrate, system).First();

            // Колонки/ряды: 100, 1100, 2100. Первая направляющая на X=100, от Y=100 до Y=2100.
            Assert.Equal(new Point2d(100, 100), guide.Start);
            Assert.Equal(new Point2d(100, 2100), guide.End);
            Assert.Equal(2000, guide.Length, 6);
        }

        [Fact]
        public void Generate_VerticalGuidesKeepConstantX()
        {
            var substrate = new Substrate(5000, 4000);
            var system = new FacadeSystem(800, 900, 150, 120);

            var guides = GuideLayout.Generate(substrate, system);

            Assert.NotEmpty(guides);
            Assert.All(guides, g =>
            {
                Assert.Equal(g.Start.X, g.End.X);
                Assert.True(g.End.Y > g.Start.Y);
            });
        }

        [Fact]
        public void Generate_SingleBracketRow_YieldsNoGuides()
        {
            // По высоте только один ряд (usable 400 < step 1000) => соединять нечего.
            var substrate = new Substrate(3000, 600);
            var system = new FacadeSystem(1000, 1000, 100, 100);

            var guides = GuideLayout.Generate(substrate, system);

            Assert.Empty(guides);
        }

        [Fact]
        public void Generate_IsIdempotent()
        {
            var substrate = new Substrate(4200, 3300);
            var system = new FacadeSystem(900, 700, 150, 120);

            var first = GuideLayout.Generate(substrate, system).Select(g => (g.Start, g.End)).ToArray();
            var second = GuideLayout.Generate(substrate, system).Select(g => (g.Start, g.End)).ToArray();

            Assert.Equal(first, second);
        }
    }
}
