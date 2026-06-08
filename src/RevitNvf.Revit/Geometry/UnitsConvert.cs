using Autodesk.Revit.DB;

namespace RevitNvf.Revit.Geometry
{
    /// <summary>
    /// Перевод длины между мм (домен/СИ) и внутренними единицами Revit (футы).
    /// Конвертация выполняется только на границе адаптера.
    /// </summary>
    internal static class UnitsConvert
    {
        public static double MmToFeet(double millimeters) =>
            UnitUtils.ConvertToInternalUnits(millimeters, UnitTypeId.Millimeters);

        public static double FeetToMm(double feet) =>
            UnitUtils.ConvertFromInternalUnits(feet, UnitTypeId.Millimeters);
    }
}
