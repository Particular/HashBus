namespace HashBus.Application
{
    using System.Linq;
    using HashBus.Application.Events;

    static class TweetMapper
    {
        public static Tweet Map(Commands.Tweet tweet)
        {
            return tweet == null
                ? null  
                : new Tweet()
            {
                CreatedAt = tweet.CreatedAt,
                CreatedById = tweet.CreatedById,
                CreatedByIdStr = tweet.CreatedByIdStr,
                CreatedByName = tweet.CreatedByName,
                CreatedByScreenName = tweet.CreatedByScreenName,
                Hashtags = tweet.Hashtags
                    .Select(hashTag => new Hashtag()
                    {
                        Text = hashTag.Text,
                        Indices = hashTag.Indices
                    }).ToList(),
                Id = tweet.Id,
                RetweetedTweet = Map(tweet.RetweetedTweet),
                Text = tweet.Text,
                Track = tweet.Track,
                UserMentions = tweet.UserMentions
                    .Select(userMention => new UserMention()
                    {
                        Id = userMention.Id,
                        IdStr = userMention.IdStr,
                        Indices = userMention.Indices,
                        Name = userMention.Name,
                        ScreenName = userMention.ScreenName
                    }).ToList()
            };
        }
    }
}