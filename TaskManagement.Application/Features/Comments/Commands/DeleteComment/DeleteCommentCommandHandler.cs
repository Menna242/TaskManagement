using MediatR;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;

namespace TaskManagement.Application.Features.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICurrentUserService _currentUser;

    public DeleteCommentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);


        if (comment is null)
            throw new KeyNotFoundException($"Comment with id {request.Id} was not found.");

        var task = await _unitOfWork.Tasks.GetByIdAsync(comment.TaskItemId, cancellationToken);

        if (task is null || (task.OwnerId != _currentUser.UserId && !_currentUser.IsAdmin))
            throw new ForbiddenAccessException();

        _unitOfWork.Comments.Delete(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}