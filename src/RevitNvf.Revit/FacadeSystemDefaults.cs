namespace RevitNvf.Revit
{
    /// <summary>
    /// Константы адаптера, не зависящие от пользовательских параметров.
    /// Числовые параметры раскладки берутся из FacadeParametersStore (панель).
    /// </summary>
    internal static class FacadeSystemDefaults
    {
        /// <summary>Имя семейства кронштейна, ожидаемого в проекте (face-based).</summary>
        public const string BracketFamilyName = "NVF_Bracket_Load";
    }
}
