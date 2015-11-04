namespace HashBus.Twitter.Monitor.CatchUp
{
    using Events;
    using NServiceBus.Persistence.NHibernate;
    using NServiceBus.Saga;

    class HashtagTweetedSagaFinder : IFindSagas<HashtagTweetedSagaData>.Using<HashtagTweeted>
    {
        public HashtagTweetedSagaFinder(NHibernateStorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public HashtagTweetedSagaData FindBy(HashtagTweeted message)
        {
            var session = storageContext.Session;

            return session.QueryOver<HashtagTweetedSagaData>()
                .Where(d => d.HashtagMonitored == message.Hashtag).And(e => e.EndpointName == message.EndpointName)
                .SingleOrDefault();
        }

        NHibernateStorageContext storageContext;
    }
}