using System.Windows.Media.Animation;

namespace Wordbook.Controllers
{
    public static class ControllerLocator
    {
        static ControllerLocator()
        {
            ControllerLocator.MainController = new MainController();
            ControllerLocator.WordsController = new WordsController();
            ControllerLocator.SettingsController = new SettingsController();
        }

        public static readonly WordsController WordsController;
        public static readonly MainController MainController;
        public static readonly SettingsController SettingsController;
    }
}