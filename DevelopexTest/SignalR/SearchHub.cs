using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevelopexTest.SignalR
{
    [HubName("searchHub")]
    public class SearchHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}