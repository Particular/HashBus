using System.Configuration;

namespace HashBus.Viewer.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            var webApiBaseUrl = ConfigurationManager.AppSettings["WebApiBaseUrl"];
            var hashTag = ConfigurationManager.AppSettings["hashTag"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);
            var showPercentages = bool.Parse(ConfigurationManager.AppSettings["ShowPercentages"]);

            App.RunAsync(webApiBaseUrl, hashTag, refreshInterval, showPercentages).GetAwaiter().GetResult();
        }
    }
}
