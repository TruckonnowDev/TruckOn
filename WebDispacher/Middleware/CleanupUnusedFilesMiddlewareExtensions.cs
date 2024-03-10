using Microsoft.AspNetCore.Builder;

namespace WebDispacher.Middleware
{
    public static class CleanupUnusedFilesMiddlewareExtensions
    {
        public static IApplicationBuilder CleanupUnusedFiles(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CleanupUnusedFilesMiddleware>();
        }
    }
}