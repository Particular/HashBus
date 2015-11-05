using System;
using System.Linq;
using HashBus.Application.Events;
using HashBus.Twitter.Events;
using LiteGuard;
using NServiceBus;
using ColoredConsole;

namespace HashBus.Application
{
    public class HashtagTweetedHandler : IHandleMessages<HashtagTweeted>
    {
        private ISendOnlyBus bus;

        public HashtagTweetedHandler(ISendOnlyBus bus)
        {
            Guard.AgainstNullArgument("bus", bus);

            this.bus = bus;
        }

        public void Handle(HashtagTweeted message)
        {
            if (!message.TweetIsRetweet)
            {
                var tweetWithHashtag = new TweetWithHashtag
                {
                    Hashtag = message.Hashtag,
                    TweetCreatedAt = message.TweetCreatedAt,
                    TweetCreatedById = message.TweetCreatedById,
                    TweetCreatedByIdStr = message.TweetCreatedByIdStr,
                    TweetCreatedByName = message.TweetCreatedByName,
                    TweetCreatedByScreenName = message.TweetCreatedByScreenName,
                    TweetId = message.TweetId,
                    TweetText = message.TweetText,
                };

                ColorConsole.WriteLine(
                    $"{tweetWithHashtag.TweetCreatedByName}".White(),
                    $" @{tweetWithHashtag.TweetCreatedByScreenName}".DarkGray(),
                    " tweeted ".Gray(),
                    $"#{tweetWithHashtag.Hashtag}".DarkCyan().On(ConsoleColor.White),
                    $" · {message.TweetCreatedAt.ToLocalTime()}".DarkGray());

                bus.Publish(tweetWithHashtag);
            }

            foreach (var mentionMessage in message.TweetUserMentions
                .Where(userMention =>
                    message.TweetCreatedById != userMention.Id &&
                    message.TweetCreatedByIdStr != userMention.IdStr &&
                    message.TweetCreatedByScreenName != userMention.ScreenName &&
                    message.RetweetedTweetCreatedById != userMention.Id &&
                    message.RetweetedTweetCreatedByIdStr != userMention.IdStr &&
                    message.RetweetedTweetCreatedByScreenName != userMention.ScreenName &&
                    message.TweetText.Substring(0, userMention.Indices[0]).Trim().ToUpperInvariant() != "RT")
                .Select(userMention => new UserMentionedWithHashtag
                {
                    Hashtag = message.Hashtag,
                    TweetId = message.TweetId,
                    TweetCreatedAt = message.TweetCreatedAt,
                    TweetCreatedById = message.TweetCreatedById,
                    TweetCreatedByIdStr = message.TweetCreatedByIdStr,
                    TweetCreatedByName = message.TweetCreatedByName,
                    TweetCreatedByScreenName = message.TweetCreatedByScreenName,
                    TweetIsRetweet = message.TweetIsRetweet,
                    TweetText = message.TweetText,
                    UserMentionId = userMention.Id,
                    UserMentionIdStr = userMention.IdStr,
                    UserMentionName = userMention.Name,
                    UserMentionScreenName = userMention.ScreenName,
                }))
            {
                ColorConsole.WriteLine(
                    $"{mentionMessage.TweetCreatedByName}".White(),
                    $" @{mentionMessage.TweetCreatedByScreenName}".DarkGray(),
                    mentionMessage.TweetIsRetweet ? " retweeted ".Gray() : " tweeted ".Gray(),
                    $"@{mentionMessage.UserMentionScreenName}".Cyan(),
                    $" and ".Gray(),
                    $"#{mentionMessage.Hashtag}".DarkCyan().On(ConsoleColor.White),
                    $" · {message.TweetCreatedAt.ToLocalTime()}".DarkGray());

                bus.Publish(mentionMessage);
            }
        }
    }
}
