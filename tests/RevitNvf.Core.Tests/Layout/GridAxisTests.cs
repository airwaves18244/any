using System;
using RevitNvf.Core.Layout;
using Xunit;

namespace RevitNvf.Core.Tests.Layout
{
    public class GridAxisTests
    {
        [Fact]
        public void Positions_StepsFromOffsetWithinUsableLength()
        {
            // length 3000, offset 100, step 1000 => 100, 1100, 2100 (3100 > 2900).
            var positions = GridAxis.Positions(3000, 100, 1000);

            Assert.Equal(new[] { 100.0, 1100.0, 2100.0 }, positions);
        }

        [Fact]
        public void Positions_ZeroOffset_StartsAtEdge()
        {
            var positions = GridAxis.Positions(2000, 0, 1000);

            Assert.Equal(new[] { 0.0, 1000.0, 2000.0 }, positions);
        }

        [Fact]
        public void Positions_UsableSpanSmallerThanStep_ReturnsSingleNode()
        {
            var positions = GridAxis.Positions(600, 100, 1000);

            Assert.Equal(new[] { 100.0 }, positions);
        }

        [Fact]
        public void Positions_DoubleOffsetExceedsLength_ReturnsEmpty()
        {
            var positions = GridAxis.Positions(150, 100, 1000);

            Assert.Empty(positions);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Positions_NonPositiveStep_Throws(double step)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GridAxis.Positions(1000, 0, step));
        }
    }
}
