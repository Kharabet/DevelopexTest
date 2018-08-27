using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DevelopexTest.EventBus;
using DevelopexTest.EventBus.Events;
using DevelopexTest.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevelopexTest.SignalR
{
    [HubName("searchHub")]
    public class SearchHub : Hub
    {
        private ITestProgressHolder _progressHolder;
        private static readonly ConcurrentDictionary<string, string> _guidDictionary = new ConcurrentDictionary<string, string>();

        public SearchHub()
        {
            _progressHolder = new ProgressHolder(this);
        }

        public void OnAppError(ApplicationErrorEvent eventItem)
        {
            var guid = eventItem.Guid;
            if (!_guidDictionary.ContainsKey(guid))
                return;
            Clients.Client(_guidDictionary[guid]).appError(eventItem.ErrorMessage);
        }

        public void OnProgressChanged(ProgressChangedEvent eventItem)
        {
            var guid = eventItem.Guid;
            var status = eventItem.Status;
            var url = eventItem.Url;
            var errorMessage = eventItem.ErrorMessage;
            
            if (!_guidDictionary.ContainsKey(guid)) 
                return;
            
            if (string.IsNullOrEmpty(errorMessage))
            {
                Clients.Client(_guidDictionary[guid]).progressChanged(url, status);
            }
            else
            {
                Clients.Client(_guidDictionary[guid]).progressChanged(url, status, errorMessage);
            }
        }

        public override Task OnConnected()
        {
            string guid = Context.QueryString["guid"];

            _progressHolder.Add(guid, typeof (ProgressChangedEvent));
            _progressHolder.Add(guid, typeof (ApplicationErrorEvent));
            _guidDictionary.TryAdd(guid, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var guid = _guidDictionary.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (string.IsNullOrEmpty(guid))
            {
                return base.OnDisconnected(stopCalled);
            }
            string connectionId;
            if (_progressHolder != null)
            {
                _progressHolder.Remove(guid);
            }

            _guidDictionary.TryRemove(guid, out connectionId);
            return base.OnDisconnected(stopCalled);
        }
    }

}