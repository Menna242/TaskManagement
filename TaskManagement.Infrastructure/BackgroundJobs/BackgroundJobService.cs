using Hangfire;
using TaskManagement.Application.Abstractions.BackgroundJobs;
using TaskManagement.Application.Features.Tasks.Jobs;

namespace TaskManagement.Infrastructure.BackgroundJobs;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _client;

    public BackgroundJobService(IBackgroundJobClient client)
    {
        _client = client;
    }

    public void EnqueueTaskCompleted(Guid taskId)
    {
        _client.Enqueue<ITaskCompletedJob>(job => job.ExecuteAsync(taskId, CancellationToken.None));
    }
}