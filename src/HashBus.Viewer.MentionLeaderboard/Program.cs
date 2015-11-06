using System.Configuration;

namespace HashBus.Viewer.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            var webApiBaseUrl = ConfigurationManager.AppSettings["WebApiBaseUrl"];
            var track = ConfigurationManager.AppSettings["Track"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);
            var showPercentages = bool.Parse(ConfigurationManager.AppSettings["ShowPercentages"]);

            App.RunAsync(webApiBaseUrl, track, refreshInterval, showPercentages).GetAwaiter().GetResult();
        }
    }
}
