namespace HashBus.Twitter.Monitor.Simulator
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using NServiceBus;
    using System.Linq;
    using HashBus.Application.Commands;

    class Simulation
    {
        public static void Start(ISendOnlyBus bus, string hashtag)
        {
            var userNames = new[]
            {
                "Erik",
                "ama",
                "Karekin",
                "cosima",
                "Kamyrn",
                "ajay",
                "Timaios",
                "mila",
                "Odilia",
                "randi",
                "Kennard",
                "ilike",
                "Rab",
                "yolonda",
                "Ikaia",
            };

            var secondaryHashtags = new[]
            {
                "csharp",
                "fsharp",
                "nservicebus",
                "azure",
                "dotnet",
                "oss",
                "javascript",
                "microservices",
                "serverless",
                "ddd",
                "aspnetcore",
            };

            var random = new Random();

            while (true)
            {
                Thread.Sleep((int)Math.Pow(random.Next(6), 5));
                var now = DateTime.UtcNow;
                var track = $"#{hashtag}";
                var hashtagText = track;
                var secondaryHashtag = secondaryHashtags[random.Next(secondaryHashtags.Length)];

                if (now.Millisecond % 3 == 0)
                {
                    hashtag = hashtag.ToLowerInvariant();
                    hashtagText = hashtagText.ToLowerInvariant();
                    secondaryHashtag = secondaryHashtag.ToLowerInvariant();
                }

                if (now.Millisecond % 7 == 0)
                {
                    hashtag = hashtag.ToUpperInvariant();
                    hashtagText = hashtagText.ToUpperInvariant();
                    secondaryHashtag = secondaryHashtag.ToUpperInvariant();
                }

                var userId = random.Next(userNames.Length);
                var userMentionId = random.Next(userNames.Length);
                var userMentionIndex = random.Next(31) + 1;
                var retweetedUserId = random.Next(userNames.Length);
                var text = string.Join(
                        string.Empty,
                        Enumerable.Range(0, userMentionIndex - 1).Select(i => char.ConvertFromUtf32(random.Next(65, 128)))) +
                    $" {userNames[userMentionId]} " +
                    string.Join(
                        string.Empty,
                        Enumerable.Range(0, random.Next(32)).Select(i => char.ConvertFromUtf32(random.Next(65, 128)))) +
                    $" {hashtagText}" +
                    $" #{secondaryHashtag}";

                var message = new AnalyzeTweet
                {
                    Tweet = new Tweet
                    {
                        Track = track,
                        Id = now.Ticks,
                        CreatedAt = now,
                        CreatedById = userId,
                        CreatedByIdStr = $"{userId}",
                        CreatedByName = userNames[userId],
                        CreatedByScreenName = userNames[userId],
                        Text = text,
                        UserMentions = new List<UserMention>
                        {
                            new UserMention
                            {
                                Id=userMentionId,
                                IdStr= $"{userMentionId}",
                                Indices = new List<int> { userMentionIndex, userMentionIndex + userNames[userMentionId].Length, },
                                Name = userNames[userMentionId],
                                ScreenName = userNames[userMentionId],
                            },
                        },
                        Hashtags = new List<Hashtag>
                        {
                            new Hashtag
                            {
                                Text = hashtag,
                                Indices = new[] { text.Length - $"{hashtagText} #{secondaryHashtag}".Length, text.Length - $" #{secondaryHashtag}".Length, },
                            },
                            new Hashtag
                            {
                                Text = secondaryHashtag,
                                Indices = new[] { text.Length - $"#{secondaryHashtag}".Length, text.Length, },
                            },
                        },
                        RetweetedTweet = now.Millisecond % 3 == 0
                            ? new Tweet
                            {
                                Track = track,
                                Id = now.AddDays(-1000).Ticks,
                                CreatedAt = now.AddDays(-1000),
                                CreatedById = retweetedUserId,
                                CreatedByIdStr = $"{retweetedUserId}",
                                CreatedByName = userNames[retweetedUserId],
                                CreatedByScreenName = userNames[retweetedUserId],
                                Text = text,
                                UserMentions = new List<UserMention>(),
                                Hashtags = new List<Hashtag>(),
                                RetweetedTweet = null,
                            }
                            : null,
                    }
                };

                Writer.Write(message.Tweet);
                bus.Send(message);
            }
        }
    }
}
