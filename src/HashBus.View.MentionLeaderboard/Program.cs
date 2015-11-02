using System.Configuration;

namespace HashBus.View.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            var dataFolder = ConfigurationManager.AppSettings["DataFolder"];
            var hashTag = ConfigurationManager.AppSettings["hashTag"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);
            App.RunAsync(dataFolder, hashTag, refreshInterval).GetAwaiter().GetResult();
        }
    }
}
