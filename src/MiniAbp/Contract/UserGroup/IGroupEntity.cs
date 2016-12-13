using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniAbp.Domain;

namespace MiniAbp.Contract.UserGroup
{
    /// <summary>
    /// group necessary props
    /// </summary>
    public interface IGroupEntity
    {
        string Id { get; set; }
        string Name { get; set; }
        string Code { get; set; }
        string ParentGroupId { get; set; }
        string GroupType { get; set; }
        string GroupLevel { get; set; }
        string AreaCode { get; set; }
        string Memo { get; set; }
        int? DisplayOrder { get; set; }
        bool? IsRoot { get; set; }
    }
}
