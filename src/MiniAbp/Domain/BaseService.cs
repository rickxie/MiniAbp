using System;
using System.Collections.Generic;
using System.Data;
using MiniAbp.Extension;
using MiniAbp.Localization;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public abstract class BaseService
    {
        protected YSession Session => YSession.GetInstance();
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }
        private LocalizationManager Localization { get; set; }
        public BaseService()
        {
        }

        /// <summary>
        /// Get local string from localiztion resource
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string L(string name)
        {
            if (LocalizationSource.ContainsKey(name))
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
