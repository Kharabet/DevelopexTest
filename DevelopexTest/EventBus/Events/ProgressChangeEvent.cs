namespace DevelopexTest.EventBus.Events
{
    public class ProgressChangedEvent : EventBase
    {
        public string Url { get; private set; }
        public int Status { get; private set; }
        public string ConnectionId { get; set; }
        public string Guid { get; set; }

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