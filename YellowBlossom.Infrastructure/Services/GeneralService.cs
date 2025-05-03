using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace YellowBlossom.Infrastructure.Services
{
    public static class GeneralService
    {
        public static bool IsHttpContextOrUserNull(HttpContext context)
        {
            return context == null || context.User == null;
        }

        public static string? GetUserIdFromContext(HttpContext context)
        {
            return context?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
