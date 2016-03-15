using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniAbp.Domain;

namespace MiniAbp.Reflection
{
    public class YAssembly
    {
        public static List<Type> Types;
        public static List<Type> ServiceTypes;
        public static List<Type> RepositoryTypes;
        public static void Regist(Assembly assembly)
        {
            YAssemblyCollection.Add(assembly);
        }

        public static void Initialize()
        {
            YAssemblyCollection.Add(Assembly.GetExecutingAssembly());
            YAssemblyCollection.Initialize();
            LoadAllTypes();
        }

        public static void LoadAllTypes()
        {
            ServiceTypes = YAssemblyCollection.Types.Where(r => 
            (r.Name.ToUpper().EndsWith("SV") || r.Name.ToUpper().EndsWith("SERVICE")) 
            && r.IsSubclassOf(typeof(BaseService))).ToList();
            RepositoryTypes = YAssemblyCollection.Types.Where(r => 
            (r.Name.ToUpper().EndsWith("RP") || r.Name.ToUpper().EndsWith("REPOSITORY")) 
            && r.IsSubclassOf(typeof(BaseRepository<,>))).ToList();
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
            return ServiceTypes.FirstOrDefault(r => r.Name.ToUpper().Contains(typeName));
        }
        public static Type FindRepositoryType(string typeName)
        {
            return RepositoryTypes.FirstOrDefault(r => r.Name.ToUpper().Contains(typeName));
        }
        public static MethodInfo GetMethodByType(Type type, string methodName)
        {
            var methods = type.GetMethods();

            return methods.FirstOrDefault(r => r.Name.ToUpper().Contains(methodName));
        }
    }
}
