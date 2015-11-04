using System.Threading.Tasks;
using NServiceBus;

namespace HashBus.Twitter.Monitor
{
    using System.Configuration;
    using HashBus.Twitter.Monitor.Commands;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;

    class App
    {
        const string EndpointName = "HashBus.Twitter.Monitor";

        public static async Task RunAsync(string hashtag, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(EndpointName);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                var registerMonitor = new RegisterMonitor
                {
                    EndpointName = EndpointName,
                    HashtagMonitored = hashtag
                };

                bus.SendAsync(registerMonitor).GetAwaiter().GetResult();

                await Monitoring.StartAsync(bus, hashtag, consumerKey, consumerSecret, accessToken, accessTokenSecret, EndpointName);
            }
        }
    }

    public class Configuration : INeedInitialization
    {
        public void Customize(BusConfiguration configuration)
        {
            configuration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
        }
    }

    public class ErrorQueueProvider : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    {
        public MessageForwardingInCaseOfFaultConfig GetConfiguration()
        {
            return new MessageForwardingInCaseOfFaultConfig
            {
                ErrorQueue = "error"
            };
        }
    }

    public class AuditQueueProvider : IProvideConfiguration<AuditConfig>
    {
        public AuditConfig GetConfiguration()
        {
            return new AuditConfig
            {
                QueueName = "audit"
            };
        }
    }
}
