using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Logging;

namespace MiniAbp.Configuration
{
    public class AuditConfiguration
    {
        public Action<AuditInfo> Save = info => { };
    }
}
