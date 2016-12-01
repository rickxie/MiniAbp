using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Contract.Permission
{
    public interface IPermissionGroupControlEntity  
    {
        string Id { get; set; }
        /// <summary>
        /// Control group for permission check
        /// </summary>
        string ControlGroupId { get; set; }

        /// <summary>
        /// user, job, role, group and so on...
        /// </summary>
        int Type { get; set; }

        /// <summary>
        /// multiple value stored in this field. separeted by ,
        /// </summary>
        string Value { get; set; }
    }
}
