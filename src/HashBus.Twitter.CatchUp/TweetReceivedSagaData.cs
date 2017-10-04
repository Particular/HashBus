namespace HashBus.Twitter.CatchUp
{
    using System;
    using NServiceBus;

    public class TweetReceivedSagaData : ContainSagaData
    {
        public virtual string Hashtag { get; set; }

        public virtual Guid PreviousSessionId { get; set; }

        public virtual long PreviousTweetId { get; set; }
    }
}
