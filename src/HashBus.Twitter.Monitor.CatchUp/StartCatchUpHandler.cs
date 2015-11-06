namespace HashBus.Twitter.Monitor.CatchUp
{
    using ColoredConsole;
    using HashBus.Twitter.Monitor.Commands;
    using NServiceBus;

    public class StartCatchUpHandler : IHandleMessages<StartCatchUp>
    {
        private readonly ISendOnlyBus bus;
        private readonly ITweetReceivedService tweetsReceived;

        public StartCatchUpHandler(ISendOnlyBus bus, ITweetReceivedService tweetsReceived)
        {
            this.bus = bus;
            this.tweetsReceived = tweetsReceived;
        }

        public void Handle(StartCatchUp message)
        {
            ColorConsole.WriteLine(
                "Catching up on".Gray(),
                " ",
                $" {message.Track} ".DarkCyan().OnWhite(),
                " ",
                "tweets since tweet".Gray(),
                " ",
                $"{message.TweetId}".White());

            var count = 0;
            foreach(var tweetReceived in
                this.tweetsReceived.Get(message.Track, message.TweetId, message.EndpointName, message.SessionId))
            {
                ++count;
                Writer.Write(tweetReceived);
                this.bus.Publish(tweetReceived);
            }

            ColorConsole.WriteLine(
                "Found".Gray(),
                " ",
                $"{count:N0}".White(),
                " ",
                $" {message.Track} ".DarkCyan().OnWhite(),
                " ",
                $"tweet{(count == 1 ? "" : "s")} since tweet".Gray(),
                " ",
                $"{message.TweetId}".White());
        }
    }
}
