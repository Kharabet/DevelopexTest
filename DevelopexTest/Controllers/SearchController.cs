using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using DevelopexTest.Models;

namespace DevelopexTest.Controllers
{
    public class SearchController : ApiController
    {
        // GET api/search
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/search/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/search
        public void Post(SearchRequest request)
        {
            var webPageLinkParser = new WebPageLinkParser();
            Task.Run(() => webPageLinkParser.Parse(request));
        }
        
        // PUT api/search/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/search/5
        public void Delete(int id)
        {
        }
    }
}
