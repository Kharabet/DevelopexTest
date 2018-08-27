using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevelopexTest.EventBus
{
    public class ProgressChangedEvent : EventBase
    {
        public string Url { get; private set; }
        public int Status { get; private set; }
        public string Guid { get; private set; }
        public string ErrorMessage { get; private set; }

        public ProgressChangedEvent(int status, string guid, string url, string errorMessage = null)
        {
            Status = status;
            Guid = guid;
            Url = url;
            ErrorMessage = errorMessage;
        }
    }
}