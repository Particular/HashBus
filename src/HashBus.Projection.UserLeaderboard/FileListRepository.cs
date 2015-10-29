using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HashBus.Projection.UserLeaderboard
{
    public class FileListRepository<TValue> : IRepository<string, IEnumerable<TValue>>
    {
        private readonly string folderName;

        public FileListRepository(string folderName)
        {
            this.folderName = folderName;
        }

        public Task<IEnumerable<TValue>> GetAsync(string key)
        {
            var fileName = Path.Combine(this.folderName, $"{key}.json");
            if (!File.Exists(fileName))
            {
                return Task.FromResult(Enumerable.Empty<TValue>());
            }

            using (var textReader = new StreamReader(fileName))
            {
                return Task.FromResult(
                    new JsonSerializer().Deserialize<IEnumerable<TValue>>(new JsonTextReader(textReader)) ?? Enumerable.Empty<TValue>());
            }
        }

        public Task SaveAsync(string key, IEnumerable<TValue> value)
        {
            var fileName = Path.Combine(this.folderName, $"{key}.json");
            using (var textReader = new StreamWriter(fileName))
            {
                new JsonSerializer().Serialize(new JsonTextWriter(textReader), value);
            }

            return Task.FromResult(0);
        }
    }
}
