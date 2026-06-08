using System.Collections.Generic;

namespace RevitNvf.Core.Model
{
    /// <summary>
    /// Именованный пресет фасадной системы — связка параметров кронштейнов,
    /// облицовки и пирога. Используется панелью параметров (шаг 8) как заготовка.
    /// </summary>
    public sealed class FacadePreset
    {
        public string Name { get; }
        public FacadeSystem Bracket { get; }
        public Cladding Cladding { get; }
        public PieLayers Layers { get; }

        public FacadePreset(string name, FacadeSystem bracket, Cladding cladding, PieLayers layers)
        {
            Name = name;
            Bracket = bracket;
            Cladding = cladding;
            Layers = layers;
        }

        public override string ToString() => Name;

        /// <summary>Встроенные пресеты распространённых систем.</summary>
        public static IReadOnlyList<FacadePreset> Defaults => new[]
        {
            Ceramogranite(),
            MetalCassette(),
            Composite()
        };

        public static FacadePreset Ceramogranite() => new FacadePreset(
            "Керамогранит",
            new FacadeSystem(1000, 1000, 100, 100),
            new Cladding(600, 600, 8, PanelStartMode.Corner, includeEdgePanels: true),
            new PieLayers(100, 40, 10));

        public static FacadePreset MetalCassette() => new FacadePreset(
            "Металлокассеты",
            new FacadeSystem(800, 600, 100, 100),
            new Cladding(900, 600, 12, PanelStartMode.Centered),
            new PieLayers(120, 50, 1));

        public static FacadePreset Composite() => new FacadePreset(
            "Композит (АКП)",
            new FacadeSystem(700, 600, 80, 80),
            new Cladding(1200, 800, 15, PanelStartMode.Centered),
            new PieLayers(100, 40, 4));
    }
}
