using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Runtime
{
    public interface ISession
    {
        string UserId { get;  }
        string LanguageCulture { get;  }
    }
}
