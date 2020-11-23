namespace DevHub.Extensions.Core.TextLocalization
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Distributed;
    
    using Newtonsoft.Json.Linq;

    // Summary:
    //     Represents a type used to perform Localized texts.
    //
    // Remarks:
    //     Aggregates most localized texts to a single method.
    public class TextLocalizator<TLanguage> : ITextLocalizator<TLanguage> where TLanguage : new()
    {
        private readonly IDistributedCache store;
        private readonly string path;

        public TextLocalizator(IDistributedCache store)
        {
            var cultureKey = CultureInfo.CurrentCulture.Name;
            this.store = store;
            this.path =  Path.Combine(this.Configuration<TLanguage>().DefaultPath,$"{cultureKey}.json");
        }

        private async Task<string> GetSourceFromFile()
        {
            var source = "{}";
            if (File.Exists(path))
            {
                source = File.ReadAllText(path, Encoding.UTF8);
            }
            else
            {
               await this.SaveSourceToFile(source);
            }
            return source;
        }
        private async Task SaveSourceToFile(string source)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            await File.WriteAllTextAsync(path, source, Encoding.UTF8);
        }
        private async Task<JObject> GetSourceFromCache()
        {
            var cultureKey = CultureInfo.CurrentCulture.Name;
            var source = await store.GetStringAsync(cultureKey);
            if (source == null)
            {
                return await this.SetSourceToCache();
            }
            return JObject.Parse(source);
        }
        private async Task<JObject> SetSourceToCache()
        {
            var cultureKey = CultureInfo.CurrentCulture.Name;
            var sourceFormFile = await GetSourceFromFile();
            await store.SetStringAsync(cultureKey, sourceFormFile);
            var source = await store.GetStringAsync(cultureKey);
            return JObject.Parse(source);
        }
        private async Task<TLanguage> GetNode()
        {
            var nodeKey = typeof(TLanguage).Name;
            var source = await this.GetSourceFromCache();
            var node =  source.SelectToken(nodeKey);
            if (node == null)
            {
                node = JToken.FromObject(new TLanguage());
                source.Add(nodeKey, node);
                await this.SaveSourceToFile(source.ToString());
                source = await this.SetSourceToCache();
                node = source.SelectToken(nodeKey);
            }
            return node.ToObject<TLanguage>();
        }

        /// <summary>
        /// Return Localized value by name.
        /// </summary>
        /// <param name="key">Name of property</param>
        /// <returns>Localized value</returns>
        public string this[string key] => this.Text.GetType()
                                              .GetProperty(key)
                                              .GetValue(this.Text)?
                                              .ToString() ??
                                              $"Not set default value for {typeof(TLanguage).Name}:{key}";

        /// <summary>
        /// TLanguage class object
        /// </summary>
        public TLanguage Text => GetNode().Result;
    }
}
