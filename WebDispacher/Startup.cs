﻿using ApiMobaileServise.BackgraundService;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;         
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebDispacher
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=RA}/{action=Index}/{id?}");
            });
        }
    }
}
