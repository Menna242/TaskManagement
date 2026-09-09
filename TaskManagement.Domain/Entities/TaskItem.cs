using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Entities
{

    public enum TaskStatus
    {
        Todo,
        InProgress,
        Done
    }

    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Todo;

        public Guid ProjectId { get; set; }
        public ProjectEntity? Project { get; set; }

        public List<Comment> Comments { get; set; } = new();


    }

 

        
    
}
