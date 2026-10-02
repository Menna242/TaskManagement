using Microsoft.AspNetCore.Http;
using TaskManagement.Application.Abstractions.Authentication;

namespace TaskManagement.Infrastructure.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public bool IsAdmin =>
        _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
}