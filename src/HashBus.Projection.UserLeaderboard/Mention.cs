namespace HashBus.Projection
{
    public class Mention
    {
        public long TweetId { get; set; }

        public long? UserMentionId { get; set; }

        public string UserMentionIdStr { get; set; }

        public string UserMentionName { get; set; }

        public string UserMentionScreenName { get; set; }
    }
}
