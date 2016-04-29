using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Ado.Dependency;
using MiniAbp.Reflection;

namespace MiniAbp.Ado
{
    public class AdoModule:MabpModule
    {
        private readonly TypeFinder _typeFinder;
        public AdoModule(TypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new AdoConventionalRegisterer());
            base.PreInitialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
