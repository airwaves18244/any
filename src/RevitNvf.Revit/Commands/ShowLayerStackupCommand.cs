using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitNvf.Core.Layout;
using RevitNvf.Core.Model;
using RevitNvf.UI;

namespace RevitNvf.Revit.Commands
{
    /// <summary>
    /// Шаг 6: показывает состав пирога НВФ (слои, толщины, смещения) по параметрам
    /// из панели. Геометрию не создаёт.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ShowLayerStackupCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                PieLayers pie = FacadeParametersStore.Current.ToPieLayers();
                IReadOnlyList<Layer> layers = LayerStackup.Build(pie);

                var sb = new StringBuilder();
                sb.AppendLine("Слои от несущей стены наружу:");
                foreach (Layer layer in layers)
                {
                    sb.AppendLine($"  • {Describe(layer.Type)}: {layer.Thickness:0.#} мм (смещение {layer.OffsetFromWall:0.#} мм)");
                }
                sb.AppendLine();
                sb.AppendLine($"Суммарная толщина пирога: {LayerStackup.TotalThickness(pie):0.#} мм");

                TaskDialog.Show("RevitNvf — состав пирога", sb.ToString());
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private static string Describe(LayerType type)
        {
            switch (type)
            {
                case LayerType.Insulation: return "Утеплитель";
                case LayerType.AirGap: return "Воздушный зазор";
                case LayerType.Cladding: return "Облицовка";
                default: return type.ToString();
            }
        }
    }
}
