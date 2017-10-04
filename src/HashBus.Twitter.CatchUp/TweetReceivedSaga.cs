namespace HashBus.Twitter.CatchUp
{
    using System;
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.Twitter.CatchUp.Commands;
    using HashBus.Twitter.Monitor.Events;
    using NServiceBus;

    public class TweetReceivedSaga : Saga<TweetReceivedSagaData>,
        IAmStartedByMessages<TweetReceived>
    {
        public async Task Handle(TweetReceived message, IMessageHandlerContext context)
        {
            if (Data.PreviousSessionId != Guid.Empty && Data.PreviousSessionId != message.SessionId)
            {
                ColorConsole.WriteLine(
                    $" {message.Track} ".DarkCyan().OnWhite(),
                    " ",
                    "needs catch up from tweet".Gray(),
                    " ",
                    $"{Data.PreviousTweetId}".White());

                await context.Send(new StartCatchUp
                    {
                        TweetId = Data.PreviousTweetId,
                        Track = message.Track,
                    })
                    .ConfigureAwait(false);
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
