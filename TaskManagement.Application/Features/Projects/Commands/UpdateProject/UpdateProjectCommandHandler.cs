using MediatR;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public UpdateProjectCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);

            if (project is null)
                throw new KeyNotFoundException($"Project with id {request.Id} was not found.");

            if (project.OwnerId != _currentUser.UserId && !_currentUser.IsAdmin)
                throw new ForbiddenAccessException();

            project.Name = request.Name;
            project.Description = request.Description;

            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}