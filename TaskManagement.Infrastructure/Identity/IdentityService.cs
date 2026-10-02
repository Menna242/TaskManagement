using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Features.Auth.Dtos;
using TaskManagement.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using TaskManagement.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        AppDbContext context,
        IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponse> RegisterAsync(
        string fullName, string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);   
        }

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        });
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.Email!, accessToken, refreshToken);
    }


    public async Task<AuthResponse> LoginAsync(
     string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        });
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.Email!, accessToken, refreshToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(
    string refreshToken, CancellationToken cancellationToken = default)
    {
        var hash = _tokenService.HashToken(refreshToken);

        var stored = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == hash, cancellationToken);

        if (stored is null || !stored.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        stored.RevokedAt = DateTime.UtcNow;

        var user = stored.User;
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.Email!, newAccessToken, newRefreshToken);
    }

    public async Task LogoutAsync(
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var hash = _tokenService.HashToken(refreshToken);

        var stored = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == hash, cancellationToken);

        if (stored is not null && stored.IsActive)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}