using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Abstractions.Authentication;

namespace TaskManagement.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IIdentityService _identityService;

        public LogoutCommandHandler(IIdentityService identityService)
            => _identityService = identityService;

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _identityService.LogoutAsync(request.RefreshToken, cancellationToken);
            return Unit.Value;
        }
    }
}
