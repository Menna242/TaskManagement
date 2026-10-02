using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Features.Comments.Dtos;

namespace TaskManagement.Application.Features.Comments.Queries.GetTaskComments
{
    public record GetTaskCommentsQuery(Guid TaskId) : IRequest<IReadOnlyList<CommentDto>>;
}
