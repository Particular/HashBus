namespace HashBus.Twitter.CatchUp
{
    using System;
    using ColoredConsole;
    using HashBus.Twitter.CatchUp.Commands;
    using HashBus.Twitter.Monitor.Events;
    using NServiceBus.Saga;

    public class TweetReceivedSaga : Saga<TweetReceivedSagaData>,
        IAmStartedByMessages<TweetReceived>
    {
        public void Handle(TweetReceived message)
        {
            if (Data.PreviousSessionId != Guid.Empty && Data.PreviousSessionId != message.SessionId)
            {
                ColorConsole.WriteLine(
                    $" {message.Track} ".DarkCyan().OnWhite(),
                    " ",
                    "needs catch up from tweet".Gray(),
                    " ",
                    $"{Data.PreviousTweetId}".White());

                Bus.Send(new StartCatchUp
                {
                    TweetId = Data.PreviousTweetId,
                    Track = message.Track,
                });

            }

            Data.PreviousSessionId = message.SessionId;
            Data.Hashtag = message.Track;
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
