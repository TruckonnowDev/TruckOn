using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaoModels.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    DateTimeRegistration = table.Column<DateTime>(nullable: false),
                    DateTimeLastUpdate = table.Column<DateTime>(nullable: true),
                    DateTimeLastLogin = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DispatchersTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchersTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriversControls",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(nullable: true),
                    TrailerCapacity = table.Column<string>(nullable: true),
                    IssuingStateProvince = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    TokenShope = table.Column<string>(nullable: true),
                    IsInspectionDriver = table.Column<bool>(nullable: false),
                    IsInspectionToDayDriver = table.Column<bool>(nullable: false),
                    LastDateInspaction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriversControls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriversReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlcoholTendency = table.Column<string>(nullable: true),
                    DriverSkills = table.Column<string>(nullable: true),
                    DrugTendency = table.Column<string>(nullable: true),
                    EldKnowledge = table.Column<string>(nullable: true),
                    English = table.Column<string>(nullable: true),
                    Experience = table.Column<string>(nullable: true),
                    PaymentHandling = table.Column<string>(nullable: true),
                    ReturnEquipment = table.Column<string>(nullable: true),
                    Terminated = table.Column<string>(nullable: true),
                    WorkEffeciency = table.Column<string>(nullable: true),
                    DotViolations = table.Column<string>(nullable: true),
                    NumberAccidents = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    DateFired = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriversReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Geolocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geolocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoriesDriversActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ActionType = table.Column<int>(nullable: false),
                    ChangedField = table.Column<string>(nullable: true),
                    ContentBefore = table.Column<string>(nullable: true),
                    ContentAfter = table.Column<string>(nullable: true),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriesDriversActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoriesMarketPostsActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketPostId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ActionType = table.Column<int>(nullable: false),
                    ChangedField = table.Column<string>(nullable: true),
                    ContentBefore = table.Column<string>(nullable: true),
                    ContentAfter = table.Column<string>(nullable: true),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriesMarketPostsActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoriesTrailerActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrailerId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ActionType = table.Column<int>(nullable: false),
                    ChangedField = table.Column<string>(nullable: true),
                    ContentBefore = table.Column<string>(nullable: true),
                    ContentAfter = table.Column<string>(nullable: true),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriesTrailerActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoriesTrucksActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TruckId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    ActionType = table.Column<int>(nullable: false),
                    ChangedField = table.Column<string>(nullable: true),
                    ContentBefore = table.Column<string>(nullable: true),
                    ContentAfter = table.Column<string>(nullable: true),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriesTrucksActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhonesNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Iso2 = table.Column<string>(nullable: true),
                    DialCode = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhonesNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhotosListMPs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotosListMPs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhotoTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questionnaires",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecoveriesTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveriesTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "VehiclesBodies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesBodies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehiclesCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehiclesTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    ConditionPost = table.Column<int>(nullable: false),
                    ShowView = table.Column<bool>(nullable: false),
                    ShowComment = table.Column<bool>(nullable: false),
                    DateTimeLastUpdate = table.Column<DateTime>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AddressInformations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressInformations_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    USDOT = table.Column<int>(nullable: false),
                    CompanyType = table.Column<int>(nullable: false),
                    CompanyStatus = table.Column<int>(nullable: false),
                    DateTimeRegistration = table.Column<DateTime>(nullable: false),
                    DateTimeLastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersMailsQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderName = table.Column<string>(nullable: true),
                    SenderEmail = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    MailStatus = table.Column<int>(nullable: false),
                    DateTimeReceived = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersMailsQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersMailsQuestions_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoTypeId = table.Column<int>(nullable: false),
                    PhotoPath = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    Height = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false),
                    DateTimeUpload = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_PhotoTypes_PhotoTypeId",
                        column: x => x.PhotoTypeId,
                        principalTable: "PhotoTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotosMP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    PhotoListMPId = table.Column<int>(nullable: false),
                    PhotoTypeId = table.Column<int>(nullable: false),
                    PhotoPath = table.Column<string>(nullable: true),
                    PhotoUrl = table.Column<string>(nullable: true),
                    Height = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    DateTimeUpload = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotosMP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotosMP_PhotosListMPs_PhotoListMPId",
                        column: x => x.PhotoListMPId,
                        principalTable: "PhotosListMPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotosMP_PhotoTypes_PhotoTypeId",
                        column: x => x.PhotoTypeId,
                        principalTable: "PhotoTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    QuestionnaireId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordsRecoveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityRecoveryId = table.Column<string>(nullable: true),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    RecoveryTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordsRecoveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordsRecoveries_RecoveriesTypes_RecoveryTypeId",
                        column: x => x.RecoveryTypeId,
                        principalTable: "RecoveriesTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Layouts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "TruckTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    VehicleCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TruckTypes_VehiclesCategories_VehicleCategoryId",
                        column: x => x.VehicleCategoryId,
                        principalTable: "VehiclesCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiclesBrands",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    VehicleTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesBrands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclesBrands_VehiclesTypes_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehiclesTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoTypeId = table.Column<int>(nullable: false),
                    VideoPath = table.Column<string>(nullable: true),
                    VideoUrl = table.Column<string>(nullable: true),
                    Height = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false),
                    DateTimeUpload = table.Column<DateTime>(nullable: false),
                    VideoBase64 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_VideoTypes_VideoTypeId",
                        column: x => x.VideoTypeId,
                        principalTable: "VideoTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyItemsMarketsPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    MarketPostId = table.Column<int>(nullable: false),
                    PhotoListMPId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyItemsMarketsPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyItemsMarketsPosts_MarketPosts_MarketPostId",
                        column: x => x.MarketPostId,
                        principalTable: "MarketPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyItemsMarketsPosts_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyItemsMarketsPosts_PhotosListMPs_PhotoListMPId",
                        column: x => x.PhotoListMPId,
                        principalTable: "PhotosListMPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckoutMPs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketPostId = table.Column<int>(nullable: false),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    BuyerId = table.Column<string>(nullable: true),
                    SellerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutMPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckoutMPs_AspNetUsers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckoutMPs_MarketPosts_MarketPostId",
                        column: x => x.MarketPostId,
                        principalTable: "MarketPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckoutMPs_AspNetUsers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentsMarketsPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    MarketPostId = table.Column<int>(nullable: false),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsMarketsPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentsMarketsPosts_MarketPosts_MarketPostId",
                        column: x => x.MarketPostId,
                        principalTable: "MarketPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsMarketsPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SellItemsMarketsPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    ConditionItem = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    MarketPostId = table.Column<int>(nullable: false),
                    PhotoListMPId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellItemsMarketsPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellItemsMarketsPosts_MarketPosts_MarketPostId",
                        column: x => x.MarketPostId,
                        principalTable: "MarketPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellItemsMarketsPosts_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SellItemsMarketsPosts_PhotosListMPs_PhotoListMPId",
                        column: x => x.PhotoListMPId,
                        principalTable: "PhotosListMPs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewsMarketsPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    MarketPostId = table.Column<int>(nullable: false),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewsMarketsPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewsMarketsPosts_MarketPosts_MarketPostId",
                        column: x => x.MarketPostId,
                        principalTable: "MarketPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewsMarketsPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUsers",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUsers", x => new { x.CompanyId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CompanyUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    Ext = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false),
                    DateTimeLastUpdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contacts_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerST",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    NameCompanyST = table.Column<string>(nullable: true),
                    IdCustomerST = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerST", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerST_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dispatchers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    DispatcherTypeId = table.Column<int>(nullable: false),
                    Login = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispatchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispatchers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dispatchers_DispatchersTypes_DispatcherTypeId",
                        column: x => x.DispatcherTypeId,
                        principalTable: "DispatchersTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    DocPath = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    DateTimeUpload = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    DriverLicenseNumber = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    DriverControlId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    GeolocationId = table.Column<int>(nullable: true),
                    DriverReportId = table.Column<int>(nullable: true),
                    DateRegistration = table.Column<DateTime>(nullable: false),
                    DateLastUpdate = table.Column<DateTime>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Drivers_DriversControls_DriverControlId",
                        column: x => x.DriverControlId,
                        principalTable: "DriversControls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Drivers_DriversReports_DriverReportId",
                        column: x => x.DriverReportId,
                        principalTable: "DriversReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Drivers_Geolocations_GeolocationId",
                        column: x => x.GeolocationId,
                        principalTable: "Geolocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Drivers_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentsMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    PaymentMethodSTId = table.Column<string>(nullable: true),
                    CustomerAttachPaymentMethodId = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentsMethods_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminName = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    DateTimeAnswer = table.Column<DateTime>(nullable: false),
                    UserMailQuestionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminAnswers_UsersMailsQuestions_UserMailQuestionId",
                        column: x => x.UserMailQuestionId,
                        principalTable: "UsersMailsQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViewsUsersMailsQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    UserMailQuestionId = table.Column<int>(nullable: false),
                    DateTimeAction = table.Column<DateTime>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewsUsersMailsQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewsUsersMailsQuestions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViewsUsersMailsQuestions_UsersMailsQuestions_UserMailQuestionId",
                        column: x => x.UserMailQuestionId,
                        principalTable: "UsersMailsQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    QuestionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiclesModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    VehicleBrandId = table.Column<int>(nullable: false),
                    VehicleBodyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclesModels_VehiclesBodies_VehicleBodyId",
                        column: x => x.VehicleBodyId,
                        principalTable: "VehiclesBodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehiclesModels_VehiclesBrands_VehicleBrandId",
                        column: x => x.VehicleBrandId,
                        principalTable: "VehiclesBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscribeST",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerSTId = table.Column<int>(nullable: false),
                    SubscribeSTId = table.Column<string>(nullable: true),
                    ItemSubscribeSTId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ActiveType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribeST", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscribeST_CustomerST_CustomerSTId",
                        column: x => x.CustomerSTId,
                        principalTable: "CustomerST",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsDrivers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    DocPath = table.Column<string>(nullable: true),
                    DriverId = table.Column<int>(nullable: false),
                    DateTimeUpload = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsDrivers_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriversQuestionnaires",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<int>(nullable: false),
                    QuestionnaireId = table.Column<int>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    DateTimeCompleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriversQuestionnaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriversQuestionnaires_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriversQuestionnaires_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    PhoneNumberId = table.Column<int>(nullable: true),
                    FaxNumberId = table.Column<int>(nullable: true),
                    McNumber = table.Column<string>(nullable: true),
                    Instructions = table.Column<string>(nullable: true),
                    PaymentMethod = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    BrokerFee = table.Column<decimal>(nullable: false),
                    UrlRequest = table.Column<string>(nullable: true),
                    DriverId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    CurrentStatusId = table.Column<int>(nullable: false),
                    PickedUpId = table.Column<int>(nullable: true),
                    DateTimePickedUp = table.Column<DateTime>(nullable: false),
                    DeliveryId = table.Column<int>(nullable: true),
                    DateTimeDelivery = table.Column<DateTime>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false),
                    DateTimeLastUpdate = table.Column<DateTime>(nullable: false),
                    DateTimePaid = table.Column<DateTime>(nullable: false),
                    DateTimeCancelOrder = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_CurrentStatuses_CurrentStatusId",
                        column: x => x.CurrentStatusId,
                        principalTable: "CurrentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_AddressInformations_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "AddressInformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_PhonesNumbers_FaxNumberId",
                        column: x => x.FaxNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_PhonesNumbers_PhoneNumberId",
                        column: x => x.PhoneNumberId,
                        principalTable: "PhonesNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_AddressInformations_PickedUpId",
                        column: x => x.PickedUpId,
                        principalTable: "AddressInformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Brand = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    HowLong = table.Column<string>(nullable: true),
                    Vin = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Plate = table.Column<string>(nullable: true),
                    PlateExpires = table.Column<DateTime>(nullable: true),
                    AnnualIns = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false),
                    DateTimeLastUpdate = table.Column<DateTime>(nullable: false),
                    VehicleModelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trailers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trailers_VehiclesModels_VehicleModelId",
                        column: x => x.VehicleModelId,
                        principalTable: "VehiclesModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Brand = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    PlateExpires = table.Column<DateTime>(nullable: true),
                    VIN = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    Plate = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    TruckTypeId = table.Column<int>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false),
                    DateTimeLastUpdate = table.Column<DateTime>(nullable: false),
                    VehicleModelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trucks_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trucks_TruckTypes_TruckTypeId",
                        column: x => x.TruckTypeId,
                        principalTable: "TruckTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trucks_VehiclesModels_VehicleModelId",
                        column: x => x.VehicleModelId,
                        principalTable: "VehiclesModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhotosAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(nullable: false),
                    DriverQuestionnaireId = table.Column<int>(nullable: true),
                    PhotoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotosAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotosAnswers_DriversQuestionnaires_DriverQuestionnaireId",
                        column: x => x.DriverQuestionnaireId,
                        principalTable: "DriversQuestionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhotosAnswers_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotosAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(nullable: false),
                    DriverQuestionnaireId = table.Column<int>(nullable: true),
                    TextAnswer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersAnswers_DriversQuestionnaires_DriverQuestionnaireId",
                        column: x => x.DriverQuestionnaireId,
                        principalTable: "DriversQuestionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideosAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(nullable: false),
                    DriverQuestionnaireId = table.Column<int>(nullable: true),
                    VideoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideosAnswers_DriversQuestionnaires_DriverQuestionnaireId",
                        column: x => x.DriverQuestionnaireId,
                        principalTable: "DriversQuestionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VideosAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideosAnswers_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DamageForUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypePrefDamage = table.Column<string>(nullable: true),
                    TypeDamage = table.Column<string>(nullable: true),
                    TypeCurrentStatus = table.Column<string>(nullable: true),
                    IndexDamage = table.Column<int>(nullable: false),
                    FullNameDamage = table.Column<string>(nullable: true),
                    XInterest = table.Column<double>(nullable: false),
                    YInterest = table.Column<double>(nullable: false),
                    HeightDamage = table.Column<int>(nullable: false),
                    WidthDamage = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageForUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageForUsers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoriesOrdersActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: false),
                    VehicleId = table.Column<int>(nullable: true),
                    ActionType = table.Column<int>(nullable: false),
                    FieldAction = table.Column<string>(nullable: true),
                    ContentFrom = table.Column<string>(nullable: true),
                    ContentTo = table.Column<string>(nullable: true),
                    DateTimeAction = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriesOrdersActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriesOrdersActions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiclesDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleModelId = table.Column<int>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Plate = table.Column<string>(nullable: true),
                    VIN = table.Column<string>(nullable: true),
                    Lot = table.Column<string>(nullable: true),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclesDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehiclesDetails_VehiclesModels_VehicleModelId",
                        column: x => x.VehicleModelId,
                        principalTable: "VehiclesModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsTrailers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    DocPath = table.Column<string>(nullable: true),
                    TrailerId = table.Column<int>(nullable: false),
                    DateTimeUpload = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsTrailers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsTrailers_Trailers_TrailerId",
                        column: x => x.TrailerId,
                        principalTable: "Trailers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsTrucks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    DocPath = table.Column<string>(nullable: true),
                    TruckId = table.Column<int>(nullable: false),
                    DateTimeUpload = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsTrucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsTrucks_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriversInspections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTimeInspection = table.Column<DateTime>(nullable: false),
                    DriverId = table.Column<int>(nullable: true),
                    TruckId = table.Column<int>(nullable: true),
                    TrailerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriversInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriversInspections_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriversInspections_Trailers_TrailerId",
                        column: x => x.TrailerId,
                        principalTable: "Trailers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriversInspections_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehiclesInspections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTimeInspection = table.Column<DateTime>(nullable: false),
                    VehicleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclesInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclesInspections_VehiclesDetails_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "VehiclesDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhotosDriversInspections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    DriverInspectionId = table.Column<int>(nullable: false),
                    IndexPhoto = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotosDriversInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotosDriversInspections_DriversInspections_DriverInspectionId",
                        column: x => x.DriverInspectionId,
                        principalTable: "DriversInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotosDriversInspections_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotosVehiclesInspections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoId = table.Column<int>(nullable: false),
                    VehicleInspectionId = table.Column<int>(nullable: false),
                    IndexPhoto = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotosVehiclesInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotosVehiclesInspections_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotosVehiclesInspections_VehiclesInspections_VehicleInspectionId",
                        column: x => x.VehicleInspectionId,
                        principalTable: "VehiclesInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Damages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoVehicleInspectionId = table.Column<int>(nullable: false),
                    IndexImageVech = table.Column<string>(nullable: true),
                    TypePrefDamage = table.Column<string>(nullable: true),
                    TypeDamage = table.Column<string>(nullable: true),
                    TypeCurrentStatus = table.Column<string>(nullable: true),
                    IndexDamage = table.Column<int>(nullable: false),
                    FullNameDamage = table.Column<string>(nullable: true),
                    XInterest = table.Column<double>(nullable: false),
                    YInterest = table.Column<double>(nullable: false),
                    HeightDamage = table.Column<int>(nullable: false),
                    WidthDamage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Damages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Damages_PhotosVehiclesInspections_PhotoVehicleInspectionId",
                        column: x => x.PhotoVehicleInspectionId,
                        principalTable: "PhotosVehiclesInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressInformations_PhoneNumberId",
                table: "AddressInformations",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAnswers_UserMailQuestionId",
                table: "AdminAnswers",
                column: "UserMailQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_QuestionId",
                table: "AnswerOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BuyItemsMarketsPosts_MarketPostId",
                table: "BuyItemsMarketsPosts",
                column: "MarketPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyItemsMarketsPosts_PhoneNumberId",
                table: "BuyItemsMarketsPosts",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyItemsMarketsPosts_PhotoListMPId",
                table: "BuyItemsMarketsPosts",
                column: "PhotoListMPId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutMPs_BuyerId",
                table: "CheckoutMPs",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutMPs_MarketPostId",
                table: "CheckoutMPs",
                column: "MarketPostId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutMPs_SellerId",
                table: "CheckoutMPs",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsMarketsPosts_MarketPostId",
                table: "CommentsMarketsPosts",
                column: "MarketPostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsMarketsPosts_UserId",
                table: "CommentsMarketsPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PhoneNumberId",
                table: "Companies",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_UserId",
                table: "CompanyUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CompanyId",
                table: "Contacts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_PhoneNumberId",
                table: "Contacts",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerST_CompanyId",
                table: "CustomerST",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageForUsers_OrderId",
                table: "DamageForUsers",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Damages_PhotoVehicleInspectionId",
                table: "Damages",
                column: "PhotoVehicleInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispatchers_CompanyId",
                table: "Dispatchers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispatchers_DispatcherTypeId",
                table: "Dispatchers",
                column: "DispatcherTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsCompanies_CompanyId",
                table: "DocumentsCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsDrivers_DriverId",
                table: "DocumentsDrivers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsTrailers_TrailerId",
                table: "DocumentsTrailers",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsTrucks_TruckId",
                table: "DocumentsTrucks",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CompanyId",
                table: "Drivers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_DriverControlId",
                table: "Drivers",
                column: "DriverControlId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_DriverReportId",
                table: "Drivers",
                column: "DriverReportId",
                unique: true,
                filter: "[DriverReportId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_GeolocationId",
                table: "Drivers",
                column: "GeolocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_PhoneNumberId",
                table: "Drivers",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_DriversInspections_DriverId",
                table: "DriversInspections",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriversInspections_TrailerId",
                table: "DriversInspections",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_DriversInspections_TruckId",
                table: "DriversInspections",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_DriversQuestionnaires_DriverId",
                table: "DriversQuestionnaires",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriversQuestionnaires_QuestionnaireId",
                table: "DriversQuestionnaires",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriesOrdersActions_OrderId",
                table: "HistoriesOrdersActions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_TransportVehicleId",
                table: "Layouts",
                column: "TransportVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketPosts_UserId",
                table: "MarketPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CompanyId",
                table: "Orders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrentStatusId",
                table: "Orders",
                column: "CurrentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryId",
                table: "Orders",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DriverId",
                table: "Orders",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FaxNumberId",
                table: "Orders",
                column: "FaxNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PhoneNumberId",
                table: "Orders",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PickedUpId",
                table: "Orders",
                column: "PickedUpId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordsRecoveries_RecoveryTypeId",
                table: "PasswordsRecoveries",
                column: "RecoveryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentsMethods_CompanyId",
                table: "PaymentsMethods",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoTypeId",
                table: "Photos",
                column: "PhotoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosAnswers_DriverQuestionnaireId",
                table: "PhotosAnswers",
                column: "DriverQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosAnswers_PhotoId",
                table: "PhotosAnswers",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosAnswers_QuestionId",
                table: "PhotosAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosDriversInspections_DriverInspectionId",
                table: "PhotosDriversInspections",
                column: "DriverInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosDriversInspections_PhotoId",
                table: "PhotosDriversInspections",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosMP_PhotoListMPId",
                table: "PhotosMP",
                column: "PhotoListMPId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosMP_PhotoTypeId",
                table: "PhotosMP",
                column: "PhotoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosVehiclesInspections_PhotoId",
                table: "PhotosVehiclesInspections",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotosVehiclesInspections_VehicleInspectionId",
                table: "PhotosVehiclesInspections",
                column: "VehicleInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSettings_TransportVehicleId",
                table: "ProfileSettings",
                column: "TransportVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionnaireId",
                table: "Questions",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_SellItemsMarketsPosts_MarketPostId",
                table: "SellItemsMarketsPosts",
                column: "MarketPostId");

            migrationBuilder.CreateIndex(
                name: "IX_SellItemsMarketsPosts_PhoneNumberId",
                table: "SellItemsMarketsPosts",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_SellItemsMarketsPosts_PhotoListMPId",
                table: "SellItemsMarketsPosts",
                column: "PhotoListMPId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribeST_CustomerSTId",
                table: "SubscribeST",
                column: "CustomerSTId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_CompanyId",
                table: "Trailers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_VehicleModelId",
                table: "Trailers",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_CompanyId",
                table: "Trucks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TruckTypeId",
                table: "Trucks",
                column: "TruckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_VehicleModelId",
                table: "Trucks",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckTypes_VehicleCategoryId",
                table: "TruckTypes",
                column: "VehicleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersAnswers_DriverQuestionnaireId",
                table: "UsersAnswers",
                column: "DriverQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersAnswers_QuestionId",
                table: "UsersAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersMailsQuestions_PhoneNumberId",
                table: "UsersMailsQuestions",
                column: "PhoneNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesBrands_VehicleTypeId",
                table: "VehiclesBrands",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesDetails_OrderId",
                table: "VehiclesDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesDetails_VehicleModelId",
                table: "VehiclesDetails",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesInspections_VehicleId",
                table: "VehiclesInspections",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesModels_VehicleBodyId",
                table: "VehiclesModels",
                column: "VehicleBodyId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclesModels_VehicleBrandId",
                table: "VehiclesModels",
                column: "VehicleBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoTypeId",
                table: "Videos",
                column: "VideoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosAnswers_DriverQuestionnaireId",
                table: "VideosAnswers",
                column: "DriverQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosAnswers_QuestionId",
                table: "VideosAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosAnswers_VideoId",
                table: "VideosAnswers",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewsMarketsPosts_MarketPostId",
                table: "ViewsMarketsPosts",
                column: "MarketPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewsMarketsPosts_UserId",
                table: "ViewsMarketsPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewsUsersMailsQuestions_UserId",
                table: "ViewsUsersMailsQuestions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewsUsersMailsQuestions_UserMailQuestionId",
                table: "ViewsUsersMailsQuestions",
                column: "UserMailQuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminAnswers");

            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BuyItemsMarketsPosts");

            migrationBuilder.DropTable(
                name: "CheckoutMPs");

            migrationBuilder.DropTable(
                name: "CommentsMarketsPosts");

            migrationBuilder.DropTable(
                name: "CompanyUsers");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "DamageForUsers");

            migrationBuilder.DropTable(
                name: "Damages");

            migrationBuilder.DropTable(
                name: "Dispatchers");

            migrationBuilder.DropTable(
                name: "DocumentsCompanies");

            migrationBuilder.DropTable(
                name: "DocumentsDrivers");

            migrationBuilder.DropTable(
                name: "DocumentsTrailers");

            migrationBuilder.DropTable(
                name: "DocumentsTrucks");

            migrationBuilder.DropTable(
                name: "HistoriesDriversActions");

            migrationBuilder.DropTable(
                name: "HistoriesMarketPostsActions");

            migrationBuilder.DropTable(
                name: "HistoriesOrdersActions");

            migrationBuilder.DropTable(
                name: "HistoriesTrailerActions");

            migrationBuilder.DropTable(
                name: "HistoriesTrucksActions");

            migrationBuilder.DropTable(
                name: "Layouts");

            migrationBuilder.DropTable(
                name: "PasswordsRecoveries");

            migrationBuilder.DropTable(
                name: "PaymentsMethods");

            migrationBuilder.DropTable(
                name: "PhotosAnswers");

            migrationBuilder.DropTable(
                name: "PhotosDriversInspections");

            migrationBuilder.DropTable(
                name: "PhotosMP");

            migrationBuilder.DropTable(
                name: "ProfileSettings");

            migrationBuilder.DropTable(
                name: "SellItemsMarketsPosts");

            migrationBuilder.DropTable(
                name: "SubscribeST");

            migrationBuilder.DropTable(
                name: "UsersAnswers");

            migrationBuilder.DropTable(
                name: "VideosAnswers");

            migrationBuilder.DropTable(
                name: "ViewsMarketsPosts");

            migrationBuilder.DropTable(
                name: "ViewsUsersMailsQuestions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PhotosVehiclesInspections");

            migrationBuilder.DropTable(
                name: "DispatchersTypes");

            migrationBuilder.DropTable(
                name: "RecoveriesTypes");

            migrationBuilder.DropTable(
                name: "DriversInspections");

            migrationBuilder.DropTable(
                name: "TransportVehicles");

            migrationBuilder.DropTable(
                name: "PhotosListMPs");

            migrationBuilder.DropTable(
                name: "CustomerST");

            migrationBuilder.DropTable(
                name: "DriversQuestionnaires");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "MarketPosts");

            migrationBuilder.DropTable(
                name: "UsersMailsQuestions");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "VehiclesInspections");

            migrationBuilder.DropTable(
                name: "Trailers");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "Questionnaires");

            migrationBuilder.DropTable(
                name: "VideoTypes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PhotoTypes");

            migrationBuilder.DropTable(
                name: "VehiclesDetails");

            migrationBuilder.DropTable(
                name: "TruckTypes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "VehiclesModels");

            migrationBuilder.DropTable(
                name: "VehiclesCategories");

            migrationBuilder.DropTable(
                name: "CurrentStatuses");

            migrationBuilder.DropTable(
                name: "AddressInformations");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "VehiclesBodies");

            migrationBuilder.DropTable(
                name: "VehiclesBrands");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "DriversControls");

            migrationBuilder.DropTable(
                name: "DriversReports");

            migrationBuilder.DropTable(
                name: "Geolocations");

            migrationBuilder.DropTable(
                name: "VehiclesTypes");

            migrationBuilder.DropTable(
                name: "PhonesNumbers");
        }
    }
}
