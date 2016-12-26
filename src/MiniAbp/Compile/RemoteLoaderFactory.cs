using System;
using System.Reflection;
using System.Security.Principal;
using MiniAbp.DataAccess;

namespace MiniAbp.Compile
{
    /// <summary>
    /// Interface that can be run over the remote AppDomain boundary.
    /// </summary>
    public interface IRemoteInterface
    {
        object Invoke(string icMethod, object[] parameters);
        void Initialize(string connectionString, Dialect dialect);
    }
    /// <summary>
    /// Factory class to create objects exposing IRemoteInterface
    /// </summary>
    public class RemoteLoaderFactory : MarshalByRefObject
    {
        private const BindingFlags Bfi = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;
        public IRemoteInterface Create(string assemblyFile, string typeName, object[] constructArgs)
        {
            return (IRemoteInterface)
                    Activator.CreateInstanceFrom(assemblyFile, typeName, false, Bfi, null, constructArgs,null,null).Unwrap();
        }
       
    }
}
 