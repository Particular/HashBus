namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using System.Collections.Generic;
    using HashBus.Twitter.Events;

    public interface ITweetReceivedService
    {
        IEnumerable<TweetReceived> Get(string track, long sinceTweetId, string endpointName, Guid sessionId);
    }
}
