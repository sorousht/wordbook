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

        public static void OpenFlyout(Routes route)
        {
            EventAggrigator.Publish(Interactions.Navigate, new NavigateOptions(route)
            {
                Parameter = FlyoutOptions.Open()
            });
        }

        public static void CloseFlyout()
        {
            EventAggrigator.Publish(Interactions.Navigate, new NavigateOptions(Routes.None)
            {
                Parameter = FlyoutOptions.Close()
            });
        }

        public static void Navigate(Routes route)
        {
            EventAggrigator.Publish(Interactions.Navigate, new NavigateOptions(route));
        }
    }

    public enum Interactions
    {
        Notify,
        Navigate,
    }
}