using System;
using System.Collections.Generic;
using System.Linq;
using DevelopexTest.EventBus.Events;

namespace DevelopexTest.EventBus
{
    public class EventBus : IEventBus
    {
        private static readonly object SubscriptionsLock = new object();

        private readonly Dictionary<Type, List<ISubscription>> _subscriptions;

        private static readonly Lazy<EventBus> _instance = new Lazy<EventBus>(() => new EventBus());
        public static EventBus Instance
        {
            get { return _instance.Value; }
        }

        private EventBus()
        {
            _subscriptions = new Dictionary<Type, List<ISubscription>>();
        }

        public SubscriptionToken Subscribe<TEventBase>(Action<TEventBase> action) where TEventBase : EventBase
        {
            if (action == null)
                throw new ArgumentNullException();

            lock (SubscriptionsLock)
            {
                if (!_subscriptions.ContainsKey(typeof(TEventBase)))
                    _subscriptions.Add(typeof(TEventBase), new List<ISubscription>());

                var token = new SubscriptionToken(typeof(TEventBase));
                _subscriptions[typeof(TEventBase)].Add(new Subscription<TEventBase>(action, token));
                return token;
            }
        }

        public void Unsubscribe(SubscriptionToken token)
        {
            if (token == null)
                throw new ArgumentNullException();

            lock (SubscriptionsLock)
            {
                if (_subscriptions.ContainsKey(token.EventItemType))
                {
                    var allSubscriptions = _subscriptions[token.EventItemType];
                    var subscriptionToRemove = allSubscriptions.FirstOrDefault(x => x.SubscriptionToken.Token == token.Token);
                    if (subscriptionToRemove != null)
                        _subscriptions[token.EventItemType].Remove(subscriptionToRemove);
                }
            }
        }

        public void Publish<TEventBase>(TEventBase eventItem) where TEventBase : EventBase
        {
            if (eventItem == null)
                throw new ArgumentNullException();

            List<ISubscription> allSubscriptions = new List<ISubscription>();
            lock (SubscriptionsLock)
            {
                if (_subscriptions.ContainsKey(typeof(TEventBase)))
                    allSubscriptions = _subscriptions[typeof(TEventBase)];
            }

            for (var index = 0; index < allSubscriptions.Count; index++)
            {
                var subscription = allSubscriptions[index];
                try
                {
                    subscription.Publish(eventItem);
                }
                catch (Exception e )
                {
                    throw e;
                }
            }
        }
    }
}