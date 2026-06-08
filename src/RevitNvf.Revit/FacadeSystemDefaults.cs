using RevitNvf.Core.Model;

namespace RevitNvf.Revit
{
    /// <summary>
    /// Временные значения по умолчанию для команд раскладки. Будут заменены вводом
    /// из WPF-панели параметров системы на шаге 8 (см. docs/ROADMAP.md).
    /// </summary>
    internal static class FacadeSystemDefaults
    {
        /// <summary>Имя семейства кронштейна, ожидаемого в проекте (face-based).</summary>
        public const string BracketFamilyName = "NVF_Bracket_Load";

        public static FacadeSystem Create() => new FacadeSystem(
            bracketStepX: 1000,
            bracketStepY: 1000,
            bracketEdgeOffsetX: 100,
            bracketEdgeOffsetY: 100);
    }
}
