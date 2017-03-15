using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using MiniAbp.Authorization;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Domain.Entitys;
using MiniAbp.Extension;
using MiniAbp.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FileInfo = MiniAbp.Domain.Entitys.FileInfo;

namespace MiniAbp.Web.Route
{
    public class ServiceController : IDisposable
    {
        private const string ExceptionOfConnectionStringIsNull = "Connection String is Empty.";
        private const string ExceptionOfOneMethodOnly = "Service Method only one Parameter can be defined.";

        private ServiceController()
        {
            if (string.IsNullOrWhiteSpace(DbDapper.ConnectionString))
            {
                throw new ArgumentNullException(ExceptionOfConnectionStringIsNull);
            }
        }
        public static ServiceController Instance => new ServiceController();

        public object Execute(string serviceName, string methodName, object param)
        {
            serviceName = serviceName.ToUpper();
            methodName = methodName.ToUpper();
            var svType = YAssembly.FindServiceType(serviceName);
            if (svType == null)
            {
                throw new UserFriendlyException("'{0}' Service 不存在".Fill(serviceName.ToLower()));
            }
            var interfaceType = YAssembly.ServiceDic[svType];
            var method = YAssembly.GetMethodByType(interfaceType, methodName);
            object result = null; 
            var instance = IocManager.Instance.Resolve(interfaceType); 
            result = Invoke(method, instance, param);
             
            return result;
        }

        private object Invoke(MethodInfo method, object instance, object param)
        {
            object result;
            var deSerializedParam = GetMethoParam(method, param);
            if (deSerializedParam == null)
            {
                result = method.Invoke(instance, null);
            }
            else
            {
                result = method.Invoke(instance, new[] { deSerializedParam });
            }
            return result;
        }

        public object GetMethoParam(MethodInfo info, object param)
        {
            if (param is string)
            {
                object paraObject = null;
                try
                {
                    paraObject = GetStringObject(info, param.ToString());
                }
                catch (Exception ex)
                {
                    var friendlyEx = new UserFriendlyException("{0} 方法的请求参数出错".Fill(info.ToString()));
                    friendlyEx.InnerException = ex;
                    throw friendlyEx;
                }
                return paraObject;
            }
            else if (param is FileInput)
            {
                return GetFileObject(info, param);
            }           
            throw new UserFriendlyException("请求的参数类型错误， 目前只支持文件和数据类型");
        }


        private object GetStringObject(MethodInfo info, string param)
        {
            var arg = info.GetParameters();
            if (arg.Length == 0 || string.IsNullOrWhiteSpace(param))
            {
                return null;
            }
            if (arg.Length > 1)
            {
                throw new ArgumentException(ExceptionOfOneMethodOnly);
            }
            var type = arg[0].ParameterType.Assembly.GetType(arg[0].ParameterType.FullName);

            var paraObj = JsonConvert.DeserializeObject(param, type, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return paraObj;
        }
        private object GetFileObject(MethodInfo info, object param)
        {
            var arg = info.GetParameters();
            if (arg.Length == 0 || param == null)
            {
                return null;
            }
            if (arg.Length > 1)
            {
                throw new ArgumentException(ExceptionOfOneMethodOnly);
            }
            var type = arg[0].ParameterType.Assembly.GetType(arg[0].ParameterType.FullName);

            var instance = YAssemblyCollection.CreateInstance(type.FullName);
            if (instance is FileInput)
            {
                return param;
            }
            else
            {
                throw new UserFriendlyException("请求错误,请使用FileInput来作为接收参数");
            }
        }
       
       
        public void InitializeRepositorys(Type type, object instance, IDbConnection dbConnection = null,
            IDbTransaction dbTransaction = null)
        {
            var members = GetRepositoryMembers(type);
            if (members != null)
            {
                var properTyMember = members.Where(r => r.MemberType == MemberTypes.Property);
                foreach (var memberInfo in properTyMember)
                {
                    var fieldInfo = ((PropertyInfo) memberInfo);
                    var typeofMember = YAssembly.GetType(fieldInfo.PropertyType.FullName);
                    var instanceOfMember = YAssembly.CreateInstance(fieldInfo.PropertyType.FullName);
                    typeofMember.GetProperty("Connection").SetValue(instanceOfMember, dbConnection, null);
                    typeofMember.GetProperty("Transaction").SetValue(instanceOfMember, dbTransaction, null);
                    type.GetProperty(memberInfo.Name).SetValue(instance, instanceOfMember, null);
                }
            }
        }

        public List<MemberInfo> GetRepositoryMembers(Type type)
        {
            var members = type.GetMembers();
            var repositoryMember = members.Where(memberInfo => memberInfo.Name.Contains("Repository") || memberInfo.Name.Contains("Rp")).ToList();
            return repositoryMember;
        }

        public void Dispose()
        {
           
        }
    }
}
