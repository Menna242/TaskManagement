using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Features.Tasks.Dtos;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand (
        Guid Id,
        string Title,
        string? Description,
        string Status
        ) : IRequest<TaskDto>;
}
