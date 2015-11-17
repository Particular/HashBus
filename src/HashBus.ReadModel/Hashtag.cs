namespace HashBus.ReadModel
{
    using System;

    public class Hashtag
    {
        public DateTime HashtaggedAt { get; set; }

        public long TweetId { get; set; }

        public string Text { get; set; }
    }
}
