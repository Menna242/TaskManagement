using MediatR;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Features.Auth.Dtos;

namespace TaskManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return _identityService.RegisterAsync(
            request.FullName, request.Email, request.Password, cancellationToken);
    }
}