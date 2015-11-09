namespace HashBus.Viewer.TweetLeaderboard
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using LiteGuard;
    using Newtonsoft.Json;
    using RestSharp;

    class TweetLeaderboardService : IService<string, WebApi.Leaderboard<WebApi.UserEntry>>
    {
        private readonly IRestClient client;

        public TweetLeaderboardService(IRestClient client)
        {
            Guard.AgainstNullArgument(nameof(client), client);

            this.client = client;
        }

        public async Task<WebApi.Leaderboard<WebApi.UserEntry>> GetAsync(string key)
        {
            // see https://github.com/NancyFx/Nancy/issues/1154
            var request = new RestRequest($"/tweet-leaderboards/{Uri.EscapeDataString(key.Replace("#", "해시"))}");
            var response = await this.client.ExecuteGetTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"The server returned {(int)response.StatusCode} ({response.StatusCode}).");
            }

            return JsonConvert.DeserializeObject<WebApi.Leaderboard<WebApi.UserEntry>>(response.Content);
        }
    }
}
