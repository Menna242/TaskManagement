using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Exceptions
{
    public class InvalidStatusTransitionException : Exception
    {
        public InvalidStatusTransitionException(TaskItemStatus from, TaskItemStatus to)
            : base($"Cannot change task status from {from} to {to}.")
        {
        }
    }
}
