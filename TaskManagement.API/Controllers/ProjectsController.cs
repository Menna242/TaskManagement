using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Application.Features.Tasks.Queries.GetProjects;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ISender _sender;

        public ProjectsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProjectsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
