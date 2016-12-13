using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Contract.Permission
{
    /// <summary>
    /// permission control group is a group permission checking minimal unit.
    /// </summary>
    public interface IPermissionControlGroupEntity  
    {
        string Id { get; set; }
        /// <summary>
        /// multiple value stored in this field. separeted by ,
        /// </summary>
        string Name { get; set; }
    }
}
