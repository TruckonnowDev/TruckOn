using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;

namespace WebDispacher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            SeedDatabase(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static async Task SeedDatabase(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory?.CreateScope();
            var seed = scope?.ServiceProvider.GetService<ISeedDatabaseService>();
            if (seed != null)
            {
                await seed.CreateStartRole();
                await seed.CreateStartAdmin();
                await seed.CreateStartTestCompany();
                await seed.CreateBasicOrderStatuses();
                await seed.CreateVehicleInfo();
                await seed.CreateDispatcherType();
                await seed.CreateEntityTypesResetPasswords();
                await seed.CreateEntityPhotoTypes();
                await seed.InitVehiclesCategories();
                await seed.InitTrucksTypesInVehicliesCategories();
                await seed.InitTrailersTypesInVehicliesCategories();
                await seed.InitSlugsByTruckTypesNames();
                await seed.InitSlugsByTrailerTypesNames();
                await seed.InitResourcesItems();
                await seed.InitColors();
                await seed.InitTruckThemesDefault();
                await seed.InitTruckStatuses();
                await seed.InitTrailerThemesDefault();
                await seed.InitTrailerStatuses();
            }
        }
    }
}