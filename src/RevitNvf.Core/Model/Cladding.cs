using System;

namespace RevitNvf.Core.Model
{
    /// <summary>
    /// Параметры облицовки (мм): размер элемента, ширина шва, режим привязки и
    /// признак добора краевых (доборных) элементов. Без зависимостей от Revit.
    /// </summary>
    public sealed class Cladding
    {
        public double PanelWidth { get; }
        public double PanelHeight { get; }
        public double JointWidth { get; }
        public PanelStartMode StartMode { get; }

        /// <summary>
        /// Добавлять ли доборные (краевые) элементы из остатка после полных панелей.
        /// Учитывается только при <see cref="PanelStartMode.Corner"/>.
        /// </summary>
        public bool IncludeEdgePanels { get; }

        public Cladding(
            double panelWidth,
            double panelHeight,
            double jointWidth = 0,
            PanelStartMode startMode = PanelStartMode.Corner,
            bool includeEdgePanels = false)
        {
            if (panelWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(panelWidth), "Ширина элемента облицовки должна быть положительной (мм).");
            if (panelHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(panelHeight), "Высота элемента облицовки должна быть положительной (мм).");
            if (jointWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(jointWidth), "Ширина шва не может быть отрицательной (мм).");

            PanelWidth = panelWidth;
            PanelHeight = panelHeight;
            JointWidth = jointWidth;
            StartMode = startMode;
            IncludeEdgePanels = includeEdgePanels;
        }
    }
}
