namespace RevitNvf.Core.Model
{
    /// <summary>Способ привязки раскладки облицовки по оси.</summary>
    public enum PanelStartMode
    {
        /// <summary>От угла (первый элемент в нуле оси).</summary>
        Corner,

        /// <summary>Центрирование: полные элементы выровнены по центру оси.</summary>
        Centered
    }
}
