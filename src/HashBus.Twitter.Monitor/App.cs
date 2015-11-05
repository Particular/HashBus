namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
    using NServiceBus.Persistence;

    class App
    {
        const string EndpointName = "HashBus.Twitter.Monitor";

        public static async Task RunAsync(string hashtag, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(EndpointName);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>()
               .ConnectionString(@"Data Source=.\SqlExpress;Initial Catalog=NServiceBus;Integrated Security=True");

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                var sessionId = Guid.NewGuid();

                await Monitoring.StartAsync(bus, hashtag, consumerKey, consumerSecret, accessToken, accessTokenSecret, EndpointName, sessionId);
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
