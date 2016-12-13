using System.ComponentModel.DataAnnotations;
using MiniAbp.Domain;

namespace MiniAbp.Contract.UserGroup
{
    /// <summary>
    /// Job necessary props
    /// </summary>
    public interface IJobEntity
    {
        string Id { get; set; }
        string Name { get; set; }
        string Code { get; set; }
        int? JobLevel { get; set; }
        string GroupId { get; set; }
        string ParentJobId { get; set; }
        bool IsRoot { get; set; }
    }
}