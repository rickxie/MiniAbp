using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniAbp.Contract.UserGroup
{
    public interface IRoleEntity
    {
        string Id { get; set; }
        string Code { get; set; }
        string Name { get; set; }
        string ParentId { get; set; }
    }
}
