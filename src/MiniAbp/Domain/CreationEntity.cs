using System;
using System.ComponentModel.DataAnnotations; 

namespace MiniAbp.Domain
{
    public class CreationEntity : Entity
    {
        public DateTime? CreationTime { get; set; }

        public new void RefreshId()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreationTime = DateTime.Now;
        }
    }
}
