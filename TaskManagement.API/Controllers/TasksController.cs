using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Application.Features.Tasks.Dtos;
using TaskManagement.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagement.Application.Features.Tasks.Queries.GetTasks;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ISender _sender;

        public TasksController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("Id in the route does not match Id in the request body.");

            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<TaskDto>> UpdateStatus(
        Guid id,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new UpdateTaskStatusCommand(id, request.Status),
                cancellationToken);

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteTaskCommand(id), cancellationToken);
            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetTasksQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetTaskByIdQuery(id), cancellationToken);
            return Ok(result);
        }


    }
    public record UpdateTaskStatusRequest(string Status);
}
