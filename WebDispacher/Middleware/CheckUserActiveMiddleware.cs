using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;

namespace WebDispacher.Middleware
{
    public class CheckUserActiveMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckUserActiveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            if (!string.IsNullOrEmpty(httpContext.User.Identity?.Name))
                await SignOut(httpContext, userManager, signInManager);

            await _next(httpContext);
        }

        private async Task SignOut(HttpContext httpContext, UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            var user = await userManager.FindByNameAsync(httpContext.User.Identity?.Name);
            if (user != null)
            {
                if (user.LockoutEnd > DateTimeOffset.Now)
                {
                    await signInManager.SignOutAsync();
                    httpContext.Response.Redirect("/carrier-login");
                }
            }
            else
            {
                await signInManager.SignOutAsync();
                httpContext.Response.Redirect("/carrier-reg");
            }
        }
    }
}
