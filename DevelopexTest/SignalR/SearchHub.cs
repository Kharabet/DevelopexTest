using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DevelopexTest.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevelopexTest.SignalR
{
    [HubName("searchHub")]
    public class SearchHub : Hub, IEquatable<SearchHub>
    {
        public bool Equals(SearchHub other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_guidDictionary, other._guidDictionary);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SearchHub) obj);
        }

        public override int GetHashCode()
        {
            return (_guidDictionary != null ? _guidDictionary.GetHashCode() : 0);
        }

        //private readonly ProgressHolder _progressHolder;
        private readonly ConcurrentDictionary<string, string> _guidDictionary = new ConcurrentDictionary<string, string>();


        public void Hello(string guid, string url, string status, string errorMessage = null)
        {
            if (!_guidDictionary.ContainsKey(guid)) 
                return;
            
            if (string.IsNullOrEmpty(errorMessage))
            {
                Clients.Group(guid).addChatMessage(url, status);
            }
            else
            {
                Clients.Group(guid).addChatMessage(url, status, errorMessage);
            }
        }

        public void OnEvent(OnProgressChangedEvent e)
        {
            if (string.IsNullOrEmpty(e.ErrorMessage))
            {
                Hello(e.Guid, e.Url, e.Status.ToString());
            }
            else
            {
                Hello(e.Guid, e.Url, e.Status.ToString(), e.ErrorMessage);
            }
        }

        public override Task OnConnected()
        {
            EventBus.Instance.Register(this);

            string guid = Context.QueryString["guid"];
            _guidDictionary.TryAdd(guid, Context.ConnectionId);
            Groups.Add(Context.ConnectionId, guid);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            EventBus.Instance.Unregister(this);
            return base.OnDisconnected(stopCalled);
        }
    }

    public class OnProgressChangedEvent
    {
        public string Url { get; private set; }
        public int Status { get; private set; }
        public string Guid { get; private set; }
        public string ErrorMessage { get; private set; }

        public OnProgressChangedEvent(int status, string guid, string url, string errorMessage = null)
        {
            Status = status;
            Guid = guid;
            Url = url;
            ErrorMessage = errorMessage;
        }
    }
}