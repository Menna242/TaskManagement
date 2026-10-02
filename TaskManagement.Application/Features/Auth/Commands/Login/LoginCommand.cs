using MediatR;
using TaskManagement.Application.Features.Auth.Dtos;

namespace TaskManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;