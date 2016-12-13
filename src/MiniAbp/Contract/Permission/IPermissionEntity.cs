namespace MiniAbp.Contract.Permission
{
    /// <summary>
    /// Define permission entity
    /// </summary>
    public interface IPermissionEntity
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
        /// Control group which controller by sepecific group
        /// </summary>
        string ControlGroupId { get; set; }
        /// <summary>
        /// if =1 then it's permission allowed otherwise no.
        /// 0 Deny 1 Yes 2 NotSet
        /// </summary>
        int PermissionValue { get; set; }
    }
}