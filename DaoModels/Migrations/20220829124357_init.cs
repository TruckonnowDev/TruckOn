using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaoModels.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ask1s",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Exact_Mileage = table.Column<string>(nullable: true),
                    Did_you_notice_any_mechanical_imperfections_wile_loading = table.Column<string>(nullable: true),
                    Did_someone_help_you_load_it = table.Column<string>(nullable: true),
                    Did_someone_load_the_vehicle_for_you = table.Column<string>(nullable: true),
                    Did_you_Damage_anything_at_the_pick_up = table.Column<string>(nullable: true),
                    What_method_of_exit_did_you_use = table.Column<string>(nullable: true),
                    Did_you_jumped_the_vehicle_to_start = table.Column<string>(nullable: true),
                    Have_you_used_winch = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ask1s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ask2s",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    How_many_keys_total_you_been_given = table.Column<string>(nullable: true),
                    Car_locked = table.Column<string>(nullable: true),
                    Keys_location = table.Column<string>(nullable: true),
                    Client_friendliness = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ask2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AskDelyveries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Time_Of_Delivery = table.Column<string>(nullable: true),
                    Safe_delivery_location_Truck_and_trailer_parked_on = table.Column<string>(nullable: true),
                    Did_you_meet_the_client = table.Column<string>(nullable: true),
                    Truck_on_emergency_brake = table.Column<string>(nullable: true),
                    Truck_locked = table.Column<string>(nullable: true),
                    All_locks_on_the_trailer = table.Column<string>(nullable: true),
                    Vehicle_parked_in_the_safe_location = table.Column<string>(nullable: true),
                    Lightbrightness = table.Column<string>(nullable: true),
                    Vehicle_Condition_on_delivery = table.Column<string>(nullable: true),
                    Weather_Conditions = table.Column<string>(nullable: true),
                    How_did_you_get_inside_of_the_vehicle = table.Column<string>(nullable: true),
                    Did_the_vehicle_starts = table.Column<string>(nullable: true),
                    Does_the_vehicle_Drives = table.Column<string>(nullable: true),
                    Anyone_Rushing_you_to_perform_the_delivery = table.Column<string>(nullable: true),
                    How_Far_is_the_Trailer_from_Delivery_destination = table.Column<string>(nullable: true),
                    Exact_mileage_after_unloading = table.Column<string>(nullable: true),
                    Anyone_helping_you_unload = table.Column<string>(nullable: true),
                    Did_someone_else_unloaded_the_vehicle_for_you = table.Column<string>(nullable: true),
                    Did_you_notice_any_imperfections_on_body_wile_vehicle_been_transported = table.Column<string>(nullable: true),
                    After_inspecting_the_car_press_the_confirm_button = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AskDelyveries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Asks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Lightbrightness = table.Column<string>(nullable: true),
                    Vehicle = table.Column<string>(nullable: true),
                    Weather_conditions = table.Column<string>(nullable: true),
                    Anyone_Rushing_you_to_perform_the_inspection = table.Column<string>(nullable: true),
                    Plate = table.Column<string>(nullable: true),
                    TypeVehicle = table.Column<string>(nullable: true),
                    Safe_delivery_location = table.Column<string>(nullable: true),
                    How_far_from_trailer = table.Column<string>(nullable: true),
                    Name_of_the_person_who_gave_you_keys = table.Column<string>(nullable: true),
                    Enough_space_to_take_pictures = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Commpanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    DateRegistration = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commpanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Customer_STs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdCompany = table.Column<int>(nullable: false),
                    NameCompany = table.Column<string>(nullable: true),
                    NameCompanyST = table.Column<string>(nullable: true),
                    IdCustomerST = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer_STs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dispatchers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdCompany = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispatchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTruckAndTrailers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdTr = table.Column<int>(nullable: false),
                    NameDoc = table.Column<string>(nullable: true),
                    DocPath = table.Column<string>(nullable: true),
                    TypeDoc = table.Column<string>(nullable: true),
                    TypeTr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTruckAndTrailers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriverReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdDriver = table.Column<int>(nullable: false),
                    IdCompany = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    English = table.Column<string>(nullable: true),
                    ReturnedEquipmen = table.Column<string>(nullable: true),
                    WorkingEfficiency = table.Column<string>(nullable: true),
                    EldKnowledge = table.Column<string>(nullable: true),
                    DrivingSkills = table.Column<string>(nullable: true),
                    PaymentHandling = table.Column<string>(nullable: true),
                    AlcoholTendency = table.Column<string>(nullable: true),
                    DrugTendency = table.Column<string>(nullable: true),
                    Terminated = table.Column<string>(nullable: true),
                    DotViolations = table.Column<string>(nullable: true),
                    NumberOfAccidents = table.Column<string>(nullable: true),
                    Experience = table.Column<string>(nullable: true),
                    DriversLicenseNumber = table.Column<string>(nullable: true),
                    DateRegistration = table.Column<string>(nullable: true),
                    DateFired = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DucumentCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdCommpany = table.Column<int>(nullable: false),
                    NameDoc = table.Column<string>(nullable: true),
                    DocPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DucumentCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DucumentDrivers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdDriver = table.Column<int>(nullable: false),
                    NameDoc = table.Column<string>(nullable: true),
                    DocPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DucumentDrivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "geolocations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geolocations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HistoryOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdOreder = table.Column<string>(nullable: true),
                    IdUser = table.Column<int>(nullable: false),
                    IdDriver = table.Column<int>(nullable: false),
                    IdVech = table.Column<int>(nullable: false),
                    IdConmpany = table.Column<int>(nullable: false),
                    DateAction = table.Column<string>(nullable: true),
                    TypeAction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NamePaterns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamePaterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordRecoveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdDriver = table.Column<int>(nullable: false),
                    IdUser = table.Column<int>(nullable: false),
                    Date = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordRecoveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdCompany = table.Column<int>(nullable: false),
                    IdPaymentMethod_ST = table.Column<string>(nullable: true),
                    IdCustomerAttachPaymentMethod = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribe_STs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdCompany = table.Column<int>(nullable: false),
                    IdCustomerST = table.Column<string>(nullable: true),
                    IdSubscribeST = table.Column<string>(nullable: true),
                    IdItemSubscribeST = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ActiveType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribe_STs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    Make = table.Column<string>(nullable: true),
                    HowLong = table.Column<string>(nullable: true),
                    Vin = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Plate = table.Column<string>(nullable: true),
                    Exp = table.Column<string>(nullable: true),
                    AnnualIns = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountPhoto = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    IsNextInspection = table.Column<bool>(nullable: false),
                    TypeTransportVehicle = table.Column<string>(nullable: true),
                    PlateTruck = table.Column<string>(nullable: true),
                    PlateTraler = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    NameTruk = table.Column<string>(nullable: true),
                    Yera = table.Column<string>(nullable: true),
                    Make = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    Satet = table.Column<string>(nullable: true),
                    Exp = table.Column<string>(nullable: true),
                    Vin = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    PlateTruk = table.Column<string>(nullable: true),
                    ColorTruk = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    KeyAuthorized = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    VideoBase64 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    EmailOrLogin = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    TrailerCapacity = table.Column<string>(nullable: true),
                    DriversLicenseNumber = table.Column<string>(nullable: true),
                    IssuingState_Province = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    TokenShope = table.Column<string>(nullable: true),
                    IsInspectionDriver = table.Column<bool>(nullable: false),
                    IsInspectionToDayDriver = table.Column<bool>(nullable: false),
                    geolocationsID = table.Column<int>(nullable: true),
                    IsFired = table.Column<bool>(nullable: false),
                    LastDateInspaction = table.Column<string>(nullable: true),
                    DateRegistration = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_geolocations_geolocationsID",
                        column: x => x.geolocationsID,
                        principalTable: "geolocations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskLoads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NameMethod = table.Column<string>(nullable: true),
                    OptionalParameter = table.Column<string>(nullable: true),
                    IdDriver = table.Column<string>(nullable: true),
                    Array = table.Column<byte[]>(nullable: true),
                    LogTaskId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLoads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLoads_LogTasks_LogTaskId",
                        column: x => x.LogTaskId,
                        principalTable: "LogTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Layouts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Index = table.Column<string>(nullable: true),
                    OrdinalIndex = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false),
                    TransportVehicleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Layouts_TransportVehicles_TransportVehicleId",
                        column: x => x.TransportVehicleId,
                        principalTable: "TransportVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdCompany = table.Column<int>(nullable: false),
                    IdTr = table.Column<int>(nullable: false),
                    TypeTransportVehikle = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false),
                    TransportVehicleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSettings_TransportVehicles_TransportVehicleId",
                        column: x => x.TransportVehicleId,
                        principalTable: "TransportVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionDrivers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountPhotoTruck = table.Column<int>(nullable: false),
                    CountPhotoTrailer = table.Column<int>(nullable: false),
                    Date = table.Column<string>(nullable: true),
                    IdITruck = table.Column<int>(nullable: false),
                    IdITrailer = table.Column<int>(nullable: false),
                    DriverId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionDrivers_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhotoDrivers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdInspaction = table.Column<int>(nullable: false),
                    IndexPhoto = table.Column<int>(nullable: false),
                    path = table.Column<string>(nullable: true),
                    Width = table.Column<double>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    InspectionDriverId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoDrivers_InspectionDrivers_InspectionDriverId",
                        column: x => x.InspectionDriverId,
                        principalTable: "InspectionDrivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    path = table.Column<string>(nullable: true),
                    Width = table.Column<double>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    Ask1Id = table.Column<int>(nullable: true),
                    Ask1Id1 = table.Column<int>(nullable: true),
                    Ask2Id = table.Column<int>(nullable: true),
                    Ask2Id1 = table.Column<int>(nullable: true),
                    AskDelyveryID = table.Column<int>(nullable: true),
                    AskForUserDelyveryMID = table.Column<int>(nullable: true),
                    AskID = table.Column<int>(nullable: true),
                    PhotoInspectionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Ask1s_Ask1Id",
                        column: x => x.Ask1Id,
                        principalTable: "Ask1s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Photos_Ask1s_Ask1Id1",
                        column: x => x.Ask1Id1,
                        principalTable: "Ask1s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Photos_Ask2s_Ask2Id",
                        column: x => x.Ask2Id,
                        principalTable: "Ask2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Photos_Ask2s_Ask2Id1",
                        column: x => x.Ask2Id1,
                        principalTable: "Ask2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Photos_AskDelyveries_AskDelyveryID",
                        column: x => x.AskDelyveryID,
                        principalTable: "AskDelyveries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Photos_Asks_AskID",
                        column: x => x.AskID,
                        principalTable: "Asks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "askForUserDelyveryMs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up = table.Column<string>(nullable: true),
                    What_form_of_payment_are_you_using_to_pay_for_transportation = table.Column<string>(nullable: true),
                    CountPay = table.Column<string>(nullable: true),
                    PhotoPayId = table.Column<int>(nullable: true),
                    VideoRecordId = table.Column<int>(nullable: true),
                    EmailPay = table.Column<string>(nullable: true),
                    NamePaymment = table.Column<string>(nullable: true),
                    App_will_ask_for_name_of_the_client_signature = table.Column<string>(nullable: true),
                    App_will_ask_for_signature_of_the_client_signatureId = table.Column<int>(nullable: true),
                    Please_rate_the_driver = table.Column<string>(nullable: true),
                    IsProblem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_askForUserDelyveryMs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_askForUserDelyveryMs_Photos_App_will_ask_for_signature_of_the_client_signatureId",
                        column: x => x.App_will_ask_for_signature_of_the_client_signatureId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_askForUserDelyveryMs_Photos_PhotoPayId",
                        column: x => x.PhotoPayId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_askForUserDelyveryMs_Videos_VideoRecordId",
                        column: x => x.VideoRecordId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AskFromUsers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Your_Full_Name = table.Column<string>(nullable: true),
                    Your_phone = table.Column<string>(nullable: true),
                    How_many_keys_are_driver_been_given = table.Column<string>(nullable: true),
                    Any_titles_been_given_to_driver = table.Column<string>(nullable: true),
                    What_form_of_payment_are_you_using_to_pay_for_transportation = table.Column<string>(nullable: true),
                    App_will_ask_for_signature_of_the_client_signatureId = table.Column<int>(nullable: true),
                    CountPay = table.Column<string>(nullable: true),
                    PhotoPayId = table.Column<int>(nullable: true),
                    VideoRecordId = table.Column<int>(nullable: true),
                    EmailPay = table.Column<string>(nullable: true),
                    NamePaymment = table.Column<string>(nullable: true),
                    IsProblem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AskFromUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_AskFromUsers_Photos_App_will_ask_for_signature_of_the_client_signatureId",
                        column: x => x.App_will_ask_for_signature_of_the_client_signatureId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AskFromUsers_Photos_PhotoPayId",
                        column: x => x.PhotoPayId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AskFromUsers_Videos_VideoRecordId",
                        column: x => x.VideoRecordId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipping",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    idOrder = table.Column<string>(nullable: true),
                    InternalLoadID = table.Column<string>(nullable: true),
                    CurrentStatus = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<string>(nullable: true),
                    CDReference = table.Column<string>(nullable: true),
                    UrlReqvest = table.Column<string>(nullable: true),
                    DispatchDate = table.Column<string>(nullable: true),
                    PickupExactly = table.Column<string>(nullable: true),
                    DeliveryEstimated = table.Column<string>(nullable: true),
                    ShipVia = table.Column<string>(nullable: true),
                    Condition = table.Column<string>(nullable: true),
                    PriceListed = table.Column<string>(nullable: true),
                    TotalPaymentToCarrier = table.Column<string>(nullable: true),
                    OnDeliveryToCarrier = table.Column<string>(nullable: true),
                    CompanyOwesCarrier = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    BrokerFee = table.Column<string>(nullable: true),
                    ContactC = table.Column<string>(nullable: true),
                    PhoneC = table.Column<string>(nullable: true),
                    FaxC = table.Column<string>(nullable: true),
                    IccmcC = table.Column<string>(nullable: true),
                    NameP = table.Column<string>(nullable: true),
                    ContactNameP = table.Column<string>(nullable: true),
                    AddresP = table.Column<string>(nullable: true),
                    StateP = table.Column<string>(nullable: true),
                    ZipP = table.Column<string>(nullable: true),
                    CityP = table.Column<string>(nullable: true),
                    PhoneP = table.Column<string>(nullable: true),
                    EmailP = table.Column<string>(nullable: true),
                    NameD = table.Column<string>(nullable: true),
                    ContactNameD = table.Column<string>(nullable: true),
                    AddresD = table.Column<string>(nullable: true),
                    StateD = table.Column<string>(nullable: true),
                    ZipD = table.Column<string>(nullable: true),
                    CityD = table.Column<string>(nullable: true),
                    PhoneD = table.Column<string>(nullable: true),
                    EmailD = table.Column<string>(nullable: true),
                    Titl1DI = table.Column<string>(nullable: true),
                    AskFromUserid = table.Column<int>(nullable: true),
                    Ask2Id = table.Column<int>(nullable: true),
                    askForUserDelyveryMID = table.Column<int>(nullable: true),
                    IdDriver = table.Column<int>(nullable: false),
                    DataPaid = table.Column<string>(nullable: true),
                    DataCancelOrder = table.Column<string>(nullable: true),
                    DataFullArcive = table.Column<string>(nullable: true),
                    IsProblem = table.Column<bool>(nullable: false),
                    IsInstructinRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipping_Ask2s_Ask2Id",
                        column: x => x.Ask2Id,
                        principalTable: "Ask2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipping_AskFromUsers_AskFromUserid",
                        column: x => x.AskFromUserid,
                        principalTable: "AskFromUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipping_askForUserDelyveryMs_askForUserDelyveryMID",
                        column: x => x.askForUserDelyveryMID,
                        principalTable: "askForUserDelyveryMs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DamageForUsers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypePrefDamage = table.Column<string>(nullable: true),
                    TypeDamage = table.Column<string>(nullable: true),
                    TypeCurrentStatus = table.Column<string>(nullable: true),
                    IndexDamage = table.Column<int>(nullable: false),
                    FullNameDamage = table.Column<string>(nullable: true),
                    XInterest = table.Column<double>(nullable: false),
                    YInterest = table.Column<double>(nullable: false),
                    HeightDamage = table.Column<int>(nullable: false),
                    WidthDamage = table.Column<int>(nullable: false),
                    ShippingId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageForUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DamageForUsers_Shipping_ShippingId",
                        column: x => x.ShippingId,
                        principalTable: "Shipping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShippingId = table.Column<string>(nullable: true),
                    How_Are_You_Satisfied_With_Service = table.Column<string>(nullable: true),
                    Would_You_Use_Our_Company_Again = table.Column<string>(nullable: true),
                    Would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion = table.Column<string>(nullable: true),
                    How_did_the_driver_perform = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Shipping_ShippingId",
                        column: x => x.ShippingId,
                        principalTable: "Shipping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehiclwInformation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Year = table.Column<string>(nullable: true),
                    Make = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Plate = table.Column<string>(nullable: true),
                    VIN = table.Column<string>(nullable: true),
                    Lot = table.Column<string>(nullable: true),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    AskID = table.Column<int>(nullable: true),
                    Ask1Id = table.Column<int>(nullable: true),
                    AskDelyveryID = table.Column<int>(nullable: true),
                    ScanId = table.Column<int>(nullable: true),
                    ShippingId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclwInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclwInformation_Ask1s_Ask1Id",
                        column: x => x.Ask1Id,
                        principalTable: "Ask1s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehiclwInformation_AskDelyveries_AskDelyveryID",
                        column: x => x.AskDelyveryID,
                        principalTable: "AskDelyveries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehiclwInformation_Asks_AskID",
                        column: x => x.AskID,
                        principalTable: "Asks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehiclwInformation_Photos_ScanId",
                        column: x => x.ScanId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehiclwInformation_Shipping_ShippingId",
                        column: x => x.ShippingId,
                        principalTable: "Shipping",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhotoInspections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IndexPhoto = table.Column<int>(nullable: false),
                    CurrentStatusPhoto = table.Column<string>(nullable: true),
                    VehiclwInformationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoInspections_VehiclwInformation_VehiclwInformationId",
                        column: x => x.VehiclwInformationId,
                        principalTable: "VehiclwInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Damages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IndexImageVech = table.Column<string>(nullable: true),
                    TypePrefDamage = table.Column<string>(nullable: true),
                    TypeDamage = table.Column<string>(nullable: true),
                    TypeCurrentStatus = table.Column<string>(nullable: true),
                    IndexDamage = table.Column<int>(nullable: false),
                    FullNameDamage = table.Column<string>(nullable: true),
                    XInterest = table.Column<double>(nullable: false),
                    YInterest = table.Column<double>(nullable: false),
                    HeightDamage = table.Column<int>(nullable: false),
                    WidthDamage = table.Column<int>(nullable: false),
                    PhotoInspectionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Damages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Damages_PhotoInspections_PhotoInspectionId",
                        column: x => x.PhotoInspectionId,
                        principalTable: "PhotoInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_askForUserDelyveryMs_App_will_ask_for_signature_of_the_client_signatureId",
                table: "askForUserDelyveryMs",
                column: "App_will_ask_for_signature_of_the_client_signatureId");

            migrationBuilder.CreateIndex(
                name: "IX_askForUserDelyveryMs_PhotoPayId",
                table: "askForUserDelyveryMs",
                column: "PhotoPayId");

            migrationBuilder.CreateIndex(
                name: "IX_askForUserDelyveryMs_VideoRecordId",
                table: "askForUserDelyveryMs",
                column: "VideoRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AskFromUsers_App_will_ask_for_signature_of_the_client_signatureId",
                table: "AskFromUsers",
                column: "App_will_ask_for_signature_of_the_client_signatureId");

            migrationBuilder.CreateIndex(
                name: "IX_AskFromUsers_PhotoPayId",
                table: "AskFromUsers",
                column: "PhotoPayId");

            migrationBuilder.CreateIndex(
                name: "IX_AskFromUsers_VideoRecordId",
                table: "AskFromUsers",
                column: "VideoRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageForUsers_ShippingId",
                table: "DamageForUsers",
                column: "ShippingId");

            migrationBuilder.CreateIndex(
                name: "IX_Damages_PhotoInspectionId",
                table: "Damages",
                column: "PhotoInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_geolocationsID",
                table: "Drivers",
                column: "geolocationsID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ShippingId",
                table: "Feedbacks",
                column: "ShippingId",
                unique: true,
                filter: "[ShippingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionDrivers_DriverId",
                table: "InspectionDrivers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_TransportVehicleId",
                table: "Layouts",
                column: "TransportVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoDrivers_InspectionDriverId",
                table: "PhotoDrivers",
                column: "InspectionDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoInspections_VehiclwInformationId",
                table: "PhotoInspections",
                column: "VehiclwInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Ask1Id",
                table: "Photos",
                column: "Ask1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Ask1Id1",
                table: "Photos",
                column: "Ask1Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Ask2Id",
                table: "Photos",
                column: "Ask2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_Ask2Id1",
                table: "Photos",
                column: "Ask2Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AskDelyveryID",
                table: "Photos",
                column: "AskDelyveryID");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AskForUserDelyveryMID",
                table: "Photos",
                column: "AskForUserDelyveryMID");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_AskID",
                table: "Photos",
                column: "AskID");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoInspectionId",
                table: "Photos",
                column: "PhotoInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSettings_TransportVehicleId",
                table: "ProfileSettings",
                column: "TransportVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_Ask2Id",
                table: "Shipping",
                column: "Ask2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_AskFromUserid",
                table: "Shipping",
                column: "AskFromUserid");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_askForUserDelyveryMID",
                table: "Shipping",
                column: "askForUserDelyveryMID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLoads_LogTaskId",
                table: "TaskLoads",
                column: "LogTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclwInformation_Ask1Id",
                table: "VehiclwInformation",
                column: "Ask1Id");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclwInformation_AskDelyveryID",
                table: "VehiclwInformation",
                column: "AskDelyveryID");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclwInformation_AskID",
                table: "VehiclwInformation",
                column: "AskID");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclwInformation_ScanId",
                table: "VehiclwInformation",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclwInformation_ShippingId",
                table: "VehiclwInformation",
                column: "ShippingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_PhotoInspections_PhotoInspectionId",
                table: "Photos",
                column: "PhotoInspectionId",
                principalTable: "PhotoInspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_askForUserDelyveryMs_AskForUserDelyveryMID",
                table: "Photos",
                column: "AskForUserDelyveryMID",
                principalTable: "askForUserDelyveryMs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_askForUserDelyveryMs_Photos_App_will_ask_for_signature_of_the_client_signatureId",
                table: "askForUserDelyveryMs");

            migrationBuilder.DropForeignKey(
                name: "FK_askForUserDelyveryMs_Photos_PhotoPayId",
                table: "askForUserDelyveryMs");

            migrationBuilder.DropForeignKey(
                name: "FK_AskFromUsers_Photos_App_will_ask_for_signature_of_the_client_signatureId",
                table: "AskFromUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AskFromUsers_Photos_PhotoPayId",
                table: "AskFromUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_VehiclwInformation_Photos_ScanId",
                table: "VehiclwInformation");

            migrationBuilder.DropTable(
                name: "Commpanies");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Customer_STs");

            migrationBuilder.DropTable(
                name: "DamageForUsers");

            migrationBuilder.DropTable(
                name: "Damages");

            migrationBuilder.DropTable(
                name: "Dispatchers");

            migrationBuilder.DropTable(
                name: "DocumentTruckAndTrailers");

            migrationBuilder.DropTable(
                name: "DriverReports");

            migrationBuilder.DropTable(
                name: "DucumentCompanies");

            migrationBuilder.DropTable(
                name: "DucumentDrivers");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "HistoryOrders");

            migrationBuilder.DropTable(
                name: "Layouts");

            migrationBuilder.DropTable(
                name: "NamePaterns");

            migrationBuilder.DropTable(
                name: "PasswordRecoveries");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "PhotoDrivers");

            migrationBuilder.DropTable(
                name: "ProfileSettings");

            migrationBuilder.DropTable(
                name: "Subscribe_STs");

            migrationBuilder.DropTable(
                name: "TaskLoads");

            migrationBuilder.DropTable(
                name: "Trailers");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "InspectionDrivers");

            migrationBuilder.DropTable(
                name: "TransportVehicles");

            migrationBuilder.DropTable(
                name: "LogTasks");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "geolocations");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "PhotoInspections");

            migrationBuilder.DropTable(
                name: "VehiclwInformation");

            migrationBuilder.DropTable(
                name: "Ask1s");

            migrationBuilder.DropTable(
                name: "AskDelyveries");

            migrationBuilder.DropTable(
                name: "Asks");

            migrationBuilder.DropTable(
                name: "Shipping");

            migrationBuilder.DropTable(
                name: "Ask2s");

            migrationBuilder.DropTable(
                name: "AskFromUsers");

            migrationBuilder.DropTable(
                name: "askForUserDelyveryMs");

            migrationBuilder.DropTable(
                name: "Videos");
        }
    }
}
