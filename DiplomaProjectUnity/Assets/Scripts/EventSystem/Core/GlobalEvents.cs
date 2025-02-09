using System;
using System.Collections.Generic;

namespace DiplomaProject.EventSystem.Core
{
    public static class GlobalEvents
    {
        private static readonly Dictionary<Type, List<Delegate>> _eventListeners = new();

        public static void AddListener<T>(Action<T> listener) where T : IEvent
        {
            if (!_eventListeners.ContainsKey(typeof(T)))
            {
                _eventListeners[typeof(T)] = new List<Delegate>();
            }

            _eventListeners[typeof(T)].Add(listener);
        }

        public static void RemoveListener<T>(Action<T> listener) where T : IEvent
        {
            if (_eventListeners.TryGetValue(typeof(T), out var listeners))
            {
                listeners.Remove(listener);
                if (listeners.Count == 0)
                {
                    _eventListeners.Remove(typeof(T));
                }
            }
        }

        public static void Publish<T>(T @event) where T : IEvent
        {
            if (_eventListeners.TryGetValue(typeof(T), out var listeners))
            {
                foreach (var listener in listeners)
                {
                    ((Action<T>)listener).Invoke(@event);
                }
            }
        }
    }
}