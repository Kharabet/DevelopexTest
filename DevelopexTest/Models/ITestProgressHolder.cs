using System;

namespace DevelopexTest.Models
{
    interface ITestProgressHolder
    {
        void Add(string guid, Type eventType);

        void Subscribe(Type eventType);

        void Unsubscribe(Type eventType);

        void Remove(string guid);

    }
}
