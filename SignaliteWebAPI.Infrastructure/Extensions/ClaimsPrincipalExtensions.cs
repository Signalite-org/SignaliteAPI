using System.Security.Claims;
using SignaliteWebAPI.Infrastructure.Exceptions;

namespace SignaliteWebAPI.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            throw new AuthException("Cannot get id from token");
        }
        return int.Parse(userId);
    }

    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.Name);
        if (username == null)
        {
            throw new AuthException("Cannot get username from token");
        }
        return username;
    }
}