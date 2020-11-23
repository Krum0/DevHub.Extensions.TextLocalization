namespace DevHub.Extensions.Core.TextLocalization
{
    using System.Globalization;
    using System.IO;
    using System.Text;

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


        private string GetSourceFromFile()
        {
            var source = "{}";
            if (File.Exists(path))
            {
                source = File.ReadAllText(path, Encoding.UTF8);
            }
            else
            {
                this.SaveSourceToFile(source);
            }
            return source;
        }
        private void SaveSourceToFile(string source)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, source, Encoding.UTF8);
        }
        private JObject GetSourceFromCache()
        {
            var cultureKey = CultureInfo.CurrentCulture.Name;
            var source = store.GetString(cultureKey);
            if (source == null)
            {
                var sourceFormFile = GetSourceFromFile();
                store.SetString(cultureKey, sourceFormFile);
                source = store.GetString(cultureKey);
            }
            return JObject.Parse(source);
        }
        private TLanguage GetNode()
        {
            var nodeKey = typeof(TLanguage).Name;
            var source = this.GetSourceFromCache();
            var node = source.SelectToken(nodeKey);
            if (node == null)
            {
                node = JToken.FromObject(new TLanguage());
                source.Add(nodeKey, node);
                this.SaveSourceToFile(source.ToString());
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
        public TLanguage Text => GetNode();
    }
}
