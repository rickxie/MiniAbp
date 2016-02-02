using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniAbp.Reflection
{
    public class YAssemblyCollection
    {
        public static List<Assembly> AssemblyCollection = new List<Assembly>();
        public static List<Type> Types = new List<Type>();
        public YAssemblyCollection()
        {
            
        }

        public static void Initialize()
        {
            foreach (var assembly in AssemblyCollection)
            {
                Types.AddRange(assembly.GetTypes());
            }
        }

        public static void Add(Assembly asse)
        {
            AssemblyCollection.Add(asse);
        }

        public static object CreateInstance(string fullName)
        {
            return GetType(fullName).Assembly.CreateInstance(fullName);
        }

        public static Type GetType(string fullName)
        {
            return Types.FirstOrDefault(r => r.FullName == fullName);
        }
        public static Type FindType(string fullName)
        {
            return Types.FirstOrDefault(r => r.FullName == fullName);
        }
    }
}
