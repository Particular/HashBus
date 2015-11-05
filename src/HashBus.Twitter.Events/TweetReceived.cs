namespace HashBus.Twitter.Events
{
    using System;
    using System.Collections.Generic;

    public class TweetReceived
    {
        public string Hashtag { get; set; }

        public long TweetId { get; set; }

        public DateTime TweetCreatedAt { get; set; }

        public long TweetCreatedById { get; set; }

        public string TweetCreatedByIdStr { get; set; }

        public string TweetCreatedByName { get; set; }

        public string TweetCreatedByScreenName { get; set; }

        public bool TweetIsRetweet { get; set; }

        public string TweetText { get; set; }

        public IList<UserMention> TweetUserMentions { get; set; }

        public IList<Hashtag> TweetHashtags { get; set; }

        public long RetweetedTweetId { get; set; }

        public DateTime RetweetedTweetCreatedAt { get; set; }

        public long RetweetedTweetCreatedById { get; set; }

        public string RetweetedTweetCreatedByIdStr { get; set; }

        public string RetweetedTweetCreatedByName { get; set; }

        public string RetweetedTweetCreatedByScreenName { get; set; }
		
        public string EndpointName { get; set; }

        public Guid SessionId { get; set; }
    }
}
