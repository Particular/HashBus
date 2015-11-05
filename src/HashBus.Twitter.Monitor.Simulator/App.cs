namespace HashBus.Twitter.Monitor.Simulator
{
    using System;
    using NServiceBus;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
    using NServiceBus.Persistence;

    class App
    {
        const string EndpointName = "HashBus.Twitter.Monitor";

        public static void Run()
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

                Simulation.Start(bus, EndpointName, sessionId);
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
