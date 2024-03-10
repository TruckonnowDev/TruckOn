using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebDispacher.Middleware
{
    public class CleanupUnusedFilesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _uploadsPath;

        public CleanupUnusedFilesMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _uploadsPath = Path.Combine(env.WebRootPath, "uploads");
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            CleanupUnusedFiles();
        }

        private void CleanupUnusedFiles()
        {
            var currentTime = DateTime.Now;

            var uploadedFiles = Directory.GetFiles(_uploadsPath);
            var usedFiles = Directory.GetFiles(_uploadsPath, "*.jpg")
                .Select(filePath => Path.GetFileName(filePath));

            var unusedAndExpiredFiles = uploadedFiles
                .Except(usedFiles)
                .Where(filePath =>
                {
                    var creationTime = File.GetCreationTime(filePath);
                    var elapsedTime = currentTime - creationTime;
                    return elapsedTime.TotalHours > 1;
                });

            foreach (var fileToDelete in unusedAndExpiredFiles)
            {
                File.Delete(fileToDelete);
            }
        }
    }
}