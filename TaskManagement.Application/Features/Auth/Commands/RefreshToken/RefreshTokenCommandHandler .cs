using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Features.Auth.Dtos;

namespace TaskManagement.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;

        public RefreshTokenCommandHandler(IIdentityService identityService)
            => _identityService = identityService;

        public Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            => _identityService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}
