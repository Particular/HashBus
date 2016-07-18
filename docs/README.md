# Set up HashBus for an event
The purpose of this document is to describe how to set up HashBus to monitor tweets and display leaderboard.

## Projects
Below is a brief description of existing executables

### HashBus.Twitter.Monitor
This project uses tweeter streaming API to receive notifications whenever a tweet with given hashtag was written. When it happens it create `TweetReceived` and publish it. At the same time it send a command `AnalyzeTweet` to `HashBus.Application`.

### HashBus.Twitter.Monitor.CatchUp
This project uses tweeter pull API to catch up when the `HashBus.Twitter.Monitor` was down. It subscribes for `TweetReceived` event and analyze SessionId that is included in that event. When the SessionId changes it means that `HashBus.Twitter.Monitor` was down. In which case it pulls all the tweets that have happened during the down period and for each one of them sends `AnalyzeTweet` command.

### HashBus.Application
This project handles `AnalyzeTweet` commands removing 'bad data' from the tweets, such as:
 - retweeting your own tweets
 - multiple mentions
 - multiple use of the same hashtag

After analying tweet it publish `TweetAnalyzed` event.

### HashBus.Projector.*
All those project subscribe to `TweetAnalyzed` events and project report that is saved into MongoDB. We have 1 projector for corresponding report:
 - Top Tweeters
 - Top Retweeters
 - Top Tweeters/Retweeters
 - Most Mentioned
 - Most Mentioned
 - Most Retweeted

### HashBus.WebApi
The WebApi pulls the data from MongoDb and exposes it using WebApi.

### HashBus.Viewer.*
This group of projects consume WebApi and display corresponding report on the console window. Viewers use pooling to refresh it's state.

### HashBus.Twitter.BackFill
This project is used to initiate `HashBus.Twitter.Monitor.CatchUp`. Using configuration one can set starting from which tweet the tweets should be analyzed and after running the application the tweets will be analyzed.

## Configuration

There are few settings that should be set up to configure HashBus properly:

### HashBus.Twitter.Monitor App.config
There is the following setting: `<add key="Track" value="#Microsoft" />`. This should be set up to a hashtag for which the leaderboards should be calculated. It is important to note that after changing this value the tweets will be analyzed so the time to set this value should be well thought.

### HashBus.Projector.* App.config
This config file holds address to MongoDb instance that should be set up correctly.

### HashBus.WebApi App.config
This config file holds address to MongoDb instance that should be set up correctly as well as `BaseUrl`. In the configuration file there is a key called: `<add key="IgnoredUserNames" value="adamralph,andreasohlund" />` which should be used to remove the given twitter handles from reports. This filtering should be used for the following cases:
 - Particular employees
 - Bots and fake accounts

### HashBus.Viewer.* App.config
This configuration file holds:
 - address to WebApi that will be consumed
 - hashtag for which the data will be asked from WebApi 
