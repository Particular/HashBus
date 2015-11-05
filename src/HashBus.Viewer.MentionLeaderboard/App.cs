﻿using System.Threading.Tasks;
using RestSharp;

namespace HashBus.Viewer.MentionLeaderboard
{
    class App
    {
        public static async Task RunAsync(
            string webApiBaseUrl, string hashtag, int refreshInterval, bool showPercentages)
        {
            var client = new RestClient(webApiBaseUrl);

            await MentionLeaderboardView.StartAsync(
                hashtag,
                refreshInterval,
                new MentionLeaderboardService(client), 
                showPercentages);
        }
    }
}
