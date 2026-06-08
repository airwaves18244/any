using System;
using System.Collections.Generic;
using RevitNvf.Core.Geometry;
using RevitNvf.Core.Model;

namespace RevitNvf.Core.Layout
{
    /// <summary>
    /// Раскладка облицовки прямоугольной сеткой с учётом шва и режима привязки.
    /// При <see cref="Cladding.IncludeEdgePanels"/> (только для привязки от угла)
    /// добавляет доборные элементы из остатка по краям. Чистая функция,
    /// детерминирована и идемпотентна. Величины в мм. Без зависимостей от Revit.
    /// </summary>
    public static class PanelLayout
    {
        private const double Epsilon = 1e-6;

        private readonly struct Cell
        {
            public readonly double Start;
            public readonly double Size;
            public readonly bool IsEdge;

            public Cell(double start, double size, bool isEdge)
            {
                Start = start;
                Size = size;
                IsEdge = isEdge;
            }
        }

        public static IReadOnlyList<Panel> Generate(Substrate substrate, Cladding cladding)
        {
            if (substrate is null) throw new ArgumentNullException(nameof(substrate));
            if (cladding is null) throw new ArgumentNullException(nameof(cladding));

            IReadOnlyList<Cell> xCells = AxisCells(substrate.Width, cladding.PanelWidth, cladding.JointWidth, cladding);
            IReadOnlyList<Cell> yCells = AxisCells(substrate.Height, cladding.PanelHeight, cladding.JointWidth, cladding);

            var panels = new List<Panel>(xCells.Count * yCells.Count);
            foreach (Cell yc in yCells)
            {
                foreach (Cell xc in xCells)
                {
                    panels.Add(new Panel(
                        new Point2d(xc.Start, yc.Start),
                        xc.Size,
                        yc.Size,
                        isEdge: xc.IsEdge || yc.IsEdge));
                }
            }

            return panels;
        }

        /// <summary>
        /// Ячейки вдоль оси: полные элементы с шагом (size+joint), затем — при
        /// раскладке от угла с добором — доборный элемент из остатка.
        /// </summary>
        private static IReadOnlyList<Cell> AxisCells(double length, double size, double joint, Cladding cladding)
        {
            double pitch = size + joint;
            int fullCount = (int)Math.Floor((length + joint + Epsilon) / pitch);
            if (fullCount < 0) fullCount = 0;
            if (fullCount == 0)
            {
                return Array.Empty<Cell>();
            }

            double firstStart = cladding.StartMode == PanelStartMode.Centered
                ? (length - (fullCount * size + (fullCount - 1) * joint)) / 2.0
                : 0.0;

            var cells = new List<Cell>(fullCount + 1);
            for (int i = 0; i < fullCount; i++)
            {
                cells.Add(new Cell(firstStart + i * pitch, size, isEdge: false));
            }

            // Доборный элемент — только при привязке от угла и явном включении.
            if (cladding.IncludeEdgePanels && cladding.StartMode == PanelStartMode.Corner)
            {
                double lastFullEnd = (fullCount - 1) * pitch + size;
                double edgeStart = lastFullEnd + joint;
                double edgeWidth = length - edgeStart;
                if (edgeWidth > Epsilon)
                {
                    cells.Add(new Cell(edgeStart, edgeWidth, isEdge: true));
                }
            }

            return cells;
        }
    }
}
