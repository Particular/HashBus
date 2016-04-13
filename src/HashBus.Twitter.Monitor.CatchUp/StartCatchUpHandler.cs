namespace HashBus.Twitter.Monitor.CatchUp
{
    using System.Linq;
    using ColoredConsole;
    using HashBus.Twitter.Monitor.Commands;
    using NServiceBus;

    public class StartCatchUpHandler : IHandleMessages<StartCatchUp>
    {
        private readonly ISendOnlyBus bus;
        private readonly ITweetService tweetService;

        public StartCatchUpHandler(ISendOnlyBus bus, ITweetService tweetService)
        {
            this.bus = bus;
            this.tweetService = tweetService;
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
            foreach(var analyzeTweet in
                this.tweetService.Get(message.Track, message.TweetId).Select(tweet => TweetMapper.Map(tweet, message.Track)))
            {
                ++count;
                Writer.Write(analyzeTweet.Tweet);
                this.bus.Send(analyzeTweet);
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
