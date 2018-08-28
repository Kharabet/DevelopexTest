using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopexTest.EventBus.Events;
using DevelopexTest.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevelopexTest.SignalR.Hubs
{
    [HubName("searchHub")]
    public class SearchHub : Hub
    {
        private ProgressHolder _progressHolder;
        public static readonly ConcurrentDictionary<string, string> GuidDictionary = new ConcurrentDictionary<string, string>();

        public SearchHub()
        {
            _progressHolder = new ProgressHolder(this);
        }

        public override Task OnConnected()
        {
            string guid = Context.QueryString["guid"];
            Debug.WriteLine("connect thread:");
            Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
            
            _progressHolder.Add(guid, typeof (ProgressChangedEvent));
            _progressHolder.Add(guid, typeof (ApplicationErrorEvent));
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
            Debug.WriteLine("disconnect thread:");
            Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
            if (_progressHolder != null)
            {
                _progressHolder.Remove(guid);
            }

            GuidDictionary.TryRemove(guid, out connectionId);
            return base.OnDisconnected(stopCalled);
        }
    }

}