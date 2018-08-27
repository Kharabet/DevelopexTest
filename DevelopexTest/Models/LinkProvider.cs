using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using DevelopexTest.EventBus;
using DevelopexTest.EventBus.Events;

namespace DevelopexTest.Models
{
    public class LinkProvider
    {
        private readonly List<string> _links = new List<string>();
        private List<Tuple<string, string>> _edges = new List<Tuple<string, string>>();
        private BlockingCollection<KeyValuePair<int, string>> _queue;
        private int _linkCounter = 0;
        private readonly int _maxUrlsCount;
        private static readonly List<SubscriptionToken> _eventsList = new List<SubscriptionToken>();
        private List<WebPageLink> _linkList = new List<WebPageLink>();
        private Dictionary<string, int> _linkTotraversLevelDict = new Dictionary<string, int>(); 

        private class WebPageLink
        {
            public int TraverseLevel { get; set; }
            public bool isVisited { get; set; }
            public string Url { get; set; }

            public WebPageLink(string url, int traverseLevel, bool isVisited = false)
            {
                Url = url;
                TraverseLevel = traverseLevel;
                isVisited = isVisited;
            }
        }

        //private class TraverseLevelQueue
        //{
        //    public int TraverseLevel { get; set; }
        //    public BlockingCollection<string> queue = new BlockingCollection<string>(); 
        //    public TraverseLevelQueue(int traverseLevel)
        //    {
        //        TraverseLevel = traverseLevel;
        //    }
        //}

        public LinkProvider(string rootLink, int maxUrlsCount)
        {
            _linkList.Add(new WebPageLink(rootLink, 0));
            _linkTotraversLevelDict.Add(rootLink, 0);

            var priorityQueue = new SimplePriorityQueue<int, string>(maxUrlsCount);
            _queue = new BlockingCollection<KeyValuePair<int, string>>(priorityQueue);
            var item = new KeyValuePair<int, string>(0, rootLink);
            _queue.Add(item);

            _maxUrlsCount = maxUrlsCount;

            Interlocked.Increment(ref _linkCounter);
            _eventsList.Add(EventBus.EventBus.Instance.Subscribe<LinksFindedEvent>(OnLinkFinded));
        }

        //private void OnLinkVisited(string url)
        //{
        //    var link = linkList.FirstOrDefault(x => x.Url == url);
        //    if (link != null)
        //    {
        //        link.isVisited = true;
        //    }
        //}

        private KeyValuePair<int, string> CreateQueueItem(WebPageLink wpLink)
        {
            return new KeyValuePair<int, string>(wpLink.TraverseLevel, wpLink.Url);
        }

        private void OnLinkFinded(LinksFindedEvent eventItem)
        {
            if (_linkCounter >= _maxUrlsCount)
            {
                _queue.CompleteAdding();
                EventBus.EventBus.Instance.Unsubscribe(_eventsList.Find(x => x.EventItemType == eventItem.GetType()));
                return;
            }
            int traverseLevel = _linkTotraversLevelDict[eventItem.ParentLink];
            WebPageLink parentLink = new WebPageLink(eventItem.ParentLink, _linkList.First(x => x.Url == eventItem.ParentLink).TraverseLevel);
            foreach (var innerLink in eventItem.InnerLinks)
            {
                if (_linkList.FirstOrDefault(x => x.Url == innerLink) != null)
                {
                    continue;
                }
                if (_linkTotraversLevelDict.ContainsKey(innerLink))
                {
                    continue;
                }

                WebPageLink link = new WebPageLink(innerLink, parentLink.TraverseLevel + 1);
                _linkList.Add(link);
                //_queue.Add(CreateQueueItem(link));
                _queue.Add(new KeyValuePair<int, string>(traverseLevel, innerLink));

                Interlocked.Increment(ref _linkCounter);

                if (_linkCounter >= _maxUrlsCount)
                {
                    _queue.CompleteAdding();
                    EventBus.EventBus.Instance.Unsubscribe(_eventsList.Find(x => x.EventItemType == eventItem.GetType()));
                    break;
                }
            }
        }

        public BlockingCollection<KeyValuePair<int, string>> Queue
        {
            get { return _queue; }

            set { }
        }
    }
}