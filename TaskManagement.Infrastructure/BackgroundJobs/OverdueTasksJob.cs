using Hangfire.Common;
using Microsoft.Extensions.Logging;
using Quartz;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class OverdueTasksJob : IJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OverdueTasksJob> _logger;

    public OverdueTasksJob(IUnitOfWork unitOfWork, ILogger<OverdueTasksJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var cancellationToken = context.CancellationToken;

        try
        {
            var now = DateTime.UtcNow;

            var overdue = await _unitOfWork.Tasks.FindAsync(
                t => t.DueDate != null
                     && t.DueDate < now
                     && t.Status != TaskItemStatus.Completed
                     && t.Status != TaskItemStatus.Cancelled,
                cancellationToken);

            _logger.LogInformation(
                "Overdue tasks job finished. Found {OverdueCount} overdue tasks at {RunAt}",
                overdue.Count, now);

            foreach (var task in overdue)
            {
                _logger.LogWarning(
                    "Task {TaskId} ({Title}) is overdue since {DueDate}. Owner {OwnerId}",
                    task.Id, task.Title, task.DueDate, task.OwnerId);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Overdue tasks job was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Overdue tasks job failed");
        }
    }
}