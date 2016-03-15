using System;
using System.ComponentModel.DataAnnotations; 

namespace MiniAbp.Domain
{
    public class Entity : IEntity<string>
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        public DateTime? CreationTime { get; set; }

        public void RefreshId()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreationTime = DateTime.Now;
        }
    }
}
