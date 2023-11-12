using DaoModels.DAO;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using WebDispacher.Business.Interfaces;
using System.Linq;
using DaoModels.DAO.Enum;
using static System.Net.Mime.MediaTypeNames;
using WebDispacher.Constants;
using System.Xml.Linq;
using System.Collections.Generic;

namespace WebDispacher.Business.Services
{
    public class SeedDatabaseService : ISeedDatabaseService
    {
        private readonly Context db;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly ICompanyService companyService;

        public SeedDatabaseService(Context db, RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager, ICompanyService companyService)
        {
            this.roleManager = roleManager;
            this.db = db;
            this.userManager = userManager;
            this.companyService = companyService;
        }

        public async Task CreateStartRole()
        {
            if (db.Roles.Any(x => x.Name == "Admin"))
            {
                Console.WriteLine("Admin role already exists");
            }
            else
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                Console.WriteLine("Admin role successfully added");
            }

            if (db.Roles.Any(x => x.Name == "User"))
            {
                Console.WriteLine("User role already exists");
            }
            else
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
                Console.WriteLine("User role successfully added");
            }
        }

        public async Task CreateStartAdmin()
        {
            if (db.Users.Any(x => x.UserName == "truckonnow@test.com") || db.Companies.Any(x => x.Name == "Truckonnow"))
            {
                Console.WriteLine("Admin company or user already exists");
            }
            else
            {
                var user = new User
                {
                    Email = "truckonnow@test.com",
                    UserName = "truckonnow@test.com"
                };

                var phoneNumber = new PhoneNumber
                {
                    Name = "United States",
                    Iso2 = "us",
                    DialCode = 1,
                    Number = "(555)-555-5555",
                };

                var company = new Company
                {
                    Name = "Truckonnow Team",
                    CompanyType = CompanyType.Carrier,
                    CompanyStatus = CompanyStatus.Admin,
                    DateTimeRegistration = DateTime.Now,
                    PhoneNumber = phoneNumber,
                };

                await userManager.CreateAsync(user, "truckon777");
                await db.Companies.AddAsync(company);
                await db.SaveChangesAsync();
                var companyUser = new CompanyUser
                {
                    CompanyId = company.Id,
                    UserId = user.Id,
                };
                await db.CompanyUsers.AddAsync(companyUser);
                await db.SaveChangesAsync();
                await userManager.AddToRoleAsync(user, "Admin");

                companyService.InitStripeForCompany("Truckonnow Team", "truckonnow@test.com", company.Id);

                Console.WriteLine("Admin user and company successfully added");

            }
            
            if (db.Users.Any(x => x.UserName == "mrserge.husack@gmail.com") || db.Companies.Any(x => x.Name == "Serge Admin Company"))
            {
                Console.WriteLine("Serge Admin Company company or user already exists");
            }
            else
            {
                var user = new User
                {
                    Email = "mrserge.husack@gmail.com",
                    UserName = "mrserge.husack@gmail.com"
                };

                var phoneNumber = new PhoneNumber
                {
                    Name = "United States",
                    Iso2 = "us",
                    DialCode = 1,
                    Number = "(555)-555-5555",
                };

                var company = new Company
                {
                    Name = "Serge Admin Company",
                    CompanyType = CompanyType.Carrier,
                    CompanyStatus = CompanyStatus.Admin,
                    DateTimeRegistration = DateTime.Now,
                    PhoneNumber = phoneNumber,
                };

                await userManager.CreateAsync(user, "Welcome1");
                await db.Companies.AddAsync(company);
                await db.SaveChangesAsync();
                var companyUser = new CompanyUser
                {
                    CompanyId = company.Id,
                    UserId = user.Id,
                };
                await db.CompanyUsers.AddAsync(companyUser);
                await db.SaveChangesAsync();
                await userManager.AddToRoleAsync(user, "Admin");

                companyService.InitStripeForCompany("Serge Admin Company", "mrserge.husack@gmail.com", company.Id);

                Console.WriteLine("Serge Admin Company user and company successfully added");

            }
        }
        
        public async Task CreateStartTestCompany()
        {
            if (db.Users.Any(x => x.UserName == "truckonnow1@test.com") || db.Companies.Any(x => x.Name == "Truckonnow1"))
            {
                Console.WriteLine("User company or user (1) already exists");
            }
            else
            {
                var user = new User
                {
                    Email = "truckonnow1@test.com",
                    UserName = "truckonnow1@test.com"
                };

                var phoneNumber = new PhoneNumber
                {
                    Name = "United States",
                    Iso2 = "us",
                    DialCode = 1,
                    Number = "(555)-555-5555",
                };

                var company = new Company
                {
                    Name = "Truckonnow Team1",
                    CompanyType = CompanyType.Carrier,
                    CompanyStatus = CompanyStatus.Active,
                    DateTimeRegistration = DateTime.Now,
                    PhoneNumber = phoneNumber,
                };

                await db.Companies.AddAsync(company);
                await userManager.CreateAsync(user, "truckon777");
                await db.SaveChangesAsync();

                var companyUser = new CompanyUser
                {
                    CompanyId = company.Id,
                    UserId = user.Id,
                };

                await db.CompanyUsers.AddAsync(companyUser);
                await db.SaveChangesAsync();
                await userManager.AddToRoleAsync(user, "User");
                companyService.InitStripeForCompany("Truckonnow Team1", "truckonnow1@test.com", company.Id);
                Console.WriteLine("User user and company (1) successfully added");
            } 
            
            if (db.Users.Any(x => x.UserName == "truckonnow2@test.com") || db.Companies.Any(x => x.Name == "Truckonnow2"))
            {
                Console.WriteLine("User company or user (2) already exists");
            }
            else
            {
                var user = new User
                {
                    Email = "truckonnow2@test.com",
                    UserName = "truckonnow2@test.com"
                };

                var phoneNumber = new PhoneNumber
                {
                    Name = "United States",
                    Iso2 = "us",
                    DialCode = 1,
                    Number = "(555)-555-5555",
                };

                var company = new Company
                {
                    Name = "Truckonnow Team2",
                    CompanyType = CompanyType.Carrier,
                    CompanyStatus = CompanyStatus.Active,
                    DateTimeRegistration = DateTime.Now,
                    PhoneNumber = phoneNumber,
                };

                await db.Companies.AddAsync(company);
                await userManager.CreateAsync(user, "truckon777");
                await db.SaveChangesAsync();
                var companyUser = new CompanyUser
                {
                    CompanyId = company.Id,
                    UserId = user.Id,
                };
                await db.CompanyUsers.AddAsync(companyUser);
                await db.SaveChangesAsync();
                await userManager.AddToRoleAsync(user, "User");
                companyService.InitStripeForCompany("Truckonnow Team2", "truckonnow2@test.com", company.Id);
                Console.WriteLine("User user and company (2) successfully added");
            } 
            
            if (db.Users.Any(x => x.UserName == "truckonnow3@test.com") || db.Companies.Any(x => x.Name == "Truckonnow3"))
            {
                Console.WriteLine("User company or user (3) already exists");
            }
            else
            {
                var user = new User
                {
                    Email = "truckonnow3@test.com",
                    UserName = "truckonnow3@test.com"
                };

                var phoneNumber = new PhoneNumber
                {
                    Name = "United States",
                    Iso2 = "us",
                    DialCode = 1,
                    Number = "(555)-555-5555",
                };

                var company = new Company
                {
                    Name = "Truckonnow Team3",
                    CompanyType = CompanyType.Carrier,
                    CompanyStatus = CompanyStatus.Active,
                    DateTimeRegistration = DateTime.Now,
                    PhoneNumber = phoneNumber,
                };
                await db.Companies.AddAsync(company);
                await userManager.CreateAsync(user, "truckon777");
                await db.SaveChangesAsync();
                var companyUser = new CompanyUser
                {
                    CompanyId = company.Id,
                    UserId = user.Id,
                };
                await db.CompanyUsers.AddAsync(companyUser);
                await db.SaveChangesAsync();
                await userManager.AddToRoleAsync(user, "User");
                companyService.InitStripeForCompany("Truckonnow Team3", "truckonnow3@test.com", company.Id);
                Console.WriteLine("User user and company (3) successfully added");
            }
            
            if (db.Users.Any(x => x.UserName == "serge.husack@gmail.com") || db.Companies.Any(x => x.Name == "Serge Carrier Company"))
            {
                Console.WriteLine("Serge Carrier Company or user already exists");
            }
            else
            {
                var user = new User
                {
                    Email = "serge.husack@gmail.com",
                    UserName = "serge.husack@gmail.com"
                };
                var phoneNumber = new PhoneNumber
                {
                    Name = "United States",
                    Iso2 = "us",
                    DialCode = 1,
                    Number = "(555)-555-5555",
                };
                var company = new Company
                {
                    Name = "Serge Carrier Company",
                    CompanyType = CompanyType.Carrier,
                    CompanyStatus = CompanyStatus.Active,
                    DateTimeRegistration = DateTime.Now,
                    PhoneNumber = phoneNumber,
                };
                await db.Companies.AddAsync(company);
                await userManager.CreateAsync(user, "Welcome1");
                await db.SaveChangesAsync();
                var companyUser = new CompanyUser
                {
                    CompanyId = company.Id,
                    UserId = user.Id,
                };
                await db.CompanyUsers.AddAsync(companyUser);
                await db.SaveChangesAsync();
                await userManager.AddToRoleAsync(user, "User");
                companyService.InitStripeForCompany("Serge Carrier Company", "serge.husack@gmail.com", company.Id);
                Console.WriteLine("Serge Carrier Company user and company  successfully added");
            }
        }

        public async Task CreateBasicOrderStatuses()
        {
            await CreateCurrentStatus(OrderConstants.OrderStatusNewLoad);
            await CreateCurrentStatus(OrderConstants.OrderStatusAssigned);
            await CreateCurrentStatus(OrderConstants.OrderStatusPickedUp);
            await CreateCurrentStatus(OrderConstants.OrderStatusDelivered);
            await CreateCurrentStatus(OrderConstants.OrderStatusDeliveredBilled);
            await CreateCurrentStatus(OrderConstants.OrderStatusDeliveredPaid);
            await CreateCurrentStatus(OrderConstants.OrderStatusDeleted);
            await CreateCurrentStatus(OrderConstants.OrderStatusDeletedBilled);
            await CreateCurrentStatus(OrderConstants.OrderStatusDeletedPaid);
            await CreateCurrentStatus(OrderConstants.OrderStatusArchived);
            await CreateCurrentStatus(OrderConstants.OrderStatusArchivedBilled);
            await CreateCurrentStatus(OrderConstants.OrderStatusArchivedPaid);
        }

        public async Task CreateCurrentStatus(string orderStatus)
        {
            if (db.CurrentStatuses.Any(cs => cs.StatusName == orderStatus))
            {
                Console.WriteLine(orderStatus + " status already exists");
            }
            else
            {
                var currentStatusNewLoad = new CurrentStatus
                {
                    StatusName = orderStatus
                };

                await db.CurrentStatuses.AddAsync(currentStatusNewLoad);
                await db.SaveChangesAsync();
            }
        }
        
        public async Task CreateVehicleInfo()
        {
            await CreateCurrentVehicleType("Vehicle");
            await CreateCurrentVehicleType("Moto");

            await CreateCurrentVehicleBrand("Ford", "Vehicle");
            await CreateCurrentVehicleBrand("Chevrolet", "Vehicle");
            await CreateCurrentVehicleBrand("Toyota", "Vehicle");
            await CreateCurrentVehicleBrand("Honda", "Vehicle");
            await CreateCurrentVehicleBrand("Ram", "Vehicle");
            await CreateCurrentVehicleBrand("Nissan", "Vehicle");
            await CreateCurrentVehicleBrand("Jeep", "Vehicle");
            await CreateCurrentVehicleBrand("Hyundai", "Vehicle");
            await CreateCurrentVehicleBrand("Subaru", "Vehicle");
            await CreateCurrentVehicleBrand("GMC ", "Vehicle");

            await CreateCurrentVehicleBody("Sedan");
            await CreateCurrentVehicleBody("Coupe");
            await CreateCurrentVehicleBody("Crossover");
            await CreateCurrentVehicleBody("SUV");
            await CreateCurrentVehicleBody("Hatchback");
            await CreateCurrentVehicleBody("Pickup Truck");

            await CreateCurrentVehicleModel("Mustang", "Ford", "Coupe");
            await CreateCurrentVehicleModel("Fusion", "Ford", "Sedan");
            await CreateCurrentVehicleModel("Explorer", "Ford", "SUV");
            await CreateCurrentVehicleModel("Escape", "Ford", "Crossover");


            await CreateCurrentVehicleModel("Silverado", "Chevrolet", "Pickup Truck");
            await CreateCurrentVehicleModel("Equinox", "Chevrolet", "Crossover");
            await CreateCurrentVehicleModel("Traverse", "Chevrolet", "SUV");
            await CreateCurrentVehicleModel("Malibu", "Chevrolet", "Sedan");
            await CreateCurrentVehicleModel("Camaro", "Chevrolet", "Coupe");
            
            await CreateCurrentVehicleModel("Civic ", "Honda", "Sedan");
            await CreateCurrentVehicleModel("CR-V", "Honda", "Crossover");
            await CreateCurrentVehicleModel("Accord", "Honda", "Sedan");
            await CreateCurrentVehicleModel("Pilot", "Honda", "SUV");
            await CreateCurrentVehicleModel("CR-V", "Honda", "Hatchback");
        }

        public async Task CreateCurrentVehicleType(string typeName)
        {
            if (db.VehiclesTypes.Any(cs => cs.Name == typeName))
            {
                Console.WriteLine(typeName + " vehicle type already exists");
            }
            else
            {
                var currentVehicleType = new VehicleType
                {
                    Name = typeName
                };

                await db.VehiclesTypes.AddAsync(currentVehicleType);

                await db.SaveChangesAsync();
            }
        }
        
        public async Task CreateCurrentVehicleBrand(string typeName, string vehicleTypeName)
        {
            if (db.VehiclesBrands.Any(cs => cs.Name == typeName))
            {
                Console.WriteLine(typeName + " vehicle brand already exists");
            }
            else
            {
                var vehicleTypeId = db.VehiclesTypes.FirstOrDefault(vt => vt.Name == vehicleTypeName).Id;
                var currentVehicleBrand = new VehicleBrand
                {
                    Name = typeName,
                    VehicleTypeId = vehicleTypeId
                };

                await db.VehiclesBrands.AddAsync(currentVehicleBrand);

                await db.SaveChangesAsync();
            }
        }
        
        public async Task CreateCurrentVehicleBody(string typeName)
        {
            if (db.VehiclesBodies.Any(cs => cs.Name == typeName))
            {
                Console.WriteLine(typeName + " vehicle body already exists");
            }
            else
            {
                
                var currentVehicleBody = new VehicleBody
                {
                    Name = typeName
                };

                await db.VehiclesBodies.AddAsync(currentVehicleBody);

                await db.SaveChangesAsync();
            }
        }
        
        public async Task CreateCurrentVehicleModel(string typeName, string vehicleBrandName, string vehicleBodyName)
        {
            if (db.VehiclesModels.Any(cs => cs.Name == typeName))
            {
                Console.WriteLine(typeName + " vehicle model already exists");
            }
            else
            {
                var vehicleBrandId = db.VehiclesBrands.FirstOrDefault(vb => vb.Name == vehicleBrandName).Id;
                var vehicleBodyId = db.VehiclesBodies.FirstOrDefault(vb => vb.Name == vehicleBodyName).Id;
                var currentVehicleModel = new VehicleModel
                {
                    Name = typeName,
                    VehicleBrandId = vehicleBrandId,
                    VehicleBodyId = vehicleBodyId,
                };

                await db.VehiclesModels.AddAsync(currentVehicleModel);

                await db.SaveChangesAsync();
            }
        }

        public async Task CreateDispatcherType()
        {
            if (db.DispatchersTypes.Any(cs => cs.Name == "Central Dispatch"))
            {
                Console.WriteLine("Central Dispatch vehicle type already exists");
            }
            else
            {
                var dispatcherType = new DispatcherType
                {
                    Name = "Central Dispatch"
                };

                await db.DispatchersTypes.AddAsync(dispatcherType);

                await db.SaveChangesAsync();
            }
        }

        public async Task CreateEntityTypesResetPasswords()
        {
            if (db.RecoveriesTypes.Any(cs => cs.Name == UserConstants.UserEntityTypeResetPassword))
            {
                Console.WriteLine("User type already exists");
            }
            else
            {
                var recoveryType = new RecoveryType
                {
                    Name = UserConstants.UserEntityTypeResetPassword
                };

                await db.RecoveriesTypes.AddAsync(recoveryType);

                await db.SaveChangesAsync();
            }
            
            if (db.RecoveriesTypes.Any(cs => cs.Name == UserConstants.DriverEntityTypeResetPassword))
            {
                Console.WriteLine("Driver type already exists");
            }
            else
            {
                var recoveryType = new RecoveryType
                {
                    Name = UserConstants.DriverEntityTypeResetPassword
                };

                await db.RecoveriesTypes.AddAsync(recoveryType);

                await db.SaveChangesAsync();
            }
        } 
        
        public async Task CreateEntityPhotoTypes()
        {
            if (db.RecoveriesTypes.Any(cs => cs.Name == UserConstants.MarketplaceBuyEntityTypeResetPassword))
            {
                Console.WriteLine("Marketplace Buy type already exists");
            }
            else
            {
                var marketplaceType = new PhotoType
                {
                    Type = UserConstants.MarketplaceBuyEntityTypeResetPassword
                };

                await db.PhotoTypes.AddAsync(marketplaceType);

                await db.SaveChangesAsync();
            }
            
            if (db.RecoveriesTypes.Any(cs => cs.Name == UserConstants.MarketplaceSellEntityTypeResetPassword))
            {
                Console.WriteLine("Marketplace Sell type already exists");
            }
            else
            {
                var marketplaceType = new PhotoType
                {
                    Type = UserConstants.MarketplaceSellEntityTypeResetPassword
                };

                await db.PhotoTypes.AddAsync(marketplaceType);

                await db.SaveChangesAsync();
            }
        }
        
        
        public async Task InitVehicleCategory(string name)
        {
            if (db.VehiclesCategories.Any(cs => cs.Name == name))
            {
                Console.WriteLine($"{name} category already exists");
            }
            else
            {
                var vehicleCategory = new VehicleCategory
                {
                    Name = name
                };

                await db.VehiclesCategories.AddAsync(vehicleCategory);

                await db.SaveChangesAsync();
            }
        }

        public async Task InitVehiclesCategories()
        {
            await InitVehicleCategory("Pickup Truck");
            await InitVehicleCategory("Semi-Truck");
            await InitVehicleCategory("Box Truck");
            await InitVehicleCategory("Dump Truck");
            await InitVehicleCategory("Garbage Truck");
            await InitVehicleCategory("Grapple Truck");
            await InitVehicleCategory("Refrigerator Truck");
            await InitVehicleCategory("Tow Truck");
            await InitVehicleCategory("Van");
            await InitVehicleCategory("Tractor");
            await InitVehicleCategory("Motorhome");
            await InitVehicleCategory("Bus");
        }

        public async Task InitTruckTypeInVehicleCategory(string vehicleCategoryName, string truckTypeName)
        {
            var vehicleCategory = db.VehiclesCategories.FirstOrDefault(vc => vc.Name == vehicleCategoryName);
            if (vehicleCategory == null)
            {
                Console.WriteLine($"{vehicleCategoryName} not found");

                return;
            }

            if (db.TruckTypes.Any(cs => cs.Name == truckTypeName))
            {
                Console.WriteLine($"{truckTypeName} truck type already exists");
            }
            else
            {
                var truckType = new TruckType
                {
                    Name = truckTypeName,
                    VehicleCategoryId  = vehicleCategory.Id,
                };

                await db.TruckTypes.AddAsync(truckType);

                await db.SaveChangesAsync();
            }
        }

        public async Task InitTrucksTypesInVehicliesCategories()
        {
            List<(string, List<string>)> values = new List<(string, List<string>)>
            {
                ("Pickup Truck", new List<string>() {
                "2Dr Pickup with Single Wheel", "2Dr Pickup with Dually Wheels", "4Dr Crew Cab Pickup with Single Wheel", "4Dr Crew Cab Pickup with Dually Wheels",
                "2Dr Pickup ( flat bed ) Pickup with Single Wheel", "2Dr Pickup ( flat bed ) Pickup with  Dually Wheels", "4Dr Crew Cab ( flat bed ) Pickup with Single Wheel",
                "4Dr Crew Cab ( flat bed ) Pickup with  Dually Wheels" }),
                ("Semi-Truck",new List<string>() { "Day Cab", "Flat Roof Sleeper", "Mid-Roof Sleeper", "Raised Roof Sleeper"}),
                ("Box Truck", new List<string>() { "10-foot Standard Box Truck (capacity 2850 lbs.)", "12-foot truck Standard Box Truck (capacity 3100 lbs.)", "16-foot truck Standard Box Truck (capacity 4300 lbs.)",
                    "22-foot truck Standard Box Truck (capacity 10000 lbs.)", "24-foot truck Standard Box Truck (capacity 10000 lbs.)", "26-foot truck Standard Box Truck (capacity 12000 lbs.)",
                    "Utility Box Truck", "Refrigerated Box Truck", "Landscaping Box Truck", "Flatbed Box Truck"}),
                ("Dump Truck", new List<string>() { "Standard Dump Truck", "Side Dump Truck", "Tri-Axle Dump Truck", "Super Dump Truck", "Bottom Dump Truck", "Double Bottom Truck", "OFF-Highway Dump Truck"}),
                ("Garbage Truck", new List<string>() { "Front Loader", "Rear Loader", "Side Loader", "Manual Side Loader", "Automatic Side Loader", "Manual/Automatic Side Loader", "Semi-Automatic Side Loader", }),
                ("Grapple Truck", new List<string>() { "Combination Loader", "Rear-steering Design", "Rear-mounted Grappling Arm (no collection bin)", "Rear-mounted Grappling Arm with Connected Trailers",
                    "Grapple-enabled Roll-off Truck"}),
                ("Refrigerator Truck", new List<string>() { "Refrigerated with Lift Gate", "Full-Freezer Van", "Semi-Freezer Van", "Chiller Conversion Van", "Insulation Van"}),
                ("Tow Truck", new List<string>() {"Boom", "Wheel-Lift", "Integrated", "Flatbed", "Lift Flatbed"}),
                ("Van", new List<string>() { "Panel Van", "Crew Van", "Minibus", "Chassis Van", "Drop-side Van", "Tripper Van", "Box Van", "Luton Van", "Electric Van", }),
                ("Tractor", new List<string>() { "Industrial Tractor (tugger)", "Row Crop Tractor", "Orchard Tractor", "Compact Tractor", "Sub-Compact Tractor", "Utility Tractor", "Garden Tractor",
                    "Specialty Tractor", "Earthmoving Tractor",  }),
                ("Motorhome", new List<string>() { "Class A (29-45ft.)", "Class B (18-24ft.)", "Class C (30-33ft.)" }),
                ("Bus", new List<string>() { "Single-deck Bus", "Double-deck Bus", "Articulated Bus", "Trolley Bus", "School Bus", "Special needs Bus", "Coach/Motor Bus", "Shuttle Bus", "Party Bus",  }),

            };

            for(int i = 0; i < values.Count; i++)
            {
                var vehicleCategoryName = values[i].Item1;
                for(int j = 0;  j < values[i].Item2.Count; j++)
                {
                    await InitTruckTypeInVehicleCategory(vehicleCategoryName, values[i].Item2[j]);
                }
            }
        }

        public async Task SetSlugByTruckTypeName(string truckTypeName, string slug)
        {
            var truckTypes = db.TruckTypes.FirstOrDefault(vc => vc.Name == truckTypeName);
            if (truckTypes == null)
            {
                Console.WriteLine($"{truckTypes} not found");

                return;
            }

            truckTypes.Slug = slug;

            await db.SaveChangesAsync();
        }

        public async Task InitSlugsByTruckTypesNames()
        {
            await SetSlugByTruckTypeName("2Dr Pickup with Single Wheel", "2DrPickupwithSingleWheel");
            await SetSlugByTruckTypeName("2Dr Pickup with Dually Wheels", "2DrPickupwithDuallyWheels");
            await SetSlugByTruckTypeName("4Dr Crew Cab Pickup with Single Wheel", "4DrCrewCabPickupwithSingleWheel");
            await SetSlugByTruckTypeName("4Dr Crew Cab Pickup with Dually Wheels", "4DrCrewCabPickupwithDuallyWheels");
            await SetSlugByTruckTypeName("2Dr Pickup ( flat bed ) Pickup with Single Wheel", "2DrPickupPickupwithSingleWheel");
            await SetSlugByTruckTypeName("2Dr Pickup ( flat bed ) Pickup with  Dually Wheels", "2DrPickupPickupwithDuallyWheels");
            await SetSlugByTruckTypeName("4Dr Crew Cab ( flat bed ) Pickup with Single Wheel", "4DrCrewCabPickupwithSingleWheelFlatBed");
            await SetSlugByTruckTypeName("4Dr Crew Cab ( flat bed ) Pickup with  Dually Wheels", "4DrCrewCabPickupwithDuallyWheelsFlatBed");
        }
    }
}
