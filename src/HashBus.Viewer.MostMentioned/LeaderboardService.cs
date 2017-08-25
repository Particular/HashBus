namespace HashBus.Viewer
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Runtime.Remoting;
    using System.Threading.Tasks;
    using LiteGuard;
    using Newtonsoft.Json;
    using RestSharp;

    class LeaderboardService<TEntry> : IService<string, WebApi.Leaderboard<TEntry>>
    {
        private readonly IRestClient client;
        private readonly string resource;

        public LeaderboardService(IRestClient client, string resource)
        {
            Guard.AgainstNullArgument(nameof(client), client);
            Guard.AgainstNullArgument(nameof(resource), resource);

            this.client = client;
            this.resource = resource;
        }

        public async Task<WebApi.Leaderboard<TEntry>> GetAsync(string key)
        {
            // see https://github.com/NancyFx/Nancy/issues/1154
            key = Uri.EscapeDataString(key.Replace("#", "해시"));
            var request = new RestRequest(string.Format(CultureInfo.InvariantCulture, this.resource, key));
            var response = await this.client.ExecuteGetTaskAsync(request);

            if (response.StatusCode == 0)
            {
                throw new ServerException(response.ErrorMessage, response.ErrorException);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"The server returned {(int)response.StatusCode} ({response.StatusCode}).");
            }

            return JsonConvert.DeserializeObject<WebApi.Leaderboard<TEntry>>(response.Content);
        }
    }
}
