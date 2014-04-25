namespace Wordbook.Views
{
    public static class ViewLocator
    {
        static ViewLocator()
        {
            ViewLocator.Edit = new Edit();
            ViewLocator.List = new List();
        }
        public static List List { get; set; }

        public static Edit Edit { get; set; }
    }
}