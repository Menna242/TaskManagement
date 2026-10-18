using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Abstractions.BackgroundJobs
{
    public interface IBackgroundJobService
    {
        void EnqueueTaskCompleted(Guid taskId);

    }
}
