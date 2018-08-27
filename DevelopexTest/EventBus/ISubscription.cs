using DevelopexTest.EventBus.Events;

namespace DevelopexTest.EventBus
{
    interface ISubscription
    {
        SubscriptionToken SubscriptionToken { get; }
        void Publish(EventBase eventBase);
    }
}
