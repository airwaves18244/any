using System;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;
using Xunit;

namespace RevitNvf.Core.Tests.Layout
{
    public class LayerStackupTests
    {
        [Fact]
        public void Build_OrdersLayersFromWallWithCumulativeOffsets()
        {
            var pie = new PieLayers(insulationThickness: 100, airGapThickness: 40, claddingThickness: 10);

            var layers = LayerStackup.Build(pie);

            Assert.Equal(3, layers.Count);

            Assert.Equal(LayerType.Insulation, layers[0].Type);
            Assert.Equal(0, layers[0].OffsetFromWall);

            Assert.Equal(LayerType.AirGap, layers[1].Type);
            Assert.Equal(100, layers[1].OffsetFromWall);

            Assert.Equal(LayerType.Cladding, layers[2].Type);
            Assert.Equal(140, layers[2].OffsetFromWall);
        }

        [Fact]
        public void TotalThickness_SumsLayers()
        {
            var pie = new PieLayers(100, 40, 10);

            Assert.Equal(150, LayerStackup.TotalThickness(pie));
        }

        [Theory]
        [InlineData(0, 40, 10)]
        [InlineData(100, 0, 10)]
        [InlineData(100, 40, 0)]
        public void PieLayers_NonPositiveThickness_Throws(double insulation, double airGap, double cladding)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PieLayers(insulation, airGap, cladding));
        }
    }
}
