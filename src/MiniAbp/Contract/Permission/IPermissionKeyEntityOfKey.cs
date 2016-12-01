namespace MiniAbp.Contract.Permission
{
    /// <summary>
    /// Define permission entity
    /// </summary>
    public interface IPermissionKeyEntity<TKey>
    {
        /// <summary>
        /// Identity
        /// </summary>
        TKey Id { get; set; }
        /// <summary>
        /// Permission Key, where define with requirements
        /// </summary>
        string PermissionKey { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        TKey ParentId { get; set; }
    }
}