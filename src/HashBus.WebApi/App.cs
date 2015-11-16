namespace HashBus.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using ColoredConsole;
    using HashBus.ReadModel;
    using HashBus.ReadModel.MongoDB;
    using MongoDB.Driver;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Hosting.Self;
    using Nancy.Responses.Negotiation;
    using Nancy.TinyIoc;

    class App
    {
        public static void Run(
            Uri baseUri, string mongoConnectionString, string mongoDBDatabase)
        {
            var bootstrapper = new Bootstrapper(new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase));
            using (var host = new NancyHost(bootstrapper, baseUri))
            {
                host.Start();
                ColorConsole.WriteLine("Web API hosted at ".Gray(), $"{baseUri}".White());
                ColorConsole.Write("Powered by ".DarkGray(), " Nancy ".Black().OnWhite());
                Thread.Sleep(Timeout.Infinite);
            }
        }

        private class Bootstrapper : DefaultNancyBootstrapper
        {
            private readonly IMongoDatabase mongoDatabase;

            public Bootstrapper(IMongoDatabase mongoDatabase)
            {
                this.mongoDatabase = mongoDatabase;
            }

            protected override NancyInternalConfiguration InternalConfiguration
            {
                get
                {
                    return NancyInternalConfiguration.WithOverrides(configuration =>
                        configuration.ResponseProcessors = new[] { typeof(JsonProcessor) });
                }
            }

            protected override void ConfigureApplicationContainer(TinyIoCContainer container)
            {
                base.ConfigureApplicationContainer(container);

                container.Register<IRepository<string, IEnumerable<Mention>>>(
                        new MongoDBListRepository<Mention>(this.mongoDatabase, "mention_leaderboard__mentions"));

                container.Register<IRepository<string, IEnumerable<Tweet>>>(
                        new MongoDBListRepository<Tweet>(this.mongoDatabase, "tweet_leaderboard__tweets"));

                container.Register<IRepository<string, IEnumerable<TweetRetweet>>>(
                        new MongoDBListRepository<TweetRetweet>(this.mongoDatabase, "top_tweeters_retweeters__tweet_retweets"));

                container.Register<IRepository<string, IEnumerable<Retweet>>>(
                        new MongoDBListRepository<Retweet>(this.mongoDatabase, "top_retweeters__retweets"));

                container.Register<IRepository<string, IEnumerable<Retweetee>>>(
                        new MongoDBListRepository<Retweetee>(this.mongoDatabase, "most_retweeted__retweetees"));

                container.Register<IRepository<string, IEnumerable<Hashtag>>>(
                        new MongoDBListRepository<Hashtag>(this.mongoDatabase, "most_hashtagged__hashtags"));
            }
        }
    }
}
