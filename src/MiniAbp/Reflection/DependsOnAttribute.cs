using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Reflection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        public Type[] DependedModuleTypes { get; private set; }

        public DependsOnAttribute(params Type[] denpendedModuleTypes)
        {
            DependedModuleTypes = denpendedModuleTypes;
        }
    }
}
