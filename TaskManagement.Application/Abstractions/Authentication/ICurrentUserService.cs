namespace TaskManagement.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAdmin { get; }
}