using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject
{

    public  record class DeleteProjectCommand(Guid Id) : IRequest<Unit>;

}
