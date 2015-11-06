namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using Commands;
    using Events;
    using NServiceBus.Saga;

    public class HashtagTweetedSaga : Saga<HashtagTweetedSagaData>,
        IAmStartedByMessages<TweetReceived>
    {
        public void Handle(TweetReceived message)
        {
            if (Data.PreviousSessionId != Guid.Empty && Data.PreviousSessionId != message.SessionId)
            {
                // reset session id
                Console.WriteLine("==================             ====================");
                Console.WriteLine("Handling HashtagTweeted with new session id message: EndpointName: {0}, Hashtag: {1} ", message.EndpointName, message.Track);

                Bus.Send(new StartCatchUp {TweetId = Data.PreviousTweetId});                
            }

            Data.PreviousSessionId = message.SessionId;
            Data.Hashtag = message.Track;
            Data.EndpointName = message.EndpointName;
            Data.PreviousTweetId = message.TweetId;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<HashtagTweetedSagaData> mapper)
        {
            // hashtag and endpointNmae is our coposit key look at HashtagTweetedSagaFinder.cs
        }
    }
}