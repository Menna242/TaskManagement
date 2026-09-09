using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Repositories;

namespace TaskManagement.Infrastructure.Persistence
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IRepository<TaskItem>? _tasks;
        private IRepository<ProjectEntity>? _projects;
        private IRepository<Comment>? _comments;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }


        public IRepository<TaskItem> Tasks => _tasks ?? new Repository<TaskItem>(_context);

        public IRepository<ProjectEntity> Projects =>_projects?? new Repository<ProjectEntity>(_context);

        public IRepository<Comment> Comments =>_comments ?? new Repository<Comment>(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
