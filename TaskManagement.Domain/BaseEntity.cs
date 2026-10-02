using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain
{
    public abstract class BaseEntity<TId> where TId : notnull
    {
        public TId Id { get; set; } = default!; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}


