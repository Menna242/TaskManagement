using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Features.Projects.Dtos;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ICurrentUserService _currentUser;

        public CreateProjectCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not Guid userId)
                throw new UnauthorizedAccessException("User is not authenticated.");



            var project = new ProjectEntity
            {
                Name = request.Name,
                Description = request.Description,
                OwnerId = userId
            };


            await _unitOfWork.Projects.AddAsync(project,cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt
            };
        }
    }
}
