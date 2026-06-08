using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitNvf.Revit.Panels;

namespace RevitNvf.Revit.Commands
{
    /// <summary>Шаг 8: показывает dockable-панель параметров фасадной системы.</summary>
    [Transaction(TransactionMode.Manual)]
    public class ShowParametersPaneCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                DockablePane pane = commandData.Application.GetDockablePane(PaneIds.FacadeParameters);
                pane.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
