using System;
using System.ComponentModel.DataAnnotations;

namespace MiniAbp.Contract.UserGroup
{
    /// <summary>
    /// user necessary props.
    /// </summary>
    public interface IUserEntity 
    {
         string Id { get; set; }
         string Name { get; set; }
         string Account { get; set; }
         string Password { get; set; }
    }
}
