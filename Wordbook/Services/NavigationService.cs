namespace Wordbook.Services
{
    public class NavigationService
    {
        static NavigationService()
        {
            NavigationService.Current = AppPage.Edit;
        }

        public static AppPage Current { get; private set; }

        public static void Navigate(AppPage page)
        {
            NavigationService.Current = page;
        }
    }

    public enum AppPage
    {
        List,
        Edit,
    }
}