using FluentValidation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.NewStatus)
             .NotEmpty().WithMessage("Status is required.")
             .Must(s => Enum.TryParse<TaskItemStatus>(s, ignoreCase: true, out var parsed)
                        && Enum.IsDefined(parsed))
             .WithMessage("Status must be one of: Todo, InProgress, Completed, Cancelled.");
    }
}