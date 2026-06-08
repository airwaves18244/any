using Autodesk.Revit.UI;
using RevitNvf.UI.Views;

namespace RevitNvf.Revit.Panels
{
    /// <summary>
    /// Поставщик содержимого dockable-панели: размещает WPF-вид параметров фасада
    /// (из RevitNvf.UI) в панели Revit, пристыкованной справа.
    /// </summary>
    internal sealed class FacadeParametersPaneProvider : IDockablePaneProvider
    {
        private readonly FacadeParametersView _view;

        public FacadeParametersPaneProvider(FacadeParametersView view)
        {
            _view = view;
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = _view;
            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Right
            };
        }
    }
}
