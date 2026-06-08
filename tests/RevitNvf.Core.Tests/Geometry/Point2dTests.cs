using RevitNvf.Core.Geometry;
using Xunit;

namespace RevitNvf.Core.Tests.Geometry
{
    /// <summary>
    /// Smoke-тесты скаффолдинга (шаг 1): подтверждают, что Core собирается
    /// и тесты прогоняются. Содержательные тесты раскладки — шаг 3.
    /// </summary>
    public class Point2dTests
    {
        [Fact]
        public void Constructor_StoresCoordinates()
        {
            var p = new Point2d(120.0, -45.5);

            Assert.Equal(120.0, p.X);
            Assert.Equal(-45.5, p.Y);
        }

        [Fact]
        public void Addition_SumsComponents()
        {
            var sum = new Point2d(10, 20) + new Point2d(5, -3);

            Assert.Equal(new Point2d(15, 17), sum);
        }

        [Fact]
        public void Equality_IsValueBased()
        {
            Assert.Equal(new Point2d(1, 2), new Point2d(1, 2));
            Assert.NotEqual(new Point2d(1, 2), new Point2d(2, 1));
        }
    }
}
