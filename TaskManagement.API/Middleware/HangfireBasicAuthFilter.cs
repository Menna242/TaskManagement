using System.Text;
using Hangfire.Dashboard;

namespace TaskManagement.API.Middleware;

public class HangfireBasicAuthFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;

    public HangfireBasicAuthFilter(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        string? header = http.Request.Headers["Authorization"];

        if (header is not null && header.StartsWith("Basic "))
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(header["Basic ".Length..]));
            if (decoded == $"{_username}:{_password}")
                return true;
        }

        http.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        http.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return false;
    }
}