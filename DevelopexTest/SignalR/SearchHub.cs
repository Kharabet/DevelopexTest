using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevelopexTest.SignalR
{
    [HubName("searchHub")]
    public class SearchHub : Hub
    {
        private readonly ProgressHolder _progressHolder;

        public SearchHub(ProgressHolder progressHolder)
        {
            _progressHolder = progressHolder;
        }

        public void Hello(string guid, string message)
        {
            Clients.Group(guid).addChatMessage("asfsadf");
        }


        public override Task OnConnected()
        {
            string guid = Context.QueryString["guid"];
            Groups.Add(guid, Context.ConnectionId);
            return base.OnConnected();
        }
    }
}