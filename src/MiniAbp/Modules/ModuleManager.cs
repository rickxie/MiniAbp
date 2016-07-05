using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Configuration;
using MiniAbp.Dependency;
using MiniAbp.Reflection;

namespace MiniAbp.Modules
{
    internal class ModuleManager
    {
        private readonly ModuleCollection _modules;

        private readonly IocManager _iocManager;
        private readonly ModuleFinder _moduleFinder;

        public ModuleManager(IocManager iocManager, ModuleFinder moduleFinder)
        {
           _modules = new ModuleCollection();
            _iocManager = iocManager;
            _moduleFinder = moduleFinder;
            //Logger = NullLogger.Instance;
        }

        public virtual void InitializeModules()
        {
            LoadAll();

            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            //Regist all Rp and Sv
            sortedModules.ForEach(module => YAssembly.Regist(module.Assembly));
            YAssembly.Initialize();
            sortedModules.ForEach(module => module.Instance.PostInitialize());

        }

        public virtual void ShutdownModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
        }

        private void LoadAll()
        {
            //Logger.Debug("Loading Abp modules...");

            var moduleTypes = AddMissingDependedModules(_moduleFinder.FindAll());
            //Logger.Debug("Found " + moduleTypes.Count + " ABP modules in total.");

            //Register to IOC container.
            foreach (var moduleType in moduleTypes)
            {
                if (!MabpModule.IsAbpModule(moduleType))
                {
                    throw new NullReferenceException("This type is not an Mabp module: " + moduleType.AssemblyQualifiedName);
                }

                if (!_iocManager.IsRegistered(moduleType))
                {
                    _iocManager.Register(moduleType);
                }
            }

            //Add to module collection
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = (MabpModule)_iocManager.Resolve(moduleType);

                moduleObject.IocManager = _iocManager;
                moduleObject.Configuration = _iocManager.Resolve<IStartupConfiguration>();

                _modules.Add(new ModuleInfo(moduleObject));

                //Logger.DebugFormat("Loaded module: " + moduleType.AssemblyQualifiedName);
            }

            //AbpKernelModule must be the first module
            var startupModuleIndex = _modules.FindIndex(m => m.Type == typeof(KernelModule));
            if (startupModuleIndex > 0)
            {
                var startupModule = _modules[startupModuleIndex];
                _modules.RemoveAt(startupModuleIndex);
                _modules.Insert(0, startupModule);
            }

            SetDependencies();

            //Logger.DebugFormat("{0} modules loaded.", _modules.Count);
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                //Set dependencies according to assembly dependency
                foreach (var referencedAssemblyName in moduleInfo.Assembly.GetReferencedAssemblies())
                {
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);
                    var dependedModuleList = _modules.Where(m => m.Assembly == referencedAssembly).ToList();
                    if (dependedModuleList.Count > 0)
                    {
                        moduleInfo.Dependencies.AddRange(dependedModuleList);
                    }
                }

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in MabpModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new Exception("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }

        private static ICollection<Type> AddMissingDependedModules(ICollection<Type> allModules)
        {
            var initialModules = allModules.ToList();
            foreach (var module in initialModules)
            {
                FillDependedModules(module, allModules);
            }

            return allModules;
        }

        private static void FillDependedModules(Type module, ICollection<Type> allModules)
        {
            foreach (var dependedModule in MabpModule.FindDependedModuleTypes(module))
            {
                if (!allModules.Contains(dependedModule))
                {
                    allModules.Add(dependedModule);
                    FillDependedModules(dependedModule, allModules);
                }
            }
        }
    }
}
