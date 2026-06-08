using System;
using System.Linq;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;
using Xunit;

namespace RevitNvf.Core.Tests.Layout
{
    public class BracketLayoutTests
    {
        [Fact]
        public void Generate_RegularGrid_ProducesExpectedCountAndCorners()
        {
            // Грань 3000×3000, шаг 1000, отступ 100.
            // По каждой оси: 100, 1100, 2100 (3100 > 2900 — стоп) => 3 узла, всего 9.
            var substrate = new Substrate(3000, 3000);
            var system = new FacadeSystem(bracketStepX: 1000, bracketStepY: 1000,
                                          bracketEdgeOffsetX: 100, bracketEdgeOffsetY: 100);

            var brackets = BracketLayout.Generate(substrate, system);

            Assert.Equal(9, brackets.Count);
            // Порядок: ряды снизу вверх, в ряду слева направо.
            Assert.Equal(new Point2d(100, 100), brackets.First().Position);
            Assert.Equal(new Point2d(2100, 2100), brackets.Last().Position);
        }

        [Fact]
        public void Generate_OrdersRowsBottomUpThenLeftRight()
        {
            var substrate = new Substrate(2000, 2000);
            var system = new FacadeSystem(1000, 1000, 0, 0);

            var positions = BracketLayout.Generate(substrate, system)
                                         .Select(b => b.Position)
                                         .ToArray();

            // Ось: 0, 1000, 2000 => 3×3 = 9. Первый ряд y=0: (0,0),(1000,0),(2000,0)...
            Assert.Equal(new Point2d(0, 0), positions[0]);
            Assert.Equal(new Point2d(1000, 0), positions[1]);
            Assert.Equal(new Point2d(2000, 0), positions[2]);
            Assert.Equal(new Point2d(0, 1000), positions[3]);
        }

        [Fact]
        public void Generate_IsIdempotent()
        {
            var substrate = new Substrate(4200, 2700);
            var system = new FacadeSystem(900, 600, 150, 120);

            var first = BracketLayout.Generate(substrate, system).Select(b => b.Position).ToArray();
            var second = BracketLayout.Generate(substrate, system).Select(b => b.Position).ToArray();

            Assert.Equal(first, second);
        }

        [Fact]
        public void Generate_AllBracketsWithinEdgeOffsets()
        {
            var substrate = new Substrate(5000, 3000);
            var system = new FacadeSystem(800, 700, 200, 150);

            var brackets = BracketLayout.Generate(substrate, system);

            Assert.NotEmpty(brackets);
            Assert.All(brackets, b =>
            {
                Assert.InRange(b.Position.X, system.BracketEdgeOffsetX, substrate.Width - system.BracketEdgeOffsetX);
                Assert.InRange(b.Position.Y, system.BracketEdgeOffsetY, substrate.Height - system.BracketEdgeOffsetY);
            });
        }

        [Fact]
        public void Generate_OffsetLargerThanFace_YieldsNoBrackets()
        {
            // 2·отступ по X (200) > ширина (150) => по X узлов нет => 0 кронштейнов.
            var substrate = new Substrate(150, 3000);
            var system = new FacadeSystem(1000, 1000, 100, 100);

            var brackets = BracketLayout.Generate(substrate, system);

            Assert.Empty(brackets);
        }

        [Fact]
        public void Generate_UsableSpanSmallerThanStep_YieldsSingleNodePerAxis()
        {
            // usable = 600 - 200 = 400 < step 1000 => по одному узлу на ось (в отступе).
            var substrate = new Substrate(600, 600);
            var system = new FacadeSystem(1000, 1000, 100, 100);

            var brackets = BracketLayout.Generate(substrate, system);

            var single = Assert.Single(brackets);
            Assert.Equal(new Point2d(100, 100), single.Position);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-500)]
        public void FacadeSystem_NonPositiveStep_Throws(double step)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FacadeSystem(step, 1000));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FacadeSystem(1000, step));
        }

        [Fact]
        public void Generate_NullArguments_Throw()
        {
            var substrate = new Substrate(1000, 1000);
            var system = new FacadeSystem(500, 500);

            Assert.Throws<ArgumentNullException>(() => BracketLayout.Generate(null!, system));
            Assert.Throws<ArgumentNullException>(() => BracketLayout.Generate(substrate, null!));
        }
    }
}
