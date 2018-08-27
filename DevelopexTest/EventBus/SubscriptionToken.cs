using System;

namespace DevelopexTest.EventBus
{
    public class SubscriptionToken
    {
        internal SubscriptionToken(Type eventItemType)
        {
            Token = Guid.NewGuid();
            EventItemType = eventItemType;
        }

        public Guid Token { get; private set; }

        public Type EventItemType { get; private set; }
    }
}