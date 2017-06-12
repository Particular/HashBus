namespace HashBus.Twitter.CatchUp
{
    using System.Threading.Tasks;
    using HashBus.Twitter.Monitor.Events;
    using NServiceBus;
    using NServiceBus.Extensibility;
    using NServiceBus.Persistence;
    using NServiceBus.Sagas;

    class TweetReceivedSagaFinder : IFindSagas<TweetReceivedSagaData>.Using<TweetReceived>
    {
        public Task<TweetReceivedSagaData> FindBy(TweetReceived message, SynchronizedStorageSession storageSession, ReadOnlyContextBag context)
        {
            var tweetReceivedSagaData = storageSession.Session().QueryOver<TweetReceivedSagaData>()
                .Where(d => d.Hashtag == message.Track)
                .SingleOrDefault();

            return Task.FromResult(tweetReceivedSagaData);
        }
    }
}
