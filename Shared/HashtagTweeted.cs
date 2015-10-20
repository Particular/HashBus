using System;
using NServiceBus;

public class HashtagTweeted : IEvent
{
    public Guid Id { get; set; }
}
