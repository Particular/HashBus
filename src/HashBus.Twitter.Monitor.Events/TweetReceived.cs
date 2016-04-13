namespace HashBus.Twitter.Monitor.Events
{
    using System;

    public class TweetReceived
    {
        public string Track { get; set; }

        public long TweetId { get; set; }
        
        public Guid SessionId { get; set; }
    }
}
