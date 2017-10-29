using MiniAbp.Dependency;
using MiniAbp.Extension;
using MiniAbp.Runtime;

namespace MiniAbp.Domain.Entities
{
    /// <summary>
    /// base class for input parameters
    /// </summary>
    public class InputBase
    {
        private string _userId;
        public string _UserId
        {
            get
            {
                if (_userId.IsEmpty())
                {
                    return IocManager.Instance.Resolve<ISession>().UserId;
                }
                return _userId;
            }
            set { _userId = value; }
        }


        private string _language;
        public string _LanguageCulture
        {
            get
            {
                if (_language.IsEmpty())
                {
                    return IocManager.Instance.Resolve<ISession>().LanguageCulture;
                }
                return _language;
            }
            set { _language = value; }
        }
    }
}
