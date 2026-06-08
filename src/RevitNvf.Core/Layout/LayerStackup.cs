using System;
using System.Collections.Generic;
using RevitNvf.Core.Model;

namespace RevitNvf.Core.Layout
{
    /// <summary>
    /// Сборка пирога НВФ из толщин: упорядоченные слои от несущей стены наружу
    /// с накопленным смещением. Без зависимостей от Revit.
    /// </summary>
    public static class LayerStackup
    {
        public static IReadOnlyList<Layer> Build(PieLayers pie)
        {
            if (pie is null) throw new ArgumentNullException(nameof(pie));

            double offset = 0;
            var layers = new List<Layer>(3);

            layers.Add(new Layer(LayerType.Insulation, pie.InsulationThickness, offset));
            offset += pie.InsulationThickness;

            layers.Add(new Layer(LayerType.AirGap, pie.AirGapThickness, offset));
            offset += pie.AirGapThickness;

            layers.Add(new Layer(LayerType.Cladding, pie.CladdingThickness, offset));

            return layers;
        }

        /// <summary>Суммарная толщина пирога, мм.</summary>
        public static double TotalThickness(PieLayers pie)
        {
            if (pie is null) throw new ArgumentNullException(nameof(pie));
            return pie.InsulationThickness + pie.AirGapThickness + pie.CladdingThickness;
        }
    }
}
