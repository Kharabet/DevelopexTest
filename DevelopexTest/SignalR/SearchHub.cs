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
    public class SearchHub : Hub
    {
        //private readonly ProgressHolder _progressHolder;
        private readonly ConcurrentDictionary<string, string> _guidDictionary = new ConcurrentDictionary<string, string>();

        //public SearchHub(ProgressHolder progressHolder)
        //{
        //    _progressHolder = progressHolder;
        //}

        public void Hello(string guid, string status)
        {
            Clients.Group(guid).addChatMessage(status);
        }

        public void OnEvent(OnProgressChangedEvent e)
        {
            Hello(e.Guid, string.Format("{0},{1}", e.Status.ToString(), e.Url));
        }

        public override Task OnConnected()
        {
            EventBus.Instance.Register(this);

            string guid = Context.QueryString["guid"];
            _guidDictionary.TryAdd(guid, Context.ConnectionId);
            Groups.Add(Context.ConnectionId, guid);
            return base.OnConnected();
        }
    }

    public class OnProgressChangedEvent
    {
        public string Url { get; private set; }
        public int Status { get; private set; }
        public string Guid { get; private set; }

        public OnProgressChangedEvent(int status, string guid, string url)
        {
            Status = status;
            Guid = guid;
            Url = url;
        }
    }
}