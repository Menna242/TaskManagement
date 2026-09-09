using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Entities
{
    public class Comment: BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public Guid TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }
    }
}
