using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevelopexTest.Models;

namespace DevelopexTest.SignalR
{
    public class ProgressHolder
    {
        private readonly ConcurrentDictionary<string, WebPage> _webPages = new ConcurrentDictionary<string, WebPage>();
    }
}