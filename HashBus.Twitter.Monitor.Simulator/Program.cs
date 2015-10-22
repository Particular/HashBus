using System;
using System.Threading.Tasks;
using HashBus.Twitter.Events;
using NServiceBus;

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
                await SimulateHashtagTweeted(bus);
            }
        }

        static async Task SimulateHashtagTweeted(ISendOnlyBus bus)
        {
            Console.WriteLine("Press enter to simulate that a hashtag was tweeted");
            Console.WriteLine("Press any key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    break;
                }

                var id = DateTime.UtcNow.Ticks;

                var message = new HashtagTweeted
                {
                    Id = id
                };

                await bus.PublishAsync(message);

                Console.WriteLine("Sent a new HashtagTweeted message with id: {0}", id.ToString("N"));
            }
        }
    }
}
