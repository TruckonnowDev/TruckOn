using Stripe;
using DaoModels.DAO;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.ViewModels.Mappings;

using System.Globalization;
using Microsoft.AspNetCore.Localization;
using WebDispacher.Middleware;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http.Features;
using WebDispacher.Constants.Identity;
using System;
using AspNetCoreRateLimit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Services.HistoryFactory;
using WebDispacher.ViewModels.Truck;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Driver;
using WebDispacher.ViewModels.Marketplace;

namespace WebDispacher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();

            var rateLimitOptions = new IpRateLimitOptions
            {
                EnableEndpointRateLimiting = true,
                StackBlockedRequests = false,
                RealIpHeader = "X-Real-IP",
                ClientIdHeader = "X-ClientId",
                HttpStatusCode = 429,
                QuotaExceededMessage = "Too Many Requests"
            };

            rateLimitOptions.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1s",
                    Limit = 50
                }
            };

            services.AddMemoryCache();

            services.AddSingleton(rateLimitOptions);

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddDbContext<Context>(ServiceLifetime.Transient);

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

                var defaultCulture = new CultureInfo("en");

                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                var numberFormat = new NumberFormatInfo
                {
                    NumberDecimalSeparator = ".",
                };

                foreach (var culture in supportedCultures)
                {
                    culture.NumberFormat = numberFormat;
                }
            });

            services.Configure<MvcOptions>(options =>
            {
                var cultureEs = new CultureInfo("es");
                cultureEs.NumberFormat.CurrencyDecimalSeparator = ".";
                cultureEs.NumberFormat.NumberDecimalSeparator = ".";
                
                var cultureRu = new CultureInfo("ru");
                cultureRu.NumberFormat.CurrencyDecimalSeparator = ".";
                cultureRu.NumberFormat.NumberDecimalSeparator = ".";
            });


            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<Context>();
            services.AddScoped<ITruckAndTrailerService, TruckAndTrailerService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMarketplaceService, MarketplaceService>();
            services.AddScoped<IOrderService, Business.Services.OrderService>();
            services.AddScoped<ISeedDatabaseService, SeedDatabaseService>();

            services.AddScoped<IHistoryActionService, HistoryActionService>();
            services.AddScoped<IHistoryActionFactory<HistoryTruckAction, TruckViewModel>, HistoryTruckActionFactory>();
            services.AddScoped<IHistoryActionFactory<HistoryTrailerAction, TrailerViewModel>, HistoryTrailerActionFactory>();
            services.AddScoped<IHistoryActionFactory<HistoryDriverAction, EditDriverViewModel>, HistoryDriverActionFactory>();
            services.AddScoped<IHistoryActionFactory<HistoryMarketPostAction, BuyItemMarketPostViewModel>, HistoryBuyItemMarketPostActionFactory>();
            services.AddScoped<IHistoryActionFactory<HistoryMarketPostAction, SellItemMarketPostViewModel>, HistorySellItemMarketPostActionFactory>();

            services.AddSingleton<Functions.Functions>();

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

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;
            });

            services.AddAuthorization(options =>
            {

            options.AddPolicy(PolicyIdentityConstants.CarrierCompany, policy =>
                policy.RequireRole(RolesIdentityConstants.UserRole)
                    .RequireClaim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyCarrierValue)
                    .RequireClaim(ClaimsIdentityConstants.CompanyId));

            options.AddPolicy(PolicyIdentityConstants.CarrierAdminCompany, policy =>
                policy.RequireRole(RolesIdentityConstants.AdminRole)
                .RequireClaim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyCarrierAdminValue)
                .RequireClaim(ClaimsIdentityConstants.CompanyId));

            options.AddPolicy(PolicyIdentityConstants.CarrierCompanyOrAdmin, policy =>
            {
                policy.RequireRole(RolesIdentityConstants.UserRole, RolesIdentityConstants.AdminRole)
                 .RequireAssertion(context =>
                 {
                     return context.User.HasClaim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyCarrierValue) ||
                            context.User.HasClaim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyCarrierAdminValue);
                 });
            });

                options.AddPolicy(PolicyIdentityConstants.ShipperCompany, policy =>
                policy.RequireRole(RolesIdentityConstants.UserRole)
                .RequireClaim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyShipperValue));

            options.AddPolicy(PolicyIdentityConstants.BrokerCompany, policy =>
                policy.RequireRole(RolesIdentityConstants.UserRole)
                .RequireClaim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyBrokerValue)
                .RequireClaim(ClaimsIdentityConstants.CompanyId));
            });

            services.ConfigureApplicationCookie(o =>
            {
                o.LoginPath = "/carrier-login";
                o.AccessDeniedPath = "/AccessDanied";
            });

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(25);
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
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");

            // app.UseResponseCompression();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCheckUserActive();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=RA}/{action=Index}/{id?}");
            });
            app.UseStaticFiles();
            //app.UseStatusCodePagesWithRedirects("/error?code={0}"); 
        }
    }
}
