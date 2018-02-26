namespace HashBus.Twitter.Analyzer.Events
{
    using System;
    using System.Collections.Generic;

    public class Tweet
    {
        public string Track { get; set; }

        public long Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public long CreatedById { get; set; }

        public string CreatedByName { get; set; }

        public string CreatedByScreenName { get; set; }

        public string Text { get; set; }

        public IList<UserMention> UserMentions { get; set; }

        public IList<Hashtag> Hashtags { get; set; }

        public Tweet RetweetedTweet { get; set; }
    }
}
