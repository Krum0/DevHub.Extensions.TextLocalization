using DevHub.Extensions.Core.TextLocalization.Abstraction;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHub.Extensions.Core.TextLocalization.Services
{
    public class TextLocalizator<T> : ITextLocalizator<T> where T : new()
    {
        private readonly IDistributedCache store;
        private readonly string path;
        public TextLocalizator(IDistributedCache store)
        {
            this.store = store;
            var cultureKey = CultureInfo.CurrentCulture.Name;
            this.path =  Path.Combine(this.Configuration<T>().DefaultPath,$"{cultureKey}.json");
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
        private T GetNode()
        {
            var nodeKey = typeof(T).Name;
            var source = this.GetSourceFromCache();
            var node = source.SelectToken(nodeKey);
            if (node == null)
            {
                node = JToken.FromObject(new T());
                source.Add(nodeKey, node);
                this.SaveSourceToFile(source.ToString());
            }
            return node.ToObject<T>();
        }

        public string this[string key] => this.Text.GetType()
                                              .GetProperty(key)
                                              .GetValue(this.Text)?
                                              .ToString() ??
                                              $"Not set default value for {typeof(T).Name}:{key}";
        public T Text => GetNode();
    }
}
