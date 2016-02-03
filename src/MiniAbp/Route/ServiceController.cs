using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Logging;
using MiniAbp.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Yooya.Bpm.Framework;
using Yooya.Bpm.Framework.Domain.Entity;
using Yooya.Bpm.Framework.Route;

namespace MiniAbp.Route
{
    public class ServiceController : IDisposable
    {
        private const string ExceptionOfConnectionStringIsNull = "Connection String is Empty.";
        private const string ExceptionOfOneMethodOnly = "Service Method only one Parameter can be defined.";

        private ILogger Logger = IocManager.Instance.Resolve<ILogger>();
        private ServiceController()
        {
            if (string.IsNullOrWhiteSpace(DbDapper.ConnectionString))
            {
                throw new ArgumentNullException(ExceptionOfConnectionStringIsNull);
            }
        }
        public static ServiceController Instance => new ServiceController();

        public object Execute(string serviceName, string methodName, object param, RequestType requestType)
        {
            serviceName = serviceName.ToUpper();
            methodName = methodName.ToUpper();
            var type = YAssembly.FindServiceType(serviceName);
            var method = YAssembly.GetMethodByType(type, methodName);
            object result = null;
            var dbConnection = DbDapper.NewDbConnection;
            dbConnection.Open();
            var dbTransaction = dbConnection.BeginTransaction();
            try
            {
                var instance = YAssembly.CreateInstance(type.FullName);
                type.GetProperty("DbConnection").SetValue(instance, dbConnection, null);
                type.GetProperty("DbTransaction").SetValue(instance, dbTransaction, null);
                InitializeRepositorys(type, instance, dbConnection, dbTransaction);
                result = Invoke(method, instance, param);
                dbTransaction.Commit();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof (UserFriendlyException))
                {
                    dbTransaction.Rollback();
                    result = new ExceptionMessage()
                    {
                        Message = ex.InnerException.Message,
                        CallStack = ex.InnerException.StackTrace
                    };
                    Logger.Error(ex.Message, ex.InnerException);
                }
                else
                {
                    var except = ex.InnerException ?? ex;
                    dbTransaction.Rollback();
                    result = new ExceptionMessage()
                    {
                        Message = except.Message,
                        CallStack = except.StackTrace
                    };
                    Logger.Error(ex.Message, except);
                }
            }
            finally
            {
                dbConnection.Close();
            }

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
                return GetStringObject(info, param.ToString());
            }
            else if (param is List<HttpPostedFile>)
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
                var fileInput = instance as FileInput;
                fileInput.Files = param as List<HttpPostedFile>;
                return fileInput;
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
                    typeofMember.GetProperty("DbConnection").SetValue(instanceOfMember, dbConnection, null);
                    typeofMember.GetProperty("DbTransaction").SetValue(instanceOfMember, dbTransaction, null);
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
