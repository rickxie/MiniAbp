using System;
using System.ComponentModel.DataAnnotations; 

namespace MiniAbp.Domain
{
    public abstract class Entity : IEntity<string>
    {
        [Key]
        [StringLength(50)]
        public virtual string Id { get; set; }

        public void RefreshId()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
