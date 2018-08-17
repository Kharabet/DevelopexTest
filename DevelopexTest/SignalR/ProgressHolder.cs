using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using DevelopexTest.Models;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevelopexTest.SignalR
{
    //public class ProgressHolder
    //{
    //    private readonly ConcurrentDictionary<string, WebPage> _webPages = new ConcurrentDictionary<string, WebPage>();

    //    private readonly object _updateStockPricesLock = new object();
    //    private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);

    //    private readonly Timer _timer;
    //    private volatile bool _updatingStockPrices = false;


    //    private IHubConnectionContext<dynamic> Clients
    //    {
    //        get;
    //        set;
    //    }


    //    public ProgressHolder(IHubConnectionContext<dynamic> clients)
    //    {
    //        Clients = clients;

    //        _webPages.Clear();
    //        var stocks = new List<Stock>
    //        {
    //            new Stock { Symbol = "MSFT", Price = 30.31m },
    //            new Stock { Symbol = "APPL", Price = 578.18m },
    //            new Stock { Symbol = "GOOG", Price = 570.30m }
    //        };
    //        stocks.ForEach(stock => _webPages.TryAdd(stock.Symbol, stock));

    //        _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

    //    }

    //    public IEnumerable<WebPage> GetAllStocks()
    //    {
    //        return _webPages.Values;
    //    }

    //    private void UpdateStockPrices(object state)
    //    {
    //        lock (_updateStockPricesLock)
    //        {
    //            if (!_updatingStockPrices)
    //            {
    //                _updatingStockPrices = true;

    //                foreach (var stock in _webPages.Values)
    //                {
    //                    if (TryUpdateStockPrice(stock))
    //                    {
    //                        BroadcastStockPrice(stock);
    //                    }
    //                }

    //                _updatingStockPrices = false;
    //            }
    //        }
    //    }

    //    private bool TryUpdateStockPrice(WebPage stock)
    //    {
    //    }

    //    private void BroadcastStockPrice(WebPage stock)
    //    {
    //        Clients.All.updateStockPrice(stock);
    //    }
    //}
}