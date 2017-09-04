using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Reflection;

namespace MiniAbp.Modules
{
    internal class ModuleFinder
    {
        private readonly ITypeFinder _typeFinder;

        public ModuleFinder(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public ICollection<Type> FindAll()
        {
            return _typeFinder.Find(MabpModule.IsAbpModule).ToList();
        }
    }
}
