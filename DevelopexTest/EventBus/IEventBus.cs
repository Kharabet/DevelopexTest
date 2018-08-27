using System;
using DevelopexTest.EventBus.Events;

namespace DevelopexTest.EventBus
{

    public interface IEventBus
    {
        SubscriptionToken Subscribe<TEventBase>(Action<TEventBase> action) where TEventBase : EventBase;

        void Unsubscribe(SubscriptionToken token);

        void Publish<TEventBase>(TEventBase eventItem) where TEventBase : EventBase;
    }
}

