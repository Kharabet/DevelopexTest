using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DevelopexTest.EventBus;
using DevelopexTest.EventBus.Events;
using DevelopexTest.Models;

namespace DevelopexTest.SignalR
{
    public class ProgressHolder: ITestProgressHolder
    {
        private SearchHub _hub;
        private static ConcurrentDictionary<string, List<Type>> _userGuidToEventDictionary = new ConcurrentDictionary<string, List<Type>>();
        private static List<SubscriptionToken> eventsList = new List<SubscriptionToken>();

        public ProgressHolder(SearchHub hub)
        {
            _hub = hub;
        }

        public void Add(string guid, Type eventType)
        {
            if (eventsList.Count(x => x.EventItemType == eventType) == 0)
            {
                Subscribe(eventType);
            }

            if (_userGuidToEventDictionary.ContainsKey(guid))
            {
                _userGuidToEventDictionary[guid].Add(eventType);
            }
            else
            {
                _userGuidToEventDictionary.TryAdd(guid, new List<Type>(){eventType});
            }
        }

        public void Subscribe(Type eventType)
        {
            if (eventType == typeof(ProgressChangedEvent))
            {
                var token = EventBus.EventBus.Instance.Subscribe<ProgressChangedEvent>(OnProgressChangedEvent);
                eventsList.Add(token);
            }
            else if (eventType == typeof(ApplicationErrorEvent))
            {
                var token = EventBus.EventBus.Instance.Subscribe<ApplicationErrorEvent>(OnAppErrorEvent);
                eventsList.Add(token);
            }
        }

        private void OnProgressChangedEvent(ProgressChangedEvent eventItem)
        {
            List<Type> eventTypeList;
            if (_userGuidToEventDictionary.TryGetValue(eventItem.Guid, out eventTypeList) && eventTypeList.Contains(eventItem.GetType())) 
                _hub.OnProgressChanged(eventItem);
        }

        private void OnAppErrorEvent(ApplicationErrorEvent eventItem)
        {
            List<Type> eventTypeList;
            if (_userGuidToEventDictionary.TryGetValue(eventItem.Guid, out eventTypeList) && eventTypeList.Contains(eventItem.GetType()))
                _hub.OnAppError(eventItem);
        }
        public void Unsubscribe(Type eventType)
        {
            EventBus.EventBus.Instance.Unsubscribe(eventsList.Find(x => x.EventItemType == eventType));
        }

        public void Remove(string guid)
        {
            List<Type> eventTypeList;
            _userGuidToEventDictionary.TryRemove(guid, out eventTypeList);
            foreach (var eventType in eventTypeList)
            {
                if (_userGuidToEventDictionary.Values.Count(x => x.Contains(eventType)) == 0)
                {
                    Unsubscribe(eventType);
                }
            }
        }
    }
}