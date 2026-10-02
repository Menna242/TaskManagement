using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Entities
{


    public class ProjectEntity :BaseEntity
    {
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<TaskItem> Tasks { get; set; } = new();

    }
}
