using System.ComponentModel.DataAnnotations;
using MiniAbp.Domain;

namespace MiniAbp.Contract.UserGroup
{
    /// <summary>
    /// job user necessary props
    /// </summary>
    public interface IJobUserEntity
    {
        string Id { get; set; }
        string JobId { get; set; }
        string UserId { get; set; }
        bool IsPrimary { get; set; }
    }
}
