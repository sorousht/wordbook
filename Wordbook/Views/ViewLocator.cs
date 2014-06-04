namespace Wordbook.Views
{
    public class ViewLocator
    {
        static ViewLocator()
        {
            ViewLocator.WordsView = new Words();
            ViewLocator.EditView = new Edit();
            ViewLocator.SettingsView = new Settings();
            ViewLocator.WordsFilterView = new WordsFilter();
        }

        public static readonly Words WordsView;
        public static readonly Edit EditView;
        public static readonly Settings SettingsView;
        public static readonly WordsFilter WordsFilterView;
    }
}