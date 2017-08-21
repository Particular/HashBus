namespace HashBus.Twitter.CatchUp
{
    using HashBus.Twitter.Monitor.Events;
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
                .Where(d => d.Hashtag == message.Track)
                .SingleOrDefault();
        }

        NHibernateStorageContext storageContext;
    }
}
