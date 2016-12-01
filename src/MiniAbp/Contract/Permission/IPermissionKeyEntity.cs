namespace MiniAbp.Contract.Permission
{
    /// <summary>
    /// Define permission entity
    /// </summary>
    public interface IPermissionKeyEntity
    {
        /// <summary>
        /// Identity
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Permission Key, where define with requirements
        /// </summary>
        string PermissionKey { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        string ParentId { get; set; }
    }
}