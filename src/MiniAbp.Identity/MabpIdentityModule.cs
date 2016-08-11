using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Reflection;

namespace MiniAbp.Identity
{
   public class MabpIdentityModule : MabpModule
    {
       public override void Initialize()
       {
           IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
       }
    }
}
