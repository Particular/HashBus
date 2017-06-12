namespace HashBus.NServiceBusConfiguration
{
    using NServiceBus;

    public static class BusConfigurationExtensions
    {
        public static void ApplyMessageConventions(this EndpointConfiguration configuration)
        {
            configuration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
        }

        public static void ApplyErrorAndAuditQueueSettings(this EndpointConfiguration configuration)
        {
            configuration.AuditProcessedMessagesTo("audit");
            configuration.SendFailedMessagesTo("error");
        }
    }
}
