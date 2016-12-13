using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniAbp.Domain;

namespace MiniAbp.Contract.UserGroup
{
    /// <summary>
    /// role job nessary props.
    /// </summary>
    public interface IRoleJobEntity 
    {
         string Id { get; set; }
         string RoleId { get; set; }
         string JobId { get; set; }
         string Memo { get; set; }
    }
}
