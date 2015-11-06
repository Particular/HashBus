namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using Commands;
    using NServiceBus;

    public class StartCatchUpHandler : IHandleMessages<StartCatchUp>
    {
        public void Handle(StartCatchUp message)
        {
            // TODO: call twitter API and get the latest tweets since the provided TweetId
            Console.WriteLine("==================             ====================");

            Console.WriteLine(" TODO: Starting to catchup form tweetId: {0}", message.TweetId);
        }
    }
}
