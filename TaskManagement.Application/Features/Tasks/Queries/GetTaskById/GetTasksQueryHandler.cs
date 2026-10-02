using MediatR;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Features.Tasks.Dtos;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
            return Error.NotFound("Task.NotFound", "The task was not found.");

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            ProjectId = task.ProjectId,
            CreatedAt = task.CreatedAt
        };
    }
}