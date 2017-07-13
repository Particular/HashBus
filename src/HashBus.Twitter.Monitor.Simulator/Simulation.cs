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
        public static void Start(ISendOnlyBus bus)
        {
            var random = new Random();
            var countOfUsers = 15;
            while (true)
            {
                Thread.Sleep((int)Math.Pow(random.Next(6), 5));
                var now = DateTime.UtcNow;
                var hashtag = "Simulated";
                var track = $"#{hashtag}";
                var hashtagText = track;
                var secondaryHashtag = new string(char.ConvertFromUtf32(random.Next(65, 80)).ElementAt(0), 6);

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

                var userId = random.Next(countOfUsers);
                var userMentionId = random.Next(countOfUsers);
                var userMentionIndex = random.Next(31) + 1;
                var retweetedUserId = random.Next(countOfUsers);
                var text = string.Join(
                        string.Empty,
                        Enumerable.Range(0, userMentionIndex - 1).Select(i => char.ConvertFromUtf32(random.Next(65, 128)))) +
                    $" @johnsmith{userMentionId} " +
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
                        CreatedByName = $"John Smith{userId}",
                        CreatedByScreenName = $"johnsmith{userId}",
                        Text = text,
                        UserMentions = new List<UserMention>
                        {
                            new UserMention
                            {
                                Id=userMentionId,
                                IdStr= $"{userMentionId}",
                                Indices = new List<int> { userMentionIndex, userMentionIndex + $"@johnsmith{userMentionId}".Length, },
                                Name = $"John Smith{userMentionId}",
                                ScreenName = $"johnsmith{userMentionId}",
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
                                CreatedByName = $"John Smith{retweetedUserId}",
                                CreatedByScreenName = $"johnsmith{retweetedUserId}",
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
