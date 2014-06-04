namespace Wordbook.Services
{
    public class FlyoutOptions
    {
        private FlyoutOptions(bool isOpen)
        {
            this.IsOpen = isOpen;
        }

        public static FlyoutOptions Open()
        {
            return new FlyoutOptions(true);
        }

        public static FlyoutOptions Close()
        {
            return new FlyoutOptions(false);
        }

        public bool IsOpen { get; set; }
    }
}