namespace HashBus.Twitter.Monitor.CatchUp
{
    using Events;
    using NServiceBus.Persistence.NHibernate;
    using NServiceBus.Saga;

    class TweetReceivedSagaFinder : IFindSagas<TweetReceivedSagaData>.Using<TweetReceived>
    {
        public TweetReceivedSagaFinder(NHibernateStorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public TweetReceivedSagaData FindBy(TweetReceived message)
        {
            return this.storageContext.Session.QueryOver<TweetReceivedSagaData>()
                .Where(d => d.Hashtag == message.Tweet.Track).And(e => e.EndpointName == message.EndpointName)
                .SingleOrDefault();
        }

        NHibernateStorageContext storageContext;
    }
}
