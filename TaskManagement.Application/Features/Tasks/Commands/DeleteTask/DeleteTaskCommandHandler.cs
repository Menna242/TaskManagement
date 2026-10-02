using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    class DeleteTaskCommandHandler: IRequestHandler<DeleteTaskCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly ICurrentUserService _currentUser;

        public DeleteTaskCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException($"Task with Id {request.Id} was not found.");

            if (task.OwnerId != _currentUser.UserId && !_currentUser.IsAdmin)
                throw new ForbiddenAccessException();

            _unitOfWork.Tasks.Delete(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
