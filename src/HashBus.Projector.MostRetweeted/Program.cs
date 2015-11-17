namespace HashBus.Projector.MostRetweeted
{
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];

            App.Run(mongoConnectionString, mongoDBDatabase);
        }
    }
}
