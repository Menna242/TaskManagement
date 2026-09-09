using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Abstractions.Persistence
{
    public interface IUnitOfWork
    {
        IRepository<TaskItem> Tasks { get; }
        IRepository<ProjectEntity> Projects { get; }
        IRepository<Comment> Comments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
