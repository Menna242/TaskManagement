using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Commands.AssignTask;
using TaskManagement.Application.Features.Tasks.Dtos;

namespace TaskManagement.Application.Features.Tasks.Commands.AssignTask
{
    public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IIdentityService _identityService;


        public AssignTaskCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _identityService = identityService;
        }

        public async Task<TaskDto> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException($"Task with Id {request.TaskId} was not found.");

            if (task.OwnerId != _currentUser.UserId && !_currentUser.IsAdmin)
                throw new ForbiddenAccessException();

            if (!await _identityService.UserExistsAsync(request.UserId, cancellationToken))
                throw new KeyNotFoundException($"User with Id {request.UserId} was not found.");


            task.AssignedToUserId = request.UserId;
            task.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                AssignedToUserId = task.AssignedToUserId,
                ProjectId = task.ProjectId,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt
            };
        }

       
    }
}
