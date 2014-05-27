using System;

namespace Wordbook.Services
{
    public class InteractionService
    {
        public static void On(Interactions interaction, Action<object> action)
        {
            EventAggrigator.Subscribe(interaction, action);
        }

        public static void Notify(NotifyOptions options)
        {
            EventAggrigator.Publish(Interactions.Notify, options);
        }
    }

    public enum Interactions
    {
        Notify
    }
}