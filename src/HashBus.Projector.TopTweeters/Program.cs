namespace HashBus.Projector.TopTweeters
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            App.Run(mongoConnectionString, mongoDBDatabase);
        }
    }
}
