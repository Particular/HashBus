using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace HashBus
{
    class ErrorQueueConfigurationProvider : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    {
        public MessageForwardingInCaseOfFaultConfig GetConfiguration()
        {
            return new MessageForwardingInCaseOfFaultConfig { ErrorQueue = "error", };
        }
    }
}
