using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DevelopexTest.Models
{
    public class EventBus
    {
        public static EventBus Instance { get { return instance ?? (instance = new EventBus()); } }

        public void Register(object listener)
        {
            if (!listeners.Any(l => l.Listener == listener))
                listeners.Add(new EventListenerWrapper(listener));
        }

        public void Unregister(object listener)
        {
            listeners.RemoveAll(l => l.Listener == listener);
        }

        public void PostEvent(object e)
        {
            listeners.Where(l => l.EventType == e.GetType()).ToList().ForEach(l => l.PostEvent(e));
        }

        private static EventBus instance;

        private EventBus() { }

        private List<EventListenerWrapper> listeners = new List<EventListenerWrapper>();

        private class EventListenerWrapper : IEquatable<EventListenerWrapper>
        {
            public bool Equals(EventListenerWrapper other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(method, other.method) && Equals(Listener, other.Listener) && Equals(EventType, other.EventType);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((EventListenerWrapper) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (method != null ? method.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Listener != null ? Listener.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (EventType != null ? EventType.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public object Listener { get; private set; }
            public Type EventType { get; private set; }

            private MethodBase method;

            public EventListenerWrapper(object listener)
            {
                Listener = listener;

                Type type = listener.GetType();

                method = type.GetMethod("OnEvent");
                if (method == null)
                    throw new ArgumentException("Class " + type.Name + " does not containt method OnEvent");

                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length != 1)
                    throw new ArgumentException("Method OnEvent of class " + type.Name + " have invalid number of parameters (should be one)");

                EventType = parameters[0].ParameterType;
            }

            public void PostEvent(object e)
            {
                method.Invoke(Listener, new[] { e });
            }
        }
    }
}