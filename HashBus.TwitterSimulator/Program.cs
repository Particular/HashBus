using System;
using System.Threading.Tasks;
using NServiceBus;

namespace HashBus.TwitterSimulator
{
    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            BusConfiguration busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Server");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");

            using (IBus bus = await Bus.Create(busConfiguration).StartAsync())
            {
                await SimulateHashtagTweeted(bus);
            }
        }

        static async Task SimulateHashtagTweeted(IBus bus)
        {
            Console.WriteLine("Press enter to simulate that a hashtag was tweeted");
            Console.WriteLine("Press any key to exit");

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    break;
                }

                Guid id = Guid.NewGuid();

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
