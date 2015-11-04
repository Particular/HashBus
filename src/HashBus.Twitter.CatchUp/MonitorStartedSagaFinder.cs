namespace HashBus.Twitter.Monitor.CatchUp
{
    using Commands;
    using NServiceBus.Persistence.NHibernate;
    using NServiceBus.Saga;

    class RegisterMonitorSagaFinder : IFindSagas<HashtagTweetedSagaData>.Using<RegisterMonitor>
    {
        public RegisterMonitorSagaFinder(NHibernateStorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public HashtagTweetedSagaData FindBy(RegisterMonitor message)
        {
            var session = storageContext.Session;

            return session.QueryOver<HashtagTweetedSagaData>()
                .Where(d => d.HashtagMonitored == message.HashtagMonitored).And(e => e.EndpointName == message.EndpointName)
                .SingleOrDefault();
        }

        NHibernateStorageContext storageContext;
    }
}