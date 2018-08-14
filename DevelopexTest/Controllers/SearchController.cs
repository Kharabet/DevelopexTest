using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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

            var startPage = new WebPage(request.StartUrl);
            startPage.Scan();



            /* First approach
             * 
             * 
             * 
             * 
            // Create web client.
            WebClient client = new WebClient { Encoding = Encoding.UTF8 };

            // Download string.
            string value = client.DownloadString(request.StartUrl);


            string urlPattern = @"http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?";
            var links = Regex.Matches(value, urlPattern).Cast<string>().ToList();
            var edges = new List<Tuple<string, string>>();

            while (request.MaxUrlsCount > links.Count())
            {
                foreach (var link in links)
                {
                    var nestedLinks = Regex.Matches(client.DownloadString(link), urlPattern).Cast<string>();
                    links.AddRange(nestedLinks);

                    foreach (var nestedLink in nestedLinks)
                    {
                        edges.Add(Tuple.Create(link, nestedLink));
                    }
                }
            }

            var graph = new Graph<string>(links, edges);

            var algorithms = new Algorithms();

            var path = new List<string>();

            Console.WriteLine(string.Join(", ", algorithms.BFS(graph, request.StartUrl, v => path.Add(v))));

            var finalLinks = path.Take(request.MaxUrlsCount); 


            */




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
