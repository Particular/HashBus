using System.Threading.Tasks;
using LiteGuard;
using RestSharp;
using Newtonsoft.Json;

namespace HashBus.Viewer.MentionLeaderboard
{
    using System;
    using System.Net;

    class MentionLeaderboardService : IService<string, WebApi.MentionLeaderboard>
    {
        private readonly IRestClient client;

        public MentionLeaderboardService(IRestClient client)
        {
            Guard.AgainstNullArgument(nameof(client), client);

            this.client = client;
        }

        public async Task<WebApi.MentionLeaderboard> GetAsync(string key)
        {
            // see https://github.com/NancyFx/Nancy/issues/1154
            var request = new RestRequest($"/mention-leaderboards/{Uri.EscapeDataString(key.Replace("#", "해시"))}");
            var response = await this.client.ExecuteGetTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"The server returned {(int)response.StatusCode} ({response.StatusCode}).");
            }

            return JsonConvert.DeserializeObject<WebApi.MentionLeaderboard>(response.Content);
        }
    }
}
