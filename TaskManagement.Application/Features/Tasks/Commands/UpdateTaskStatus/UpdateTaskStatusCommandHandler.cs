using MediatR;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.BackgroundJobs;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Dtos;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IBackgroundJobService _backgroundJobs;


    public UpdateTaskStatusCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IBackgroundJobService backgroundJobs)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _backgroundJobs = backgroundJobs;
    }

    public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {

        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
            throw new KeyNotFoundException($"Task with Id {request.Id} was not found.");

        if (task.OwnerId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenAccessException();

        var newStatus = Enum.Parse<TaskItemStatus>(request.NewStatus, ignoreCase: true);
        task.ChangeStatus(newStatus);


        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (task.Status == TaskItemStatus.Completed)
            _backgroundJobs.EnqueueTaskCompleted(task.Id);

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            ProjectId = task.ProjectId,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };
    }
}