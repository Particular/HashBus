namespace HashBus.Twitter.Monitor.CatchUp.Commands
{
    public class StartCatchUp
    {
        public long TweetId { get; set; }

        public string Track { get; set; }
        
        public string EndpointName { get; set; }
    }
}
