using MediatR;
using TaskManagement.Application.Features.Tasks.Dtos;

namespace TaskManagement.Application.Features.Tasks.Commands.AssignTask;

public record AssignTaskCommand(Guid TaskId, Guid UserId) : IRequest<TaskDto>;