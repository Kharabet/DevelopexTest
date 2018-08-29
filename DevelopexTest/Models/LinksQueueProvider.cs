using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DevelopexTest.EventBus;
using DevelopexTest.EventBus.Events;

namespace DevelopexTest.Models
{
    //Provides link collecting and enqueue them by Breadth-First Search. 
    //The innner url structure is similar to a tree We have root link(root nodes) 
    //which contains inner links(first-level nodes). Each of them contains inner
    //links too (second-level nodes) and so on. 
    //
    //_linkQueue is BlockingCollection based on SimplePriorityQueue which allows as 
    //take from collection items with highest piority (top level nodes first).
    //It's not classic Breadth-First Search but we have no Idle Threads. They take first available  
    //node with highest priority. If we have heavy site(on first level) which process for a while 
    //and collect inner links(second level nodes) later than other threads(suppose, they already collect 5-th level links),
    //they will be collect immediately (cause they will have higher priority)
    public class LinksQueueProvider
    {
        private int _linkCounter = 0;

        private readonly int _maxUrlsCount;

        private object _lock = new object();

        private List<WebPageLink> _linkList = new List<WebPageLink>();

        private BlockingCollection<KeyValuePair<int, string>> _linkQueue;

        private static readonly List<SubscriptionToken> _subscriptions = new List<SubscriptionToken>();

        private class WebPageLink
        {
            public int TraverseLevel { get; private set; }
            public bool isVisited { get; private set; }
            public string Url { get; private set; }

            public WebPageLink(string url, int traverseLevel, bool isVisited = false)
            {
                Url = url;
                TraverseLevel = traverseLevel;
                isVisited = isVisited;
            }
        }

        public LinksQueueProvider(string rootLink, int maxUrlsCount)
        {
            _maxUrlsCount = maxUrlsCount;
            _subscriptions.Add(EventBus.EventBus.Instance.Subscribe<LinksFindedEvent>(OnLinkFinded));

            var priorityQueue = new SimplePriorityQueue<int, string>(maxUrlsCount);
            _linkQueue = new BlockingCollection<KeyValuePair<int, string>>(priorityQueue);

            AddLink(rootLink, 0);
        }

        private KeyValuePair<int, string> CreateQueueItem(WebPageLink wpLink)
        {
            return new KeyValuePair<int, string>(wpLink.TraverseLevel, wpLink.Url);
        }

        private void OnLinkFinded(LinksFindedEvent eventItem)
        {
            if (_linkQueue.IsAddingCompleted)
            {
                return;
            }

            lock (_lock)
            {
                if (_linkCounter >= _maxUrlsCount)
                {
                    _linkQueue.CompleteAdding();
                    EventBus.EventBus.Instance.Unsubscribe(_subscriptions.Find(x =>
                        x.EventItemType == eventItem.GetType()));
                    return;
                }
                var delta = _maxUrlsCount - _linkCounter;
                var prLink = _linkList.Find(x => x.Url == eventItem.ParentLink);
                if (prLink == null)
                {
                    return;
                }
                var parentTraverseLevel = prLink.TraverseLevel;
                WebPageLink parentLink = new WebPageLink(eventItem.ParentLink, parentTraverseLevel);
                foreach (var innerLink in eventItem.InnerLinks.Take(delta))
                {
                    if (!_linkList.Any(x => x.Url == innerLink))
                    {
                        AddLink(innerLink, parentLink.TraverseLevel + 1);
                    }
                }
            }
        }

        private void AddLink(string link, int traverseLevel)
        {
            if (_linkQueue.IsAddingCompleted)
            {
                return;
            }
            var webPageLink = new WebPageLink(link, traverseLevel);
            _linkList.Add(webPageLink);
            _linkQueue.Add(CreateQueueItem(webPageLink));
            _linkCounter++;
        }

        public BlockingCollection<KeyValuePair<int, string>> LinkQueue
        {
            get { return _linkQueue; }

            set { }
        }
    }
}