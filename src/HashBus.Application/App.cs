namespace HashBus.Application
{
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
    using System.Threading;
    using System.Threading.Tasks;
    using NServiceBus;

    class App
    {
        public static void Run()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Application");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
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
}
