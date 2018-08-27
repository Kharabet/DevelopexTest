namespace DevelopexTest.EventBus.Events
{
    public class ApplicationErrorEvent : EventBase
    {
        public string ErrorMessage { get; set; }
        public string Guid { get; set; }

        public ApplicationErrorEvent(string guid, string errorMessage)
        {
            Guid = guid;
            ErrorMessage = errorMessage;
        }
    }
}