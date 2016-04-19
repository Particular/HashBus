namespace HashBus.Twitter.Monitor.CatchUp
{
    using System.Collections.Generic;
    using Tweetinvi.Core.Interfaces;

    public interface ITweetService
    {
        IEnumerable<ITweet> Get(string track, long sinceTweetId);
    }
}
