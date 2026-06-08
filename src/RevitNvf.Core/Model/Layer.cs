namespace RevitNvf.Core.Model
{
    /// <summary>Тип слоя пирога НВФ.</summary>
    public enum LayerType
    {
        Insulation,
        AirGap,
        Cladding
    }

    /// <summary>
    /// Слой пирога НВФ: тип, толщина и смещение от несущей стены наружу (мм).
    /// </summary>
    public readonly struct Layer
    {
        public LayerType Type { get; }
        public double Thickness { get; }

        /// <summary>Смещение внутренней грани слоя от несущей стены наружу, мм.</summary>
        public double OffsetFromWall { get; }

        public Layer(LayerType type, double thickness, double offsetFromWall)
        {
            Type = type;
            Thickness = thickness;
            OffsetFromWall = offsetFromWall;
        }

        public override string ToString() =>
            $"{Type}: {Thickness:0.#} мм @ {OffsetFromWall:0.#} мм";
    }
}
