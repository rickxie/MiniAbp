using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Logging;

namespace MiniAbp.Configuration
{
    public class AuditConfiguration
    {
        public Action<AuditInfo> Save = info => { };
        public AuditBehaviours Behaviours { get; set; }

        public AuditConfiguration()
        {
            Behaviours = AuditBehaviours.None;
        }
    }

    public enum AuditBehaviours
    {
        /// <summary>
        /// 全部记录
        /// </summary>
        All,
        /// <summary>
        /// 只记录异常信息
        /// </summary>
        ExceptionOnly,
        /// <summary>
        /// 不记录
        /// </summary>
        None
    }
}
