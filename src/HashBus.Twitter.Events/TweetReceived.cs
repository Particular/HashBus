namespace HashBus.Twitter.Events
{
    using System;
    using HashBus.Application.Events;

    public class TweetReceived
    {
        public Tweet Tweet { get; set; }

        public string EndpointName { get; set; }

        public Guid SessionId { get; set; }

        public bool IsSimulated { get; set; }
    }
}
