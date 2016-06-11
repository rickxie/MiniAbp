using System;
using System.ComponentModel.DataAnnotations;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public abstract class CreationEntity : Entity
    {
        public virtual DateTime? CreationTime { get; set; }
        [StringLength(50)]
        public virtual string CreatorUserId { get; set; }

        public new void RefreshId()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreationTime = DateTime.Now;
            this.CreatorUserId = YSession.GetInstance().UserId;
        }
    }
}
