using System;
using System.Reflection;
using Autodesk.Revit.UI;
using RevitNvf.Revit.Commands;

namespace RevitNvf.Revit
{
    /// <summary>
    /// Точка входа плагина: создаёт вкладку «НВФ» на ленте Revit и кнопки команд.
    /// Зарегистрирована в build/RevitNvf.addin как Type="Application".
    /// </summary>
    public class RibbonApp : IExternalApplication
    {
        private const string TabName = "НВФ";
        private const string PanelName = "Фасад";

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                CreateRibbon(application);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("RevitNvf", "Не удалось инициализировать ленту:\n" + ex.Message);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

        private static void CreateRibbon(UIControlledApplication application)
        {
            // Вкладка может уже существовать (например, при повторной загрузке) — не пересоздаём.
            try
            {
                application.CreateRibbonTab(TabName);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
            }

            RibbonPanel panel = application.CreateRibbonPanel(TabName, PanelName);

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            var aboutButton = new PushButtonData(
                name: "RevitNvf_About",
                text: "О плагине",
                assemblyName: assemblyPath,
                className: typeof(ShowAboutCommand).FullName)
            {
                ToolTip = "Информация о плагине RevitNvf (навесные вентилируемые фасады)."
            };

            panel.AddItem(aboutButton);
        }
    }
}
