using System.Collections.Generic;

namespace DevelopexTest.EventBus.Events
{
    public class LinksFindedEvent : EventBase
    {
        public string ParentLink { get; set; }
        public List<string> InnerLinks { get; set; }

        public LinksFindedEvent(string parentLink, List<string> innerLinks)
        {
            ParentLink = parentLink;
            InnerLinks = innerLinks;
        }
    }
}