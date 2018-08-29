using System.Collections.Generic;
using System.Threading;
using DevelopexTest.EventBus.Events;

namespace DevelopexTest.Models
{   
    public class CancellationTokenProvider
    {
        public static Dictionary<string, CancellationTokenSource> GuidCtsDictionary = new Dictionary<string, CancellationTokenSource>();

        public void Add(string guid)
        {
            var cts = new CancellationTokenSource();
            if (GuidCtsDictionary.ContainsKey(guid))
            {
                GuidCtsDictionary[guid] = cts;
            }
            else
            {
                GuidCtsDictionary.Add(guid, cts);
            }
        }

        public CancellationToken GetToken(string guid)
        {
            if (!GuidCtsDictionary.ContainsKey(guid))
            {
                EventBus.EventBus.Instance.Publish(new ApplicationErrorEvent(guid, "Could not cancel request!"));
                return new CancellationToken();
            }
            return GuidCtsDictionary[guid].Token;
        }
    }
}