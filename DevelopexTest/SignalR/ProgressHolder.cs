using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DevelopexTest.EventBus;
using DevelopexTest.EventBus.Events;
using DevelopexTest.Models;
using DevelopexTest.SignalR.Hubs;
using Microsoft.AspNet.SignalR;

namespace DevelopexTest.SignalR
{
    public class ProgressHolder
    {
        private SearchHub _hub;
        private static ConcurrentDictionary<string, List<Type>> _userGuidToEventDictionary = new ConcurrentDictionary<string, List<Type>>();
        private static List<SubscriptionToken> eventsList = new List<SubscriptionToken>();
        private object _lockObj = new object();
        public ProgressHolder(SearchHub hub)
        {
            _hub = hub;
        }

        public void Add(string guid, Type eventType)
        {
            lock (_lockObj)
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
                    _userGuidToEventDictionary.TryAdd(guid, new List<Type>() { eventType });
                }
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
            if (!_userGuidToEventDictionary.TryGetValue(eventItem.Guid, out eventTypeList) ||
                !eventTypeList.Contains(eventItem.GetType()))
                return;

            var guid = eventItem.Guid;
            var status = eventItem.Status;
            var url = eventItem.Url;
            var errorMessage = eventItem.ErrorMessage;

            if (!SearchHub.GuidDictionary.ContainsKey(guid))
                return;
            var connectionId = SearchHub.GuidDictionary[guid];
            if (string.IsNullOrEmpty(errorMessage))
            {
                GlobalHost.ConnectionManager.GetHubContext<SearchHub>().Clients.Client(connectionId).progressChanged(url, status);
            }
            else
            {
                GlobalHost.ConnectionManager.GetHubContext<SearchHub>().Clients.Client(connectionId).progressChanged(url, status, errorMessage);
            }
        }

        private void OnAppErrorEvent(ApplicationErrorEvent eventItem)
        {
            List<Type> eventTypeList;
            if (!_userGuidToEventDictionary.TryGetValue(eventItem.Guid, out eventTypeList) ||
                !eventTypeList.Contains(eventItem.GetType()))
                return;

            var guid = eventItem.Guid;
            var connectionId = SearchHub.GuidDictionary[guid];
            if (!SearchHub.GuidDictionary.ContainsKey(guid))
                return;
            GlobalHost.ConnectionManager.GetHubContext<SearchHub>().Clients.Client(connectionId).appError(eventItem.ErrorMessage);
        }

        public void Unsubscribe(Type eventType)
        {
            var token = eventsList.Find(x => x.EventItemType == eventType);
            EventBus.EventBus.Instance.Unsubscribe(token);
            eventsList.Remove(token);
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