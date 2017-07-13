namespace HashBus.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            Uri baseUri, string mongoConnectionString, string mongoDBDatabase, IEnumerable<string> ignoredUserNames, IEnumerable<string> ignoredHashtags)
        {
            var bootstrapper = new Bootstrapper(
                new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase),
                new IgnoredUserNamesService(ignoredUserNames),
                new IgnoredHashtagsService(ignoredHashtags));

            var hostConfiguration = new HostConfiguration
            {
                UrlReservations = new UrlReservations { CreateAutomatically = true },
            };

            using (var host = new NancyHost(bootstrapper, hostConfiguration, baseUri))
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
            private readonly IIgnoredUserNamesService ignoredUserNamesService;
            private readonly IIgnoredHashtagsService ignoredHashtagsService;

            public Bootstrapper(IMongoDatabase mongoDatabase, IIgnoredUserNamesService ignoredUserNamesService, IIgnoredHashtagsService ignoredHashtagsService)
            {
                this.mongoDatabase = mongoDatabase;
                this.ignoredUserNamesService = ignoredUserNamesService;
                this.ignoredHashtagsService = ignoredHashtagsService;
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

                container.Register(this.ignoredUserNamesService);
                container.Register(this.ignoredHashtagsService);

                container.Register<IRepository<string, IEnumerable<Mention>>>(
                        new MongoDBListRepository<Mention>(this.mongoDatabase, "most_mentioned__mentions"));

                container.Register<IRepository<string, IEnumerable<Tweet>>>(
                        new MongoDBListRepository<Tweet>(this.mongoDatabase, "top_tweeters__tweets"));

                container.Register<IRepository<string, IEnumerable<TweetRetweet>>>(
                        new MongoDBListRepository<TweetRetweet>(this.mongoDatabase, "top_tweeters_retweeters__tweets_retweets"));

                container.Register<IRepository<string, IEnumerable<Retweet>>>(
                        new MongoDBListRepository<Retweet>(this.mongoDatabase, "top_retweeters__retweets"));

                container.Register<IRepository<string, IEnumerable<Retweetee>>>(
                        new MongoDBListRepository<Retweetee>(this.mongoDatabase, "most_retweeted__retweetees"));

                container.Register<IRepository<string, IEnumerable<Hashtag>>>(
                        new MongoDBListRepository<Hashtag>(this.mongoDatabase, "most_hashtagged__hashtags"));
            }

            protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
            {
                base.RequestStartup(container, pipelines, context);

                pipelines.AfterRequest.AddItemToEndOfPipeline(ctx =>
                {
                    if (ctx.Request.Headers.Keys.Contains("Origin"))
                    {
                        ctx.Response.Headers["Access-Control-Allow-Origin"] =
                            string.Join(" ", ctx.Request.Headers["Origin"]);

                        if (ctx.Request.Method == "OPTIONS")
                        {
                            ctx.Response.Headers["Access-Control-Allow-Methods"] =
                                "GET, POST, PUT, DELETE, OPTIONS";

                            if (ctx.Request.Headers.Keys.Contains("Access-Control-Request-Headers"))
                            {
                                ctx.Response.Headers["Access-Control-Allow-Headers"] =
                                    string.Join(", ", ctx.Request.Headers["Access-Control-Request-Headers"]);
                            }
                        }
                    }
                });
            }
        }
    }
}
