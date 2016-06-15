namespace HashBus.Viewer.MostRetweeted
{
    using System.Threading.Tasks;
    using RestSharp;

    class App
    {
        public static async Task RunAsync(
            string webApiBaseUrl,
            string track,
            int refreshInterval,
            bool showPercentages,
            int verticalPadding,
            int horizontalPadding)
        {
            await MostRetweetedLeaderBoardViewFactory.Create(
                    track, refreshInterval,
                    showPercentages,
                    verticalPadding,
                    horizontalPadding,
                    new RestClient(webApiBaseUrl))
                .RunAsync();
        }
    }
}
