using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject
{
    public record class UpdateProjectCommand
    (Guid Id, string Name, string? Description) : IRequest<Unit>;

}
