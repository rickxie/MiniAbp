using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace MiniAbp.Runtime
{
    public class NullSession : ISession
    {

        private NullSession()
        {
        }

        public static NullSession GetInstance()
        {
            return new NullSession();
        }

        public string UserId => null;

        public string LanguageCulture => null;

    }
}
