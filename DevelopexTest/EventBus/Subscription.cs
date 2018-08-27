using System;
using DevelopexTest.EventBus.Events;

namespace DevelopexTest.EventBus
{
    internal class Subscription<TEventBase> : ISubscription where TEventBase : EventBase
    {
        public SubscriptionToken SubscriptionToken { get; set; }

        public Subscription(Action<TEventBase> action, SubscriptionToken token)
        {
            if (action == null)
            {
                throw new ArgumentNullException();
            }
            if (token == null)
            {
                throw new ArgumentNullException();
            }
            _action = action;
            SubscriptionToken = token;
        }

        public void Publish(EventBase eventItem)
        {
            if (!(eventItem is TEventBase))
                throw new ArgumentException("Event Item is not the correct type.");

            _action.Invoke(eventItem as TEventBase);
        }

        private readonly Action<TEventBase> _action;
    }
}