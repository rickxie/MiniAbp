using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Configuration
{
    public class CustomConfiguration
    {
        private Dictionary<string, object> Settings { get; set; }

        public CustomConfiguration()
        {
            Settings = new Dictionary<string, object>();
        }
        public void Set<T>(string key, T value) where T : class
        {
            if (Settings.ContainsKey(key))
                Settings[key] = value;
            else
            {
                Settings.Add(key, value);
            }
        }

        public T Get<T>(string key) where T : class 
        {
            if (Settings.ContainsKey(key))
            {
                return Settings[key] as T;
            }
            else
            {
                return null;
            }
        }
    }
}
