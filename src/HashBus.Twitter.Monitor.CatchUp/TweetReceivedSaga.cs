namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using ColoredConsole;
    using Commands;
    using Events;
    using NServiceBus.Saga;

    public class TweetReceivedSaga : Saga<TweetReceivedSagaData>,
        IAmStartedByMessages<TweetReceived>
    {
        public void Handle(TweetReceived message)
        {
            if (Data.PreviousSessionId != Guid.Empty && Data.PreviousSessionId != message.SessionId)
            {
                if (message.IsSimulated)
                {
                    ColorConsole.WriteLine(
                        $"{message.EndpointName}".White(),
                        " ",
                        "for".Gray(),
                        " ",
                        $" {message.Track} ".DarkCyan().OnWhite(),
                        " ",
                        "is simulated and doesn't need catch up".Gray());
                }
                else
                {
                    ColorConsole.WriteLine(
                        $"{message.EndpointName}".White(),
                        " ",
                        "for".Gray(),
                        " ",
                        $" {message.Track} ".DarkCyan().OnWhite(),
                        " ",
                        "needs catch up from tweet".Gray(),
                        " ",
                        $"{Data.PreviousTweetId}".White());

                    Bus.Send(new StartCatchUp
                    {
                        TweetId = Data.PreviousTweetId,
                        EndpointName = message.EndpointName,
                        Track = message.Track,
                        SessionId = message.SessionId,
                    });
                }
            }

            Data.PreviousSessionId = message.SessionId;
            Data.Hashtag = message.Track;
            Data.EndpointName = message.EndpointName;
            Data.PreviousTweetId = message.TweetId;
        }

        /// <remarks>
        /// Track and EndpointNmae are a composite key, so this is handled in <see cref="TweetReceivedSagaFinder"/>.
        /// </remarks>>
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TweetReceivedSagaData> mapper)
        {
        }
    }
}
