namespace HashBus.ReadModel
{
    using System;

    public class Mention
    {
        public DateTime MentionedAt { get; set; }

        public long TweetId { get; set; }

        public long? UserMentionId { get; set; }

        public string UserMentionIdStr { get; set; }

        public string UserMentionName { get; set; }

        public string UserMentionScreenName { get; set; }
    }
}
