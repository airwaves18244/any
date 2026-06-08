using RevitNvf.UI.ViewModels;

namespace RevitNvf.UI
{
    /// <summary>
    /// Единая активная модель параметров фасада. Команды раскладки читают её,
    /// панель параметров — редактирует. По умолчанию — первый пресет.
    /// </summary>
    public static class FacadeParametersStore
    {
        private static FacadeParametersViewModel _current;

        public static FacadeParametersViewModel Current =>
            _current ?? (_current = new FacadeParametersViewModel());
    }
}
