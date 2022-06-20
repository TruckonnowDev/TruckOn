using MDispatch.Service.Utils;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.Tasks
{
    public class TaskManager
    {
        private static IUtilsService _utils;
        public static bool isWorkTask = true;

        public static async void CommandToDo(string nameCommand, params object[] tasks)
        {
            ITask task = null;
            if (_utils is null)
                _utils = DependencyService.Get<IUtilsService>();

            await Task.Run(() => _utils.CheckNet());
            if (!App.isNetwork)
            {
                return;
            }
            await Task.Delay(1000);
            switch (nameCommand)
            {
                case "DashbordVehicle":
                    {
                        task = new TaskDashbordVechle();
                        break;
                    }
                case "SavePhoto":
                    {
                        task = new SavePhoto();
                        break;
                    }
                case "SaveInspactionDriver":
                    {
                        task = new SaveInspactionDriver();
                        break;
                    }
                case "SaveRecount":
                    {
                        task = new SaveRecount();
                        break;
                    }
                case "CheckTask":
                    {
                        task = new CheckTask();
                        break;
                    }
            }
            if (task != null)
            {
                System.Threading.Tasks.Task.Run(() => task.StartTask(tasks));
            }
        }
    }
}
