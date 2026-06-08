using System;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitNvf.Revit.Families
{
    /// <summary>
    /// Поиск типоразмеров семейств в документе по имени семейства.
    /// Активацию символа выполняет вызывающий код внутри транзакции.
    /// </summary>
    internal static class FamilyResolver
    {
        /// <summary>
        /// Первый <see cref="FamilySymbol"/> семейства с заданным именем, либо null.
        /// </summary>
        public static FamilySymbol FindSymbolByFamilyName(Document doc, string familyName)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(s => string.Equals(s.Family.Name, familyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
