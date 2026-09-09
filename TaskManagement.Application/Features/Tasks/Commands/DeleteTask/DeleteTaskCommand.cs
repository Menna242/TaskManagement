using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Features.Tasks.Dtos;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    public record DeleteTaskCommand(
        Guid Id
        ) : IRequest<Unit>; //Unit like Void return null
}
