using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DevelopexTest.EventBus.Events;

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
        readonly string _text;
        readonly string _link;
        readonly string _userGuid;

        public int CountMatches { get; private set; }
        public ScanningStatus Status { get; private set; }
        public string Content { get; private set; }
        public List<string> InnerLinks { get; private set; }

        public WebPage(string link, string text, string userGuid)
        {
            _link = link;
            _text = text;
            _userGuid = userGuid;
        }

        public void Scan()
        {
            EventBus.EventBus.Instance.Publish(CreateEvent(ScanningStatus.InProgress));
            try
            {
                var html = DownloadContent(_link);
                InnerLinks = SearchForLinks(html);
                if (InnerLinks.Count > 0 )
                {
                    EventBus.EventBus.Instance.Publish(new LinksFindedEvent(_link, InnerLinks));
                }
                Content = ExtractContent(html);
                CountMatches = TextCountMatches();
                EventBus.EventBus.Instance.Publish(CountMatches == 0
                    ? CreateEvent(ScanningStatus.NotFound)
                    : CreateEvent(ScanningStatus.Finded));
            }
            catch (Exception e)
            {
                EventBus.EventBus.Instance.Publish(CreateEvent(ScanningStatus.Error, e.Message));
            }

        }

        string DownloadContent(string url)
        {
            WebClient client = new System.Net.WebClient { Encoding = Encoding.UTF8 };
            var html = client.DownloadString(url);
            var singleline = Regex.Replace(html, @"\r|\n", "").Trim();

            return singleline;
        }

        string ExtractContent(string html)
        {
            return Regex.Replace(html, @"<(script|style).*?</\1>|<.*?>", "", RegexOptions.Multiline).Trim();
        }

        List<string> SearchForLinks(string html)
        {

            string urlPattern = "<a.*?href=(\"|\')(http\\w?://.+?)(\"|\').*?>";

            var matchCollection = Regex.Matches(html, urlPattern);
            return matchCollection.Cast<Match>().Select(match => match.Groups[2].Value).Distinct().ToList();
        }

        public int TextCountMatches()
        {

            var matches = Regex.Matches(Content, _text, RegexOptions.IgnoreCase).Count;
            return matches > 0 ? matches : 0;
        }

        public ProgressChangedEvent CreateEvent(ScanningStatus status, string errorMessage = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return new ProgressChangedEvent((int)status, _userGuid, _link, errorMessage);
            }
            return new ProgressChangedEvent((int)status, _userGuid, _link);
        }


    }
}