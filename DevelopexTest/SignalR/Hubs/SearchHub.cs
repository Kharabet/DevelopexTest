using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using DevelopexTest.EventBus.Events;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevelopexTest.SignalR.Hubs
{
    //This hub pushes notifications about state changing or errors to client.
    [HubName("searchHub")]
    public class SearchHub : Hub
    {
        private SearchHubListener _searchHubListener;
        public static readonly ConcurrentDictionary<string, string> GuidDictionary = new ConcurrentDictionary<string, string>();

        public SearchHub()
        {
            _searchHubListener = new SearchHubListener(this);
        }

        public override Task OnConnected()
        {
            string guid = Context.QueryString["guid"];
            
            _searchHubListener.Add(guid, typeof (ProgressChangedEvent));
            _searchHubListener.Add(guid, typeof (ApplicationErrorEvent));
            GuidDictionary.TryAdd(guid, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var guid = GuidDictionary.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (string.IsNullOrEmpty(guid))
            {
                return base.OnDisconnected(stopCalled);
            }
            string connectionId;
            if (_searchHubListener != null)
            {
                _searchHubListener.Remove(guid);
            }

            GuidDictionary.TryRemove(guid, out connectionId);

            return base.OnDisconnected(stopCalled);
        }
    }

}