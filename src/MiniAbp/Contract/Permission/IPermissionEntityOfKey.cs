namespace MiniAbp.Contract.Permission
{
    /// <summary>
    /// Define permission entity
    /// </summary>
    public interface IPermissionEntity<T>
    {
        /// <summary>
        /// Identity
        /// </summary>
        T Id { get; set; }
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
        /// </summary>
        bool IsGranted { get; set; }
    }
}