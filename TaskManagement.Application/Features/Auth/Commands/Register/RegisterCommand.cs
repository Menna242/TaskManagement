
using MediatR;
using TaskManagement.Application.Features.Auth.Dtos;

namespace TaskManagement.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string FullName, string Email, string Password)
    : IRequest<AuthResponse>;