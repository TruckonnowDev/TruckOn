﻿using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace DaoModels.DAO
{
    public class Context : IdentityDbContext<User>
    {
        public static IMemoryCache _cache = null;

        public DbSet<AddressInformation> AddressInformations { get; set; }
        public DbSet<AdminAnswer> AdminAnswers { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CurrentStatus> CurrentStatuses { get; set; }
        public DbSet<CustomerST> CustomerST { get; set; }
        public DbSet<Damage> Damages { get; set; }
        public DbSet<DamageForUser> DamageForUsers { get; set; }
        public DbSet<Dispatcher> Dispatchers { get; set; }
        public DbSet<DispatcherType> DispatchersTypes { get; set; }
        public DbSet<DocumentCompany> DocumentsCompanies { get; set; }
        public DbSet<DocumentDriver> DocumentsDrivers { get; set; }
        public DbSet<DocumentTrailer> DocumentsTrailers { get; set; }
        public DbSet<DocumentTruck> DocumentsTrucks { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverControl> DriversControls { get; set; }
        public DbSet<DriverInspection> DriversInspections { get; set; }
        public DbSet<DriverReport> DriversReports { get; set; }
        public DbSet<Geolocation> Geolocations { get; set; }
        public DbSet<HistoryOrderAction> HistoriesOrdersActions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PasswordRecovery> PasswordsRecoveries { get; set; }
        public DbSet<PaymentMethod> PaymentsMethods { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoAnswer> PhotosAnswers { get; set; }
        public DbSet<PhotoDriverInspection> PhotosDriversInspections { get; set; }
        public DbSet<PhotoType> PhotoTypes { get; set; }
        public DbSet<PhotoVehicleInspection> PhotosVehiclesInspections { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<RecoveryType> RecoveriesTypes { get; set; }
        public DbSet<SubscribeST> SubscribeST { get; set; }
        public DbSet<Trailer> Trailers { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<UserAnswer> UsersAnswers { get; set; }
        public DbSet<UserMailQuestion> UsersMailsQuestions { get; set; }
        public DbSet<UserQuestionnaire> UsersQuestionnaires { get; set; }
        public DbSet<VehicleBody> VehiclesBodies { get; set; }
        public DbSet<VehicleBrand> VehiclesBrands { get; set; }
        public DbSet<VehicleDetails> VehiclesDetails { get; set; }
        public DbSet<VehicleInspection> VehiclesInspections { get; set; }
        public DbSet<VehicleModel> VehiclesModels { get; set; }
        public DbSet<VehicleType> VehiclesTypes { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoAnswer> VideosAnswers { get; set; }
        public DbSet<VideoType> VideoTypes { get; set; }
        public DbSet<Layouts> Layouts { get; set; }
        public DbSet<ProfileSettings> ProfileSettings { get; set; }
        public DbSet<TransportVehicle> TransportVehicles { get; set; }
        public DbSet<PhoneNumber> PhonesNumbers { get; set; }

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
                //optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=WebDispatch2;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");
                optionsBuilder.UseSqlServer("Data Source=212.224.113.5;Initial Catalog=WebDispatchNew;Integrated Security=False;User ID=roma;Password=123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
                //optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=WebDispatch;Integrated Security=False;User ID=roma;Password=123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
                //optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=WebDispatchDev;Integrated Security=False;User ID=roma;Password=123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CompanyUser>()
            .HasKey(cu => new { cu.CompanyId, cu.UserId });

            modelBuilder.Entity<CompanyUser>()
                .HasOne(cu => cu.Company)
                .WithMany(c => c.CompanyUsers)
                .HasForeignKey(cu => cu.CompanyId);

            modelBuilder.Entity<CompanyUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.CompanyUsers)
                .HasForeignKey(cu => cu.UserId);
        }
    }
}