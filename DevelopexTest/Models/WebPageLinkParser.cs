using DevelopexTest.EventBus.Events;

namespace DevelopexTest.Models
{
    public class WebPageLinkParser
    {
        private LinksQueueProvider _linkProvider;
        private ProducerConsumer _pcQueue;
        private SearchRequest _request;

        public void Parse(SearchRequest request)
        {
            _linkProvider = new LinksQueueProvider(request.StartUrl, request.MaxUrlsCount);
            _pcQueue = new ProducerConsumer(request.MaxThreadsCount);
            _request = request;
            foreach (var item in _linkProvider.LinkQueue.GetConsumingEnumerable())
            {
                if (_linkProvider.LinkQueue.IsCompleted)
                {
                    _pcQueue.Dispose();
                    EventBus.EventBus.Instance.Publish(new ApplicationErrorEvent(_request.UserGuid, "Inner link parsing failed!"));
                }
                _pcQueue.EnqueueTask(() => ProcessWebPage(item.Value, _request.TextToSearch, _request.UserGuid));
            }
        }

        private void ProcessWebPage(string link, string textToSearch, string userGuid)
        {
            var webPage = new WebPage(link, textToSearch, userGuid);
            webPage.Scan();
        }
    }
}