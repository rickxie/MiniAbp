using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CSharp;
using MiniAbp.Configuration;
using MiniAbp.Dependency;
using System.Reflection;

namespace MiniAbp.Compile
{
    /// <summary>
    /// 动态编译类
    /// </summary>
    public class RuntimeDll : IDisposable
    {
        readonly AppDomainSetup _domainSetup = new AppDomainSetup();

        /// <summary>
        /// 受编译代码的AppDomain
        /// </summary>
        protected AppDomain ObjAppDomain;
        private readonly string _dllName;
        private readonly string _targetPath;
        private bool IsIntialized { get; set; }
        public string CsharpCode { get; set; }
        public string DllPath => _targetPath + _dllName;
        public RuntimeDll(string dllName, string targetPath)
        {
            this._dllName = dllName;
            this._targetPath = targetPath;
        }

        public bool Compile(string souceCode,out string errorStr, string[] referenceDllList = null)
        {
            IsIntialized = false;
            CsharpCode = souceCode;
            //1 Create Application Domain 
            _domainSetup.ApplicationBase = _targetPath;
            ObjAppDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString("N"), null, _domainSetup);
            return Compile(out errorStr, referenceDllList);
        }

        private bool Compile(out string compileResult, params string[] referenceDllList)
        {
            //2 Create a new CSharpCodePrivodewr instance
            CSharpCodeProvider cSharpProvider = new CSharpCodeProvider();
            //3 Sets the runtime compiling parameters by creating a new compilerParameters instance
            CompilerParameters parameters = new CompilerParameters();
            referenceDllList?.ToList().ForEach(r=> parameters.ReferencedAssemblies.Add(r));

            // Set compiler argument to optimize output.
            parameters.CompilerOptions = "/optimize";
            //If the value of this property is false, a DLL will be generated. By default, the value of this property is false.
            //parameters.GenerateExecutable = true;
            //Load the remote loader interface
            //parameters.ReferencedAssemblies.Add("MiniAbp.DynamicCompile.dll");
            //Load the resulting assembly into memory
            parameters.GenerateInMemory = true;
            parameters.OutputAssembly = _dllName;
            //4 Compile
            CompilerResults cResult = cSharpProvider.CompileAssemblyFromSource(parameters, CsharpCode);
            if (cResult.Errors.HasErrors)
            {
                string strErrorMsg = cResult.Errors.Count.ToString() + " Errors:";

                for (int x = 0; x < cResult.Errors.Count; x++)
                {
                    strErrorMsg = strErrorMsg + "\r\nLine: " +
                                  cResult.Errors[x].Line.ToString() + " - " +
                                  cResult.Errors[x].ErrorText;
                }

                compileResult = "There were build erros, please modify your code. " + strErrorMsg;
                return false;
            }
            compileResult = "Compile Success!";
            return true;
        }

        private static object _initObjLock = new object();
        /// <summary>
        /// Call Method in the class
        /// </summary>
        /// <param name="classNameSpace"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Invoke(string classNameSpace, string method, object[] param)
        {
            RemoteLoaderFactory factory =
                (RemoteLoaderFactory)
                    ObjAppDomain.CreateInstance("MiniAbp", "MiniAbp.Compile.RemoteLoaderFactory")
                        .Unwrap();
            object objObject = factory.Create(_dllName, classNameSpace, null);
           
            if (objObject == null)
            {
                throw new ArgumentNullException("Error: " + "Couldn't load class.");
            }
            IRemoteInterface objRemote = (IRemoteInterface) objObject;
            //Initialize db configuration
            if (!IsIntialized)
            {
                lock (_initObjLock)
                {
                    if (!IsIntialized)
                    {
                        var dbSetting = IocManager.Instance.Resolve<DatabaseConfiguration>();
                        objRemote.Initialize(dbSetting.ConnectionString, dbSetting.Dialect);
                        IsIntialized = true;
                    }
                }
            }
            return objRemote.Invoke(method, param);
        }

        /// <summary>
        /// Release memory
        /// </summary>
        public void Dispose()
        {
            if (ObjAppDomain != null && !ObjAppDomain.IsFinalizingForUnload())
            {
                AppDomain.Unload(ObjAppDomain);
            }
            if (File.Exists(DllPath))
                File.Delete(DllPath);
        }
    }
}
