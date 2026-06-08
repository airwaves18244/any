using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitNvf.Revit.Commands
{
    /// <summary>
    /// Hello-World команда (шаг 2): показывает информационный TaskDialog.
    /// Модель не меняет, поэтому транзакция не открывается (режим Manual).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ShowAboutCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dialog = new TaskDialog("RevitNvf")
            {
                MainInstruction = "Плагин НВФ загружен",
                MainContent =
                    "RevitNvf — моделирование навесных вентилируемых фасадов.\n" +
                    "Шаг 2 (Hello-World): вкладка на ленте и команда работают.\n" +
                    "Раскладка подоблицовочной конструкции появится на следующих шагах.",
                CommonButtons = TaskDialogCommonButtons.Close
            };
            dialog.Show();

            return Result.Succeeded;
        }
    }
}
