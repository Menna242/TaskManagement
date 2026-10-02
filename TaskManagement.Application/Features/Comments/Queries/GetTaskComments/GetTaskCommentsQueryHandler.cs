using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Features.Comments.Dtos;

namespace TaskManagement.Application.Features.Comments.Queries.GetTaskComments
{
    public class GetTaskCommentsQueryHandler : IRequestHandler<GetTaskCommentsQuery, IReadOnlyList<CommentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTaskCommentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IReadOnlyList<CommentDto>> Handle(
    GetTaskCommentsQuery request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);

            if (task is null)
                throw new KeyNotFoundException($"Task with id {request.TaskId} was not found.");

            var comments = await _unitOfWork.Comments
                .FindAsync(c => c.TaskItemId == request.TaskId, cancellationToken);

            return comments
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    TaskItemId = c.TaskItemId,
                    CreatedAt = c.CreatedAt
                })
                .ToList();
        }

    }
}
