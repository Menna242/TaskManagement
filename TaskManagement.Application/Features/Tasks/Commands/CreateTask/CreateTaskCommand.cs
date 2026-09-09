using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Features.Tasks.Dtos;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand
    (
        string Title,
        string? Description,
        Guid ProjecgtId
    ) : IRequest<TaskDto>;
}
