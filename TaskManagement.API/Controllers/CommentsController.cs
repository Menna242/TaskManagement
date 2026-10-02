using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Comments.Commands.CreateComment;
using TaskManagement.Application.Features.Comments.Commands.DeleteComment;
using TaskManagement.Application.Features.Comments.Queries.GetTaskComments;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class CommentsController : ControllerBase
    {
        private readonly ISender _sender;

        public CommentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("tasks/{taskId:guid}/comments")]
        public async Task<IActionResult> CreateComment(
            Guid taskId,
            CreateCommentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateCommentCommand(taskId, request.Content),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("tasks/{taskId:guid}/comments")]
        public async Task<IActionResult> GetTaskComments(
            Guid taskId,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetTaskCommentsQuery(taskId),
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("comments/{id:guid}")]
        public async Task<IActionResult> DeleteComment(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteCommentCommand(id), cancellationToken);
            return NoContent();
        }
    }

    public record CreateCommentRequest(string Content);
}