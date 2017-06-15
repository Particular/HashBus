namespace HashBus.WebApi
{
    using System;
    using System.Configuration;
    using System.Linq;

    class Program
    {
        static void Main()
        {
            var baseUri = new Uri(ConfigurationManager.AppSettings["BaseUri"]);
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];
            var ignoredUserNames = ConfigurationManager.AppSettings["IgnoredUserNames"]
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(userName => userName.Trim());

            var ignoredHashtags = ConfigurationManager.AppSettings["IgnoredHashtags"]
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(hashtag => hashtag.Trim());

            App.Run(baseUri, mongoConnectionString, mongoDBDatabase, ignoredUserNames, ignoredHashtags);
        }
    }
}
