using System;
using System.Threading.Tasks;
using NServiceBus;

public class HashtagTweetedHandler : IHandleMessages<HashtagTweeted>
{
    public Task Handle(HashtagTweeted message)
    {
        Console.WriteLine(@"Handling: HashtagTweeted with Id: {0}", message.Id);
        return Task.FromResult(0);
    }
}
