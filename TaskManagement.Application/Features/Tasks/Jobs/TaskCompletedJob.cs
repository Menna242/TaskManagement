using Microsoft.Extensions.Logging;
using TaskManagement.Application.Abstractions.Persistence;

namespace TaskManagement.Application.Features.Tasks.Jobs;

public interface ITaskCompletedJob
{
    Task ExecuteAsync(Guid taskId, CancellationToken cancellationToken);
}

public class TaskCompletedJob : ITaskCompletedJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TaskCompletedJob> _logger;

    public TaskCompletedJob(IUnitOfWork unitOfWork, ILogger<TaskCompletedJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid taskId, CancellationToken cancellationToken)
    {

        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} no longer exists, skipping notification", taskId);
            return;
        }

        if (task.CompletionNotifiedAt is not null)
        {
            _logger.LogInformation(
                "Task {TaskId} was already notified at {NotifiedAt}, skipping",
                taskId, task.CompletionNotifiedAt);
            return;
        }

        _logger.LogInformation(
            "Task {TaskId} ({Title}) completed. Notifying owner {OwnerId}",
            task.Id, task.Title, task.OwnerId);

        task.CompletionNotifiedAt = DateTime.UtcNow;
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}