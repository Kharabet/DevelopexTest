using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DevelopexTest.Models
{
    public enum ScanningStatus
    {
        InProgress = 1,
        Finded = 2,
        NotFound = 3,
        Error = 4

    }

    public class WebPage
    {
        public ScanningStatus Status { get; set; }

        string _content;
        string _text;
        string _link;
        int _countMatches;

        public WebPage(string link)
        {
            _link = link;
        }

        public void Scan()
        {
            Status = ScanningStatus.InProgress;
            try
            {
                SearchForLinks();
                _countMatches = CountMatches();
                Status = _countMatches == 0 ? ScanningStatus.NotFound : ScanningStatus.Finded;
            }
            catch (Exception e)
            {
                Status = ScanningStatus.Error;
            }

        }

        void SearchForLinks()
        {
             WebClient client = new WebClient { Encoding = Encoding.UTF8 };

            // Download string.
             _content = client.DownloadString(_link);


            string urlPattern = @"http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?";

            var links = Regex.Matches(_content, urlPattern).Cast<string>().ToList();

        }

        public int CountMatches()
        {
            var contentText = Regex.Replace(_content, @"<.*?>", "").Trim();
            var matches = Regex.Matches(contentText, _text).Count;
            return matches > 0 ? matches : 0;
        }

    }
}