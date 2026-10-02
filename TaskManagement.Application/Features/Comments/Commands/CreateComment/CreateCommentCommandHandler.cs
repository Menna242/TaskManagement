using MediatR;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Comments.Commands.CreateComment;
using TaskManagement.Application.Features.Comments.Dtos;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Comments.Commands.CreateCommentCommandHandler;

public class AddCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AddCommentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }


    public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);

        if (task is null)
            throw new Exception("Task not found");

        if (task.OwnerId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new ForbiddenAccessException();

        var comment = new Comment
        {
            Content = request.Content,
            TaskItemId = request.TaskId
        };

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);


        return new CommentDto
        {
           Id= comment.Id,
           Content= comment.Content, 
           TaskItemId= comment.TaskItemId, 
           CreatedAt= comment.CreatedAt
        };
    }
}