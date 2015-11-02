using System.IO;
using System.Threading.Tasks;
using HashBus.Projection;

namespace HashBus.View.MentionLeaderboard
{
    class App
    {
        public static async Task RunAsync(string dataFolder, string hashtag, int refreshInterval)
        {
            await View.StartAsync(
                hashtag,
                refreshInterval,
                new FileListRepository<Mention>(Path.Combine(dataFolder, "MentionLeaderboardProjection.Mentions")));
        }
    }
}
