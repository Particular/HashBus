namespace HashBus.Twitter.Monitor.CatchUp
{
    using Events;
    using NServiceBus.Persistence.NHibernate;
    using NServiceBus.Saga;

    class HashtagTweetedSagaFinder : IFindSagas<HashtagTweetedSagaData>.Using<TweetReceived>
    {
        public HashtagTweetedSagaFinder(NHibernateStorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public HashtagTweetedSagaData FindBy(TweetReceived message)
        {
            var session = storageContext.Session;

            return session.QueryOver<HashtagTweetedSagaData>()
                .Where(d => d.Hashtag == message.Track).And(e => e.EndpointName == message.EndpointName)
                .SingleOrDefault();
        }

        NHibernateStorageContext storageContext;
    }
}