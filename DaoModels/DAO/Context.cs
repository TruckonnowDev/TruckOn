using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;

namespace DaoModels.DAO
{
    public class Context : DbContext
    {
        public static IMemoryCache _cache = null;

        public DbSet<Shipping> Shipping { get; set; }
        public DbSet<Users> User { get; set; }
        public DbSet<VehiclwInformation> VehiclwInformation { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoDriver> PhotoDrivers { get; set; }
        public DbSet<Ask> Asks { get; set; }
        public DbSet<PhotoInspection> PhotoInspections { get; set; }
        public DbSet<Ask1> Ask1s { get; set; }
        public DbSet<Ask2> Ask2s { get; set; }
        public DbSet<AskFromUser> AskFromUsers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AskDelyvery> AskDelyveries { get; set; }
        public DbSet<AskForUserDelyveryM> askForUserDelyveryMs { get; set; }
        public DbSet<Damage> Damages { get; set; }
        public DbSet<Geolocations> geolocations { get; set; }
        public DbSet<DamageForUser> DamageForUsers { get; set; }
        public DbSet<InspectionDriver> InspectionDrivers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<LogTask> LogTasks { get; set; }
        public DbSet<TaskLoad> TaskLoads { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<Trailer> Trailers { get; set; }
        public DbSet<DocumentTruckAndTrailers> DocumentTruckAndTrailers { get; set; }
        public DbSet<DucumentCompany> DucumentCompanies { get; set; }
        public DbSet<PasswordRecovery> PasswordRecoveries { get; set; }
        public DbSet<DriverReport> DriverReports { get; set; }
        public DbSet<HistoryOrder> HistoryOrders { get; set; }
        public DbSet<Commpany> Commpanies { get; set; }
        public DbSet<TransportVehicle> TransportVehicles { get; set; }
        public DbSet<NamePatern> NamePaterns { get; set; }
        public DbSet<Layouts> Layouts { get; set; }
        public DbSet<ProfileSetting> ProfileSettings { get; set; }
        public DbSet<DucumentDriver> DucumentDrivers { get; set; }
        public DbSet<Customer_ST> Customer_STs { get; set; }
        public DbSet<Subscribe_ST> Subscribe_STs { get; set; }
        public DbSet<PaymentMethod_ST> PaymentMethods { get; set; }
        public DbSet<Dispatcher> Dispatchers { get; set; }
        public DbSet<VehicleHistory> VehicleHistories { get; set; }

        public Context()
        {
            try
            { 
                //Database.EnsureCreated();
                Database.Migrate();
            }
            catch (Exception e)
            {
                //File.AppendAllText("db.txt", e.Message);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WebDispatchDB;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer("Data Source=212.224.113.5;Initial Catalog=WebDispatch;Integrated Security=False;User ID=roma;Password=123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
                //optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=WebDispatch;Integrated Security=False;User ID=roma;Password=123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
                //optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=WebDispatchDev;Integrated Security=False;User ID=roma;Password=123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
            }
        }
    }
}