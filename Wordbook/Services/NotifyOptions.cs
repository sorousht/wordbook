namespace Wordbook.Services
{
    public class NotifyOptions
    {
        public NotifyOptions(States state, object parameter)
        {
            State = state;
            Parameter = parameter;
        }

        public object Parameter { get; set; }
        public States State { get; set; }
    }
}