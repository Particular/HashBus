using NServiceBus;

namespace HashBus.Conventions
{
    public static class MessageConventionsExtentions
    {
        public static void ApplyMessageConventions(this BusConfiguration configuration)
        {
            configuration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
        }
    }
}
