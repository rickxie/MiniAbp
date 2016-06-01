using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Extension;

namespace MiniAbp.Localization
{
    public class LocalizationSource
    {
        public string Path { get; set; }
        public string Source { get; set; }

        public LocalizationSource(string source, string path)
        {
            if (source.IsEmpty() || path.IsEmpty())
            {
                throw new ArgumentException("source name or path can't be empty");
            }
            this.Path = path;
            this.Source = source;
        }
    }
}
