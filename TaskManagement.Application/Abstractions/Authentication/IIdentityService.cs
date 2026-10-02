using TaskManagement.Application.Features.Auth.Dtos;

namespace TaskManagement.Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<AuthResponse> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}