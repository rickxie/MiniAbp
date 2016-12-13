using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Domain;

namespace MiniAbp.Contract.Permission
{
    /// <summary>
    /// permission manager contract define the basic function which is necessary.
    /// </summary>
    public abstract class PermissionManagerContract<TPermission, TPermissionKey, TPermissionControlGroup, TPermissionControlGroupItem> : ISingletonDependency 
        where TPermission : IPermissionEntity
        where TPermissionKey : IPermissionKeyEntity
        where TPermissionControlGroup : IPermissionControlGroupEntity
        where TPermissionControlGroupItem : IPermissionControlGroupItemEntity
    {
        /// <summary>
        /// Cached User Permissions
        /// </summary>
        protected Dictionary<string, List<string>> UserPermissions { get; set; }

        protected PermissionManagerContract()
        {
            UserPermissions = new Dictionary<string, List<string>>();
        }

        #region Permission management
        /// <summary>
        /// 配置 Group Control Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TPermission AddOrUpdatePermission(TPermission entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlGroupId"></param>
        /// <returns></returns>
        public virtual IList<TPermission> GetAllPermission(string controlGroupId)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region PermissionKey management
        /// <summary>
        /// add or update permission
        /// </summary>
        public virtual TPermission AddOrUpdatePermissionKey(TPermission keyEntity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// remove permission key
        /// </summary>
        /// <param name="keyEntity"></param>
        /// <returns></returns>
        public virtual void RemovePermissionKey(TPermissionKey keyEntity) 
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get All PermissionKey that is defined
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetAllPermissionKey()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get All PermissionKey that is defined
        /// </summary>
        /// <returns></returns>
        public virtual IList<TPermissionKey> GetAllPermissionKeyEntity() 
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Permission management filtered by UserId

        /// <summary>
        /// Get user permission from db or cache
        /// </summary>
        /// <param name="userId"></param>
        public virtual List<string> GetUserPermissions(string userId)
        {
            throw new  NotImplementedException();
        }

        /// <summary>
        /// Check whether user has contain the permission key
        /// </summary>
        /// <param name="userId">user identity</param>
        /// <param name="key">permission key</param>
        /// <returns></returns>
        public virtual bool ContainPermissionKey(string userId, string key)
        {
            throw new  NotImplementedException();
        }
        #endregion

        #region Permission controller management filtered by UserId
        /// <summary>
        /// Regist user permission to cache
        /// </summary>
        /// <param name="entity"></param>
        public virtual TPermissionControlGroup AddOrUpdatePermissionGroupControl(TPermissionControlGroup entity)
        {
            throw new  NotImplementedException();
        }
        /// <summary>
        /// Get user permission from db or cache
        /// </summary>
        /// <param name="groupControlId"> group control identity</param>
        public virtual void RemovePermissionGroupControl(string groupControlId)
        {
            throw new  NotImplementedException();
        }

        /// <summary>
        /// Check whether user has contain the permission key
        /// </summary>
        /// <returns></returns>
        public virtual IList<TPermissionControlGroup> GetAllPermissionControlGroup(string controlGroup)
        {
            throw new  NotImplementedException();
        }

        /// <summary>
        /// get all permission control group
        /// </summary>
        /// <returns></returns>
        public virtual IList<TPermissionControlGroup> GetAllPermissionControlGroup()
        {
            throw new  NotImplementedException();
        }
        #endregion

        #region permission controller detail item

        /// <summary>
        /// 配置 Group Control Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TPermissionControlGroupItem AddOrUpdatePermissionGroupControlItem(TPermissionControlGroupItem entity)
        {
            throw new NotImplementedException();
        }

        public virtual void RemovePermissionGroupControlItem(string groupControlItemId)
        {
            throw new NotImplementedException();
        }

        public virtual IList<TPermissionControlGroupItem> GetAllPermissionControlGroupItem(string itemId)
        {
            throw new NotImplementedException();
        }

        public virtual IList<TPermissionControlGroupItem> GetAllPermissionControlGroupItem()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
