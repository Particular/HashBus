namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.Twitter.Monitor.Events;
    using NServiceBus;
    using Tweetinvi;
    using Tweetinvi.Core.Credentials;

    class Monitoring
    {
        public static async Task StartAsync(
            IEndpointInstance endpointInstance,
            string track,
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret)
        {
            var credentials = new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
            while (true)
            {
                try
                {
                    var stream = Stream.CreateFilteredStream(credentials);
                    stream.AddTrack(track);

                    var sessionId = Guid.NewGuid();
                    stream.StreamStarted += (sender, args) =>
                    {
                        sessionId = Guid.NewGuid();
                        ColorConsole.WriteLine(
                            $"{DateTime.UtcNow.ToLocalTime()}".DarkGray(),
                            " ",
                            $" {track} ".DarkCyan().OnWhite(),
                            " ",
                            "stream started with session ID".Gray(),
                            " ",
                            $"{sessionId}".White());
                    };

                    stream.StreamStopped += (sender, args) => ColorConsole.WriteLine(
                        $"{DateTime.UtcNow.ToLocalTime()} ".DarkGray(),
                        $" {track} ".DarkCyan().OnWhite(),
                        " stream stopped.".Red(),
                        args.Exception == null ? string.Empty : $" {args.Exception.Message}".DarkRed());

                    stream.MatchingTweetReceived += async (sender, e) => 
                    {
                        var analyzeTweet = TweetMapper.Map(e.Tweet, track);
                        Writer.Write(analyzeTweet.Tweet);
                        await endpointInstance.Send(analyzeTweet)
                            .ConfigureAwait(false);

                        var tweetReceived = new TweetReceived()
                        {
                            SessionId = sessionId,
                            Track = track,
                            TweetId = e.Tweet.Id
                        };
                        await endpointInstance.Publish(tweetReceived)
                            .ConfigureAwait(false);
                    };

                    await stream.StartStreamMatchingAnyConditionAsync();
                }
                catch (Exception ex)
                {
                    ColorConsole.WriteLine($"{DateTime.UtcNow.ToLocalTime()} ".DarkGray(), "Error listening to Twitter stream.".Red(), $" {ex.Message}".DarkRed());
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
