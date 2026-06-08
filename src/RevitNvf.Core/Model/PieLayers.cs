using System;

namespace RevitNvf.Core.Model
{
    /// <summary>
    /// Толщины слоёв пирога НВФ (мм): утеплитель, воздушный зазор, облицовка.
    /// Ветрозащитная мембрана не учитывается по толщине. Без зависимостей от Revit.
    /// </summary>
    public sealed class PieLayers
    {
        public double InsulationThickness { get; }
        public double AirGapThickness { get; }
        public double CladdingThickness { get; }

        public PieLayers(double insulationThickness, double airGapThickness, double claddingThickness)
        {
            if (insulationThickness <= 0)
                throw new ArgumentOutOfRangeException(nameof(insulationThickness), "Толщина утеплителя должна быть положительной (мм).");
            if (airGapThickness <= 0)
                throw new ArgumentOutOfRangeException(nameof(airGapThickness), "Толщина воздушного зазора должна быть положительной (мм).");
            if (claddingThickness <= 0)
                throw new ArgumentOutOfRangeException(nameof(claddingThickness), "Толщина облицовки должна быть положительной (мм).");

            InsulationThickness = insulationThickness;
            AirGapThickness = airGapThickness;
            CladdingThickness = claddingThickness;
        }
    }
}
