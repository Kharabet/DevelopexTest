using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using DevelopexTest.Models;

namespace DevelopexTest.Controllers
{
    public class SearchController : ApiController
    {
        // POST api/search
        public void Post(SearchRequest request)
        {
            var webPageLinkParser = new WebPageLinkParser();

            var ctProvider = new CancellationTokenProvider();

            //UserGuid uses as User identifier. I didn't want to use authorization :)
            //Collect users and their CancellationTokenSources 
            ctProvider.Add(request.UserGuid);
            var token = ctProvider.GetToken(request.UserGuid);

            Task.Run(() => webPageLinkParser.Parse(request, token));
        }
    }
}
