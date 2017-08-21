namespace HashBus.Twitter.CatchUp
{
    using System;
    using NServiceBus.Saga;

    public class TweetReceivedSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }

        public virtual string Originator { get; set; }

        public virtual string OriginalMessageId { get; set; }

        public virtual string Hashtag { get; set; }

        public virtual Guid PreviousSessionId { get; set; }

        public virtual long PreviousTweetId { get; set; }
    }
}
