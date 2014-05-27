using System;
using System.Collections.Generic;
using System.Linq;

namespace Wordbook.Services
{
    public static class EventAggrigator
    {
        private static readonly IList<KeyValuePair<object, Action<object>>> Subscribers =
            new List<KeyValuePair<object, Action<object>>>();

        public static void Subscribe(object name, Action<object> action)
        {
            EventAggrigator.Subscribers.Add(new KeyValuePair<object, Action<object>>(name, action));
        }

        public static void Publish(object name, object parameter)
        {
            foreach (var subscriber in Subscribers.Where(pair => pair.Key.Equals(name)).Where(subscriber => subscriber.Value != null))
            {
                subscriber.Value(parameter);
            }
        }
    }
}