using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniAbp.Domain;

namespace MiniAbp.Contract.UserGroup
{
    /// <summary>
    /// group extension necessary props
    /// </summary>
    public interface IGroupExtensionEntity
    {
        string Id { get; set; }
        string GroupId { get; set; }
        string AreaCode { get; set; }
        string DisplayCode { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        string Fax { get; set; }
        string Memo { get; set; }
        int? DisplayOrder { get; set; }
    }
}
