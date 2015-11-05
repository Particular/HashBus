namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using NServiceBus;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
    using NServiceBus.Persistence;

    class Program
    {
        static void Main()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Twitter.Monitor.CatchUp");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>()
                .ConnectionString(@"Data Source=.\SqlExpress;Initial Catalog=NServiceBus;Integrated Security=True");

            using (Bus.Create(busConfiguration).Start())
            {
                //
                Console.ReadLine();
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