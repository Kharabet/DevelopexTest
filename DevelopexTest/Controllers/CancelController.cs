﻿using System.Web.Http;
using DevelopexTest.Models;

namespace DevelopexTest.Controllers
{
    public class CancelController : ApiController
    {
        // POST api/cancel
        public void Post([FromBody]string guid)
        {
            if (CancellationTokenProvider.GuidCtsDictionary.ContainsKey(guid))
            {
                var cts = CancellationTokenProvider.GuidCtsDictionary[guid];
                cts.Cancel();
            }
        }
    }
}
