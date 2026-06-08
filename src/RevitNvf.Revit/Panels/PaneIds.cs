using System;
using Autodesk.Revit.UI;

namespace RevitNvf.Revit.Panels
{
    internal static class PaneIds
    {
        /// <summary>Идентификатор dockable-панели параметров фасадной системы.</summary>
        public static readonly DockablePaneId FacadeParameters =
            new DockablePaneId(new Guid("b2021306-be5f-4588-90e0-853cd0d1d50c"));
    }
}
