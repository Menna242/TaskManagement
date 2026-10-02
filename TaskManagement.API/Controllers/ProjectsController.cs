using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Application.Features.Projects.Commands.UpdateProject;
using TaskManagement.Application.Features.Projects.Queries.GetProjectById;
using TaskManagement.Application.Features.Projects.Queries.GetProjects;

namespace TaskManagement.API.Controllers
{
    [Authorize]
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("Id mismatch.");

            await _sender.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteProjectCommand(id), cancellationToken);
            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProjectsQuery(), cancellationToken);
            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProjectById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProjectByIdQuery(id), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
