namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using Commands;
    using Events;
    using NServiceBus.Saga;

    public class HashtagTweetedSaga : Saga<HashtagTweetedSagaData>,
        IAmStartedByMessages<RegisterMonitor>,
        IAmStartedByMessages<HashtagTweeted>
    {
        public void Handle(HashtagTweeted message)
        {
            Data.HashtagMonitored = message.Hashtag;
            Data.EndpointName = message.EndpointName;
            Data.LatestTweetedId = message.TweetId;
        }

        public void Handle(RegisterMonitor message)
        {
            Data.HashtagMonitored = message.HashtagMonitored;
            Data.EndpointName = message.EndpointName;

            if (Data.LatestTweetedId.HasValue)
            {
                Console.WriteLine("==================             ====================");
                Console.WriteLine("Handling RegisterMonitor message: EndpointName: {0}, HashtagMonitored: {1} ", message.EndpointName, message.HashtagMonitored);

                Bus.Send(new StartCatchUp
                {
                    TweetId = Data.LatestTweetedId.Value
                });
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<HashtagTweetedSagaData> mapper)
        {
            // hashtag can be the unique key
            // mapper.ConfigureMapping<HashtagTweeted>(m => m.EndpointName).ToSaga(s => s.EndpointName);
            //  mapper.ConfigureMapping<RegisterMonitor>(m => m.EndpointName).ToSaga(s => s.EndpointName);
        }
    }
}