using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HashBus.Twitter.Events;
using NServiceBus;
using System.Linq;

namespace HashBus.Twitter.Monitor.Simulator
{
    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Twitter.Monitor");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");

            using (var bus = await Bus.Create(busConfiguration).StartAsync())
            {
                await SimulateTwitter(bus);
            }
        }

        static async Task SimulateTwitter(ISendOnlyBus bus)
        {
            var random = new Random();
            while (true)
            {
                Thread.Sleep(random.Next(3000));

                var now = DateTime.UtcNow;
                var userId = random.Next(64);
                var userMentionId = random.Next(64);
                var userMentionIndex = random.Next(31) + 1;
                var message = new HashtagTweeted
                {
                    Id = now.Ticks,
                    CreatedAt = now,
                    Hashtag = "Simulated",
                    IsRetweet = now.Millisecond % 3 == 0,
                    Text =
                        string.Join(
                            string.Empty,
                            Enumerable.Range(0, userMentionIndex - 1).Select(i => char.ConvertFromUtf32(random.Next(65, 128)))) +
                        $" @johnsmith{userMentionId} " +
                        string.Join(
                            string.Empty,
                            Enumerable.Range(0, random.Next(32)).Select(i => char.ConvertFromUtf32(random.Next(65, 128)))) +
                            " #Simulated",
                    UserId = userId,
                    UserName = $"John Smith{userId}",
                    UserScreenName = $"johnsmith{userId}",
                    UserMentions = new List<UserMention>
                    {
                        new UserMention
                        {
                            Id=userMentionId,
                            IdStr= $"{userMentionId}",
                            Indices = new List<int> { userMentionIndex, userMentionIndex + $"@johnsmith{userMentionId}".Length },
                            Name = $"John Smith{userMentionId}",
                            ScreenName = $"johnsmith{userMentionId}",
                        }
                    },
                };

                Writer.Write(message);
                await bus.PublishAsync(message);
            }
        }
    }
}
