namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using NServiceBus.Saga;

    public class HashtagTweetedSagaData : IContainSagaData
    {
        public virtual string EndpointName { get; set; }
        public virtual string HashtagMonitored { get; set; }
        public virtual long? LatestTweetedId { get; set; }
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
    }
}