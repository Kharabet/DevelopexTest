using Newtonsoft.Json;

namespace DevelopexTest.Models
{
    public class SearchRequest
    {
        public string StartUrl { get; set; }

        public int MaxThreadsCount { get; set; }

        public int MaxUrlsCount { get; set; }

        public string TextToSearch { get; set; }
    }
}