using System;
using System.ComponentModel.DataAnnotations;
using MiniAbp.Runtime;

namespace MiniAbp.Domain
{
    public abstract class CreationAndDeletionEntity : CreationEntity
    {
        public virtual DateTime? DeletionTime { get; set; }
        [StringLength(50)]
        public virtual string DeleterUserId { get; set; }
        public virtual bool IsDeleted { get; set; }

        public virtual void MarkDeleted()
        {
            this.IsDeleted = true;
            this.DeletionTime = DateTime.Now;
            this.DeleterUserId = YSession.GetInstance().UserId;
        }
    }
}
