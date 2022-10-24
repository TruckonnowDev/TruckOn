
using Microsoft.AspNetCore.Builder;         
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Collections.Generic;
using System.IO.Compression;
using DaoModels.DAO;
using Microsoft.EntityFrameworkCore;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.ViewModels.Mappings;

using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;

namespace WebDispacher
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddDbContext<Context>();
                services.AddMvc()
                        .AddDataAnnotationsLocalization()
                        .AddViewLocalization();

           services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("es"),
                    new CultureInfo("ru")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<ITruckAndTrailerService, TruckAndTrailerService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, Business.Services.OrderService>();

            StripeConfiguration.ApiKey = "sk_test_51GuYHUKfezfzRoxlAPF3ieVKcPe9Ost93jouMwF6nT0mFCh59qDBdUEN3E23nYx3gBUGmDpTo8NfJnw6unSie3NV00UcJWHAXu";

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 200; // 200 items max
                options.ValueLengthLimit = 1024 * 1024 * 500; // 100MB max len form data
            });
            System.Net.ServicePointManager.DefaultConnectionLimit = 50;

            
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            });
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
            
            //services.AddResponseCompression(options => 
            //{
            //    IEnumerable<string> MimeTypes = new[]
            //    {
            //        "text/plain",
            //        "text/css",
            //        "application/javascript",
            //        "text/html",
            //        "application/xml",
            //        "text/xml",
            //        "application/json",
            //        "text/json",
            //        "image/png",
            //        "image/jpg"
            //    };
            //    options.EnableForHttps = true;
            //    options.MimeTypes = MimeTypes;
            //    options.Providers.Add<GzipCompressionProvider>();
            //    options.Providers.Add<BrotliCompressionProvider>();
            //});
            //services.Configure<BrotliCompressionProviderOptions>(options =>
            //{
            //    options.Level = CompressionLevel.Optimal;
            //});
            //services.Configure<GzipCompressionProviderOptions>(options =>
            //{
            //    options.Level = CompressionLevel.Optimal;
            //});
            //services.AddMemoryCache();

        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");
            // app.UseResponseCompression();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=RA}/{action=Index}/{id?}");
                
            });
            app.UseStaticFiles();
            //app.UseStatusCodePagesWithRedirects("/error?code={0}"); 
        }
    }
}
