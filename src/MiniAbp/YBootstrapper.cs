using System;
using System.Linq;
using MiniAbp.Configuration;
using MiniAbp.Dependency;
using MiniAbp.Dependency.Installer;
using MiniAbp.Localization;
using MiniAbp.Logging;
using MiniAbp.Modules;
using MiniAbp.Reflection; 

namespace MiniAbp
{
    public class YBootstrapper : IDisposable
    {
        public IocManager IocManager { get; private set; }
        private ModuleManager _moduleManager;

        public YBootstrapper() : this(IocManager.Instance)
        {
        }

        public YBootstrapper(IocManager iocManager)
        {
            IocManager = iocManager;
        }

        public void Initialize()
        {
            IocManager.IocContainer.Install(new CoreInstaller());
            IocManager.Resolve<IStartupConfiguration>().Initialize();
            _moduleManager = IocManager.Resolve<ModuleManager>();
            _moduleManager.InitializeModules();
            PostInitialize();
        }

        public void PostInitialize()
        {
          
        }
        
        public void HandleException(Exception ex)
        {
            var logger = IocManager.Resolve<ILogger>();
            logger.Fatal("运行时发生未被捕获的异常。", ex);
        }

        public void Dispose()
        {
            //TODO:做一些释放资源的操作
        }
    }
}
