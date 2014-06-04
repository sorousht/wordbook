namespace Wordbook.Services
{
    public class NavigateOptions
    {
        public NavigateOptions(Routes route)
        {
            this.Route = route;
        }
        public Routes Route { get; set; }

        public object Parameter { get; set; }
    }
}