namespace HashBus.Twitter.Monitor.Commands
{
    using System;

    public class StartCatchUp
    {
        public long TweetId { get; set; }
        public string Track { get; set; }
        public Guid SessionId { get; set; }
        public string EndpointName { get; set; }
    }
}
