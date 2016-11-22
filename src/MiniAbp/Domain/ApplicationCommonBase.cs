using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Extension;
using MiniAbp.Localization;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public abstract class ApplicationCommonBase
    {
        public ISession Session { get; set; }
        protected virtual IDbConnection DbConnection { get; set; }
        protected virtual IDbTransaction DbTransaction { get; set; }
        public LocalizationManager Localization { get; set; }
        
        /// <summary>
        /// Get local string from localiztion resource
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string L(string name)
        {
            if (!LocalizationSource.ContainsKey(name))
            {
                throw new NullReferenceException("{0} not fund in localization dictionary".Fill(name));
            }
            return LocalizationSource[name];
        }

        /// <summary>
        /// Get local string from localiztion resource
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args">parameter of str</param>
        /// <returns></returns>
        public string L(string name, params object[] args)
        {
            if (LocalizationSource.ContainsKey(name))
            {
                throw new NullReferenceException("{0} not fund in localization dictionary".Fill(name));
            }
            return string.Format(LocalizationSource[name], args);
        }

        /// <summary>
        /// resouce name of service
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Gets localization source.
        /// It's valid if <see cref="LocalizationSourceName"/> is set.
        /// </summary>
        protected IDictionary<string, string> LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new Exception("Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                return Localization.GetResouce(LocalizationSourceName, Session.LanguageCulture);

            }
        }
    }
}
