using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Features.Tasks.Dtos;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler:IRequestHandler<UpdateTaskCommand,TaskDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException($"Task with Id {request.Id} was not found.");

            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = Enum.Parse<Domain.Entities.TaskStatus>(request.Status);
            task.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Tasks.Update(task);
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
