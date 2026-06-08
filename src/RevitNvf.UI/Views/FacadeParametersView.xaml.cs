using System.Windows.Controls;

namespace RevitNvf.UI.Views
{
    /// <summary>
    /// WPF-панель параметров фасадной системы. DataContext — общий
    /// <see cref="ViewModels.FacadeParametersViewModel"/> из FacadeParametersStore.
    /// </summary>
    public partial class FacadeParametersView : UserControl
    {
        public FacadeParametersView()
        {
            InitializeComponent();
        }
    }
}
