using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Domain.Entities
{


    public class TaskItem : BaseEntity
    {
        public Guid OwnerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; private set; } = TaskItemStatus.Todo;

        public Guid ProjectId { get; set; }
        public ProjectEntity? Project { get; set; }

        public List<Comment> Comments { get; set; } = new();


        public bool CanTransitionTo(TaskItemStatus newStatus)
        {
            if (Status == TaskItemStatus.Todo && newStatus == TaskItemStatus.InProgress) return true;
            if (Status == TaskItemStatus.InProgress && newStatus == TaskItemStatus.Completed) return true;
            if (Status == TaskItemStatus.Todo && newStatus == TaskItemStatus.Cancelled) return true;
            if (Status == TaskItemStatus.InProgress && newStatus == TaskItemStatus.Cancelled) return true;
            return false;
        }

        public void ChangeStatus(TaskItemStatus newStatus)
        {
            if (!CanTransitionTo(newStatus))
                throw new InvalidStatusTransitionException(Status, newStatus);

            Status = newStatus;
        }



    }


 

        
    
}
