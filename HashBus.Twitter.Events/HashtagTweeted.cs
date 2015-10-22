using System;
using NServiceBus;

namespace HashBus.Twitter.Events
{
    public class HashtagTweeted : IEvent
    {
        public long Id { get; set; }

        public string Hashtag { get; set; }

        public bool IsRetweet { get; set; }

        public string Text { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string UserScreenName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}