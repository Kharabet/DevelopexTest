using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

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
        string _text;
        string _link;

        public int CountMatches { get; private set; }
        public ScanningStatus Status { get; private set; }
        public string Content { get; private set; }
        public List<string> InnerLinks { get; private set; }
        
        public WebPage(string link, string text)
        {
            _link = link;
            _text = text;
        }

        public void Scan()
        {
            Status = ScanningStatus.InProgress;
            try
            {
                var html = DownloadContent();
                SearchForLinks(html);
                ExtractContent(html);
                CountMatches = TextCountMatches();
                Status = CountMatches == 0 ? ScanningStatus.NotFound : ScanningStatus.Finded;
            }
            catch (Exception e)
            {
                Status = ScanningStatus.Error;
            }

        }

        string DownloadContent()
        {
            WebClient client = new WebClient { Encoding = Encoding.UTF8 };

            // Download string.
            return client.DownloadString(_link);
        }

        void ExtractContent(string html)
        {
            var singleline = Regex.Replace(html, @"\r|\n", "").Trim();
            Content = Regex.Replace(singleline, @"<.*?>|<(script|style).*?</\1>", "", RegexOptions.Multiline).Trim();
        }

        void SearchForLinks(string html)
        {

            string urlPattern = "<a.*?href=(\"|\')(http\\w?://.+?)(\"|\').*?>";

            var matchCollection = Regex.Matches(html, urlPattern);
            InnerLinks = matchCollection.Cast<Match>().Select(match => match.Groups[2].Value).Distinct().ToList();
        }

        public int TextCountMatches()
        {

            var matches = Regex.Matches(Content, _text).Count;
            return matches > 0 ? matches : 0;
        }

    }
}