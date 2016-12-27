using System;
using System.Diagnostics;
using MiniAbp.Configuration;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Extension;
using MiniAbp.Logging;
using MiniAbp.Runtime;

namespace MiniAbp.Web.Auditing
{
    public class AuditingManager: ITransientDependency
    {
        public ISession Session { get; set; }
        public AuditInfo _auditInfo;
        private WebAuditInfoProvider provider;
        private Stopwatch sp;
        private AuditConfiguration auditSetting;
        public AuditingManager()
        {
            sp = Stopwatch.StartNew();
            Session = NullSession.GetInstance();
            auditSetting = IocManager.Instance.Resolve<AuditConfiguration>();
            provider = new WebAuditInfoProvider();
        }
        /// <summary>
        /// 开始记录时间
        /// </summary>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        public void Start(string service, string method, string param)
        {
            //如果为None则无需记录
            if (auditSetting.Behaviours == AuditBehaviours.None)
            {
                _auditInfo = new AuditInfo();
                return;
            }
            sp.Start();
            _auditInfo = new AuditInfo
            { 
                UserId = Session.UserId,
                ServiceName = service,
                MethodName = method,
                RequestJson =  param,
                ExecutionTime = DateTime.Now
            };
            provider.Fill(_auditInfo);
        }
        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="ex"></param>
        public void Exception(string ex)
        {
            _auditInfo.Exception = ex;
        }

        public void Stop(string responseStr)
        {
            if (auditSetting.Behaviours == AuditBehaviours.None)
            {
                return;
            }
            if(auditSetting.Behaviours == AuditBehaviours.ExceptionOnly && _auditInfo.Exception.IsEmpty())
            {
                return;
            }
            sp.Stop();
            _auditInfo.Duration = Convert.ToInt32(sp.Elapsed.TotalMilliseconds);
            _auditInfo.ResponseJson = responseStr;
            auditSetting.Save(_auditInfo);
        }
    }
}
