using NServiceBus;
using System;

namespace HashBus.Application.Events
{
    public class UserMentionedWithHashtag : IEvent
    {
        public long TweetId { get; set; }

        public string Hashtag { get; set; }

        public bool IsRetweet { get; set; }

        public long? UserMentionId { get; set; }

        public string UserMentionIdStr { get; set; }

        public string UserMentionName { get; set; }

        public string UserMentionScreenName { get; set; }

        public DateTime TweetCreatedAt { get; set; }
    }
}
