using MediatR;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Features.Auth.Dtos;

namespace TaskManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        => _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
}