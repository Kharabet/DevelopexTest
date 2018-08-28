using System.Threading;
using DevelopexTest.EventBus.Events;

namespace DevelopexTest.Models
{
    public class WebPageLinkParser
    {
        private LinksQueueProvider _linkProvider;
        private ProducerConsumer _pcQueue;
        private SearchRequest _request;

        public void Parse(SearchRequest request, CancellationToken token)
        {
            //create object which produces the link queue to process
            _linkProvider = new LinksQueueProvider(request.StartUrl, request.MaxUrlsCount);

            //create ProducerConsumer which enqueues tasks with specific actions and cancellation tokens for N consumers(threads)
            _pcQueue = new ProducerConsumer(request.MaxThreadsCount);
            
            _request = request;
            foreach (var item in _linkProvider.LinkQueue.GetConsumingEnumerable())
            {
                //stop parsing
                if (token.IsCancellationRequested)
                {
                    _linkProvider.LinkQueue.CompleteAdding();
                    _pcQueue.Dispose();
                    return;
                }
                if (_linkProvider.LinkQueue.IsAddingCompleted)
                {
                    _pcQueue.Dispose();
                    EventBus.EventBus.Instance.Publish(new ApplicationErrorEvent(_request.UserGuid, "Inner link parsing failed!"));
                }

                //enqueue next link from queue 
                _pcQueue.EnqueueTask(() => ProcessWebPage(item.Value, _request.TextToSearch, _request.UserGuid), token);
            }
        }

        private void ProcessWebPage(string link, string textToSearch, string userGuid)
        {
            //encapsulate WebPge processing
            var webPage = new WebPage(link, textToSearch, userGuid);
            webPage.Scan();
        }
    }
}