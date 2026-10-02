namespace TaskManagement.Application.Features.Auth.Dtos;

public record AuthResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken);