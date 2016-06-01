using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy.Internal;
using Castle.MicroKernel.Registration;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Extension;

namespace MiniAbp.Reflection
{
    public class YAssembly
    {
        public static List<Type> Types;
        public static List<Type> ServiceTypes;
        public static List<Type> RepositoryTypes;
        public static Dictionary<Type, Type> ServiceDic; 
        public static void Regist(Assembly assembly)
        {
            YAssemblyCollection.Add(assembly);
        }

        public static void Initialize()
        {
            ServiceDic = new Dictionary<Type, Type>();
            YAssemblyCollection.Add(Assembly.GetExecutingAssembly());
            YAssemblyCollection.Initialize();
            LoadAllTypes();
            IocManager.Instance.IocContainer.Register(Classes.From(YAssembly.ServiceTypes).BasedOn<IApplicationService>().LifestyleTransient());
            IocManager.Instance.IocContainer.Register(Classes.From(YAssembly.RepositoryTypes).BasedOn<IRepository>().LifestyleTransient());
        }

        private static void LoadAllTypes()
        {
            ServiceTypes = YAssemblyCollection.Types.Where(r => 
            (r.Name.ToUpper().EndsWith("SV") || r.Name.ToUpper().EndsWith("SERVICE")) && r.IsClass && !r.IsAbstract
            && typeof(IApplicationService).IsAssignableFrom(r)).ToList()  ;
            RepositoryTypes = YAssemblyCollection.Types.Where(r => 
            (r.Name.ToUpper().EndsWith("RP") || r.Name.ToUpper().EndsWith("REPOSITORY")) && r.IsClass && !r.IsAbstract
            && typeof(IRepository).IsAssignableFrom(r)).ToList();

            var exceptServiceType = ServiceTypes.FindAll(r => r.Name == "IApplicationService" || r.Name == "BaseService" || r.Name == "ApplicationService" );
            exceptServiceType.ForEach(r=> ServiceTypes.Remove(r));
            var exceptRpType = RepositoryTypes.FindAll(r => r.Name == "BaseRepository" || r.Name == "IRepository");
            exceptRpType.ForEach(r=> RepositoryTypes.Remove(r));
            ServiceTypes = ServiceTypes.Distinct().ToList();
            RepositoryTypes = RepositoryTypes.Distinct().ToList();
            //Regist all interface for service
            ServiceTypes.ForEach(r =>
            {
                var faces = r.GetAllInterfaces();
                var it = faces.Where(r1 => typeof (IApplicationService).IsAssignableFrom(r1) && r1 != typeof(IApplicationService)).ToList();
               
                if (it.Count == 1)
                {
                    ServiceDic.Add(r, it.First());
                }
                else if (it.Count == 0)
                {
                    throw new Exception("Service '{0}' must inherit from one interface which inherit from IApplicationService".Fill(r.FullName));
                }
                else if(it.Count > 1)
                {
                    var itStr = string.Join(",", it.Select(r2 => r2.Name));
                    throw new Exception("Service '{0}' can't have multi interface which inherit from IApplicationService, which is {1}".Fill(r.FullName, itStr));
                }
            });
        }

        public static object CreateInstance(string fullName)
        {
            return YAssemblyCollection.CreateInstance(fullName);
        }
        public static Type GetType(string fullName)
        {
            return YAssemblyCollection.GetType(fullName);
        }

        public static Type FindServiceType(string typeName)
        {
            
            return ServiceTypes.FirstOrDefault(r => (r.Name).ToUpper() == typeName + "SV" || (r.Name ).ToUpper() == typeName + "SERVICE");
        }
        public static Type FindRepositoryType(string typeName)
        {
            return RepositoryTypes.FirstOrDefault(r => r.Name.ToUpper().Contains(typeName));
        }
        public static MethodInfo GetMethodByType(Type type, string methodName)
        {
            var methods = type.GetMethods();
            return methods.FirstOrDefault(r => r.Name.ToUpper().Equals(methodName));
        }
    }
}
