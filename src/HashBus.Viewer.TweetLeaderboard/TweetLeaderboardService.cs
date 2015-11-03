using System;
using System.Net;
using System.Threading.Tasks;
using LiteGuard;
using Newtonsoft.Json;
using RestSharp;

namespace HashBus.Viewer.TweetLeaderboard
{
    class TweetLeaderboardService : IService<string, WebApi.TweetLeaderboard>
    {
        private readonly IRestClient client;

        public TweetLeaderboardService(IRestClient client)
        {
            Guard.AgainstNullArgument(nameof(client), client);

            this.client = client;
        }

        public async Task<WebApi.TweetLeaderboard> GetAsync(string key)
        {
            var request = new RestRequest($"/tweet-leaderboards/{key}");
            var response = await this.client.ExecuteGetTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"The server returned {(int)response.StatusCode} ({response.StatusCode}).");
            }

            return JsonConvert.DeserializeObject<WebApi.TweetLeaderboard>(response.Content);
        }
    }
}
