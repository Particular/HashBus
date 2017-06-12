namespace HashBus.Twitter.CatchUp
{
    using System.Linq;
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.Twitter.CatchUp.Commands;
    using NServiceBus;

    public class StartCatchUpHandler : IHandleMessages<StartCatchUp>
    {
        private readonly ITweetService tweetService;

        public StartCatchUpHandler(ITweetService tweetService)
        {
            this.tweetService = tweetService;
        }

        public async Task Handle(StartCatchUp message, IMessageHandlerContext context)
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
                await context.Send(analyzeTweet)
                    .ConfigureAwait(false);
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
