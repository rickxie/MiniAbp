using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Reflection
{
    public interface IAssemblyFinder
    {
        /// <summary>
        /// This method should return all assemblies used by application.
        /// </summary>
        /// <returns>List of assemblies</returns>
        List<Assembly> GetAllAssemblies();
    }
}
