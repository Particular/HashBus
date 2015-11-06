namespace HashBus.NServiceBusConfiguration
{
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;

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
