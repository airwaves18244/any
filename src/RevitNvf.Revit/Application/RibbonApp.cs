using System;
using System.Reflection;
using Autodesk.Revit.UI;
using RevitNvf.Revit.Commands;
using RevitNvf.Revit.Panels;
using RevitNvf.UI;
using RevitNvf.UI.Views;

namespace RevitNvf.Revit
{
    /// <summary>
    /// Точка входа плагина: регистрирует панель параметров и строит вкладку «НВФ»
    /// на ленте Revit. Зарегистрирована в build/RevitNvf.addin как Type="Application".
    /// </summary>
    public class RibbonApp : IExternalApplication
    {
        private const string TabName = "НВФ";

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                RegisterDockablePane(application);
                CreateRibbon(application);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("RevitNvf", "Не удалось инициализировать плагин:\n" + ex.Message);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

        private static void RegisterDockablePane(UIControlledApplication application)
        {
            var view = new FacadeParametersView { DataContext = FacadeParametersStore.Current };
            application.RegisterDockablePane(
                PaneIds.FacadeParameters, "НВФ — параметры", new FacadeParametersPaneProvider(view));
        }

        private static void CreateRibbon(UIControlledApplication application)
        {
            // Вкладка может уже существовать (например, при повторной загрузке).
            try
            {
                application.CreateRibbonTab(TabName);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
            }

            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            RibbonPanel layoutPanel = application.CreateRibbonPanel(TabName, "Раскладка");
            layoutPanel.AddItem(Button(assemblyPath, "RevitNvf_PlaceBrackets", "Кронштейны",
                typeof(PlaceBracketsCommand), "Раскладка кронштейнов сеткой на выбранной плоской грани."));
            layoutPanel.AddItem(Button(assemblyPath, "RevitNvf_PlaceGuides", "Направляющие",
                typeof(PlaceGuidesCommand), "Вертикальные направляющие по линиям кронштейнов."));
            layoutPanel.AddItem(Button(assemblyPath, "RevitNvf_PlaceCladding", "Облицовка",
                typeof(PlaceCladdingCommand), "Раскладка облицовки с учётом шва и доборных элементов."));

            RibbonPanel dataPanel = application.CreateRibbonPanel(TabName, "Данные");
            dataPanel.AddItem(Button(assemblyPath, "RevitNvf_LayerStackup", "Состав пирога",
                typeof(ShowLayerStackupCommand), "Слои пирога НВФ: толщины и смещения."));
            dataPanel.AddItem(Button(assemblyPath, "RevitNvf_Takeoff", "Ведомость",
                typeof(ShowTakeoffCommand), "Сводная ведомость материалов по выбранной грани."));
            dataPanel.AddSeparator();
            dataPanel.AddItem(Button(assemblyPath, "RevitNvf_Parameters", "Параметры",
                typeof(ShowParametersPaneCommand), "Панель параметров фасадной системы и пресеты."));
            dataPanel.AddItem(Button(assemblyPath, "RevitNvf_About", "О плагине",
                typeof(ShowAboutCommand), "Информация о плагине RevitNvf."));
        }

        private static PushButtonData Button(string assemblyPath, string name, string text, Type command, string tooltip)
        {
            return new PushButtonData(name, text, assemblyPath, command.FullName)
            {
                ToolTip = tooltip
            };
        }
    }
}
