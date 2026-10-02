using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.Features.Comments.Dtos
{
    public class CommentDto
    {

        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid TaskItemId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
