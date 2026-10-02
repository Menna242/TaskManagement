using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Dtos;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ICurrentUserService _currentUser;

        public CreateTaskCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not Guid userId)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjecgtId, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException($"Project with id {request.ProjecgtId} was not found.");

            if (project.OwnerId != userId && !_currentUser.IsAdmin)
                throw new ForbiddenAccessException();


            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                ProjectId = request.ProjecgtId,
                OwnerId = userId,
            };

            await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
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
}
