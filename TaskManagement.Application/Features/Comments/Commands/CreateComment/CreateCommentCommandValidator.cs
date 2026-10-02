using FluentValidation;
using TaskManagement.Application.Features.Comments.Commands.CreateComment;

namespace TaskManagement.Application.Features.Comments.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("TaskId is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(1000).WithMessage("Content must not exceed 1000 characters.");
    }
}