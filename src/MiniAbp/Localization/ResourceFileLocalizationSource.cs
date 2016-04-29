using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniAbp.Localization
{
    public class ResourceFileLocalizationSource
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Reference to the <see cref="ResourceManager"/> object related to this localization source.
        /// </summary>
        public ResourceManager ResourceManager { get; private set; }

        /// <param name="name">Unique Name of the source</param>
        /// <param name="resourceManager">Reference to the <see cref="ResourceManager"/> object related to this localization source</param>
        public ResourceFileLocalizationSource(string name, ResourceManager resourceManager)
        {
            Name = name;
            ResourceManager = resourceManager;
        }

        /// <summary>
        /// This method is called by ABP before first usage.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// Gets localized string for given name in current language.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Localized string</returns>
        public virtual string GetString(string name)
        {
            return ResourceManager.GetString(name);
        }

        /// <summary>
        /// Gets localized string for given name and specified culture.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        public virtual string GetString(string name, CultureInfo culture)
        {
            return ResourceManager.GetString(name, culture);
        }

        /// <summary>
        /// Gets all strings in current language.
        /// </summary>
        public virtual IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Gets all strings in specified culture.
        /// </summary>
        public virtual IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture)
        {
            return ResourceManager
                .GetResourceSet(culture, true, true) 
                .Cast<DictionaryEntry>()
                .Select(entry => new LocalizedString(entry.Key.ToString(), entry.Value.ToString(), culture))
                .ToList();
        }
    }
}
