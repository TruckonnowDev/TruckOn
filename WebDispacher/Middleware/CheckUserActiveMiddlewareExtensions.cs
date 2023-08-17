using Microsoft.AspNetCore.Builder;

namespace WebDispacher.Middleware
{
    public static class CheckUserActiveMiddlewareExtensions
    {
        public static IApplicationBuilder UseCheckUserActive(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckUserActiveMiddleware>();
        }
    }
}
