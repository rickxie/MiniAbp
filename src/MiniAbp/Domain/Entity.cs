using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Checks if this entity is transient (it has not an Id).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        public virtual bool IsTransient()
        {
            if (EqualityComparer<string>.Default.Equals(Id, default(string)))
            {
                return true;
            }

            return false;
        }

    }
}
