using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using MiniAbp.Configuration;
using MiniAbp.Dependency;
using MiniAbp.Domain;

namespace MiniAbp.Reflection
{
    public abstract class MabpModule
    {
        protected internal  IocManager IocManager{ get; internal set; }
        protected internal IStartupConfiguration Configuration { get; internal set; }

        public virtual void PreInitialize()
        {
            
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void PostInitialize()
        {
          
        }

        public virtual void Shutdown()
        {
            
        }

        public static bool IsAbpModule(Type type)
        {
            return
                type.IsClass &&
                !type.IsAbstract &&
                typeof(MabpModule).IsAssignableFrom(type);
        }

        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsAbpModule(moduleType))
            {
                throw new InvalidOperationException("This type is not an Mabp module: " + moduleType.AssemblyQualifiedName);
            }

            var list = new List<Type>();

            if (moduleType.IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }

            return list;
        }
    }
}
