using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Features.Comments.Dtos;

namespace TaskManagement.Application.Features.Comments.Commands.CreateComment
{
   public record class CreateCommentCommand(Guid TaskId, string Content) : IRequest<CommentDto>;

    
}
