using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Features.Tasks.Dtos;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById
{
    public record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskDto>>;
}
