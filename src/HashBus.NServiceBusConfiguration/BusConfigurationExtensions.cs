namespace HashBus.NServiceBusConfiguration
{
    using NServiceBus;

    public static class BusConfigurationExtensions
    {
        public static void ApplyMessageConventions(this BusConfiguration configuration)
        {
            configuration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
        }
    }
}
