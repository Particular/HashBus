using System;

namespace HashBus.Application.Events
{
    public class TweetWithHashtag 
    {
        public string Hashtag { get; set; }

        public long TweetId { get; set; }

        public DateTime TweetCreatedAt { get; set; }

        public long TweetCreatedById { get; set; }

        public string TweetCreatedByIdStr { get; set; }

        public string TweetCreatedByName { get; set; }

        public string TweetCreatedByScreenName { get; set; }

        public string TweetText { get; set; }
    }
}
