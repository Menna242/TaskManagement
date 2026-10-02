using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Features.Projects.Dtos;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectById
{
    public record class GetProjectByIdQuery(Guid Id) :IRequest<ProjectDto>;
    
}
