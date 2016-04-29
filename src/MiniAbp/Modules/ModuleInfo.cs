using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Reflection;

namespace MiniAbp.Modules
{
    internal class ModuleInfo
    {
        /// <summary>
        /// The assembly which contains the module definition.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Type of the module.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Instance of the module.
        /// </summary>
        public MabpModule Instance { get; private set; }

        /// <summary>
        /// All dependent modules of this module.
        /// </summary>
        public List<ModuleInfo> Dependencies { get; private set; }

        /// <summary>
        /// Creates a new AbpModuleInfo object.
        /// </summary>
        /// <param name="instance"></param>
        public ModuleInfo(MabpModule instance)
        {
            Dependencies = new List<ModuleInfo>();
            Type = instance.GetType();
            Instance = instance;
            Assembly = Type.Assembly;
        }

        public override string ToString()
        {
            return string.Format("{0}", Type.AssemblyQualifiedName);
        }
    }
}
