using System;
using System.Diagnostics;
using MiniAbp.Configuration;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Logging;
using MiniAbp.Runtime;

namespace MiniAbp.Web.Auditing
{
    public class AuditingManager: ISingletonDependency
    {
        private readonly YSession _session;
        private AuditInfo _auditInfo;
        private WebAuditInfoProvider provider;
        private Stopwatch sp;
        public AuditingManager()
        {
            sp = Stopwatch.StartNew();
            _session = YSession.GetInstance();
            provider = new WebAuditInfoProvider();
        }
        //SaveLog to DB;
        public void Start(string service, string method, string param)
        {
            sp.Start();
            _auditInfo = new AuditInfo
            { 
                UserId = _session.UserId,
                ServiceName = service,
                MethodName = method,
                RequestJson =  param,
                ExecutionTime = DateTime.Now
            };
            provider.Fill(_auditInfo);
        }

        public void Exception(string ex)
        {
            _auditInfo.Exception = ex;
        }
        public void Stop(string responseStr)
        {
            sp.Stop();
            _auditInfo.Duration = Convert.ToInt32(sp.Elapsed.TotalMilliseconds);
            _auditInfo.ResponseJson = responseStr;
            var auditSetting = IocManager.Instance.Resolve<AuditConfiguration>();
            auditSetting.Save(_auditInfo);
        }
    }
}
