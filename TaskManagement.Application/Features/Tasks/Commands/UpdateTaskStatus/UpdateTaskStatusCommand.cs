using MediatR;
using TaskManagement.Application.Features.Tasks.Dtos;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(Guid Id, string NewStatus) : IRequest<TaskDto>;