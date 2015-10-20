using System;
using System.Threading.Tasks;
using NServiceBus;

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
            await SimulateOrderPlaced(bus);
        }
    }

    static async Task SimulateOrderPlaced(IBus bus)
    {
        Console.WriteLine("Press enter to simulate that an order was placed");
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

            var orderPlaced = new OrderPlaced
            {
                OrderId = id
            };

            await bus.PublishAsync(orderPlaced);

            Console.WriteLine("Sent a new OrderPlaced message with id: {0}", id.ToString("N"));
        }
    }
}
