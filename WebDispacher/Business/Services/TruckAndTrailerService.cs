using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using DeviceDetectorNET;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Resources.ViewModels.History;
using WebDispacher.ViewModels.Marketplace;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Trailer.Enum;
using WebDispacher.ViewModels.Truck;
using WebDispacher.ViewModels.Truck.Enum;
using WebDispacher.ViewModels.Widget;
using static System.Net.WebRequestMethods;

namespace WebDispacher.Business.Services
{
    public class TruckAndTrailerService : ITruckAndTrailerService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IHistoryActionService historyActionService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly int maxFileLength = 6 * 1024 * 1024;

        public TruckAndTrailerService(
            IMapper mapper,
            Context db,
            IUserService userService,
            IHistoryActionService historyActionService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.db = db;
            this.historyActionService = historyActionService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddProfile(string idCompany, int idTr, string typeTransport)
        {
            var tr = await GetTr(idTr, typeTransport);
            var transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);

            var profileSetting = new ProfileSettings()
            {
                TransportVehicle = new TransportVehicle()
                {
                    CountPhoto = transportVehicle.CountPhoto,
                    Layouts = userService.GetLayoutsByTransportVehicle(transportVehicle),
                    Type = transportVehicle.Type,
                    TypeTransportVehicle = transportVehicle.TypeTransportVehicle
                },
                IdCompany = Convert.ToInt32(idCompany),
                Name = TruckAndTrailerConstants.Custom,
                TypeTransportVehikle = GetTypeTransport(idTr, typeTransport),
                IdTr = idTr
            };

            return await AddProfileDb(profileSetting);
        }

        private async Task<int> AddProfileDb(ProfileSettings profileSetting)
        {
            await db.ProfileSettings.AddAsync(profileSetting);

            await db.SaveChangesAsync();

            return profileSetting.Id;
        }

        public async Task<Truck> GetTruck(string idDriver)
        {
            return await GetTruckDb(idDriver);
        }

        public async Task<Dictionary<string, string>> GetBaseTruckDoc(string id)
        {
            return await GetBaseTruckDocDb(id);
        }

        public async Task<Truck> GetTruckByPlate(string truckPlate)
        {
            return await GetTruckByPlateDb(truckPlate);
        }

        public async Task RemoveTruck(int id)
        {
            await RemoveTruckDb(id);
        }
        
        public async Task RemoveTruckWidget(int entryId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                await RemoveTruckWidgetInDb(entryId, result);
            }
        }
        
        public async Task RemoveTrailerWidget(int entryId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                await RemoveTrailerWidgetInDb(entryId, result);
            }
        }

        public async Task<bool> RemoveTruckGroupByCompanyId(int id)
        {
            return await RemoveTruckGroupByCompanyIdInDb(id);
        }

        public async Task<bool> RemoveTrailerGroupByCompanyId(int id)
        {
            return await RemoveTrailerGroupByCompanyIdInDb(id);
        }

        public async Task<bool> CheckTruckHaveGroup(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await CheckTruckHaveGroupInDb(result);
            }

            return false;
        }
        
        public async Task<bool> IsVisibleTruckLocation(LocationType selectedLocationType)
        {
            return await IsVisibleTruckLocationInDb(selectedLocationType);
        }
        
        public async Task<bool> IsVisibleTrailerLocation(LocationType selectedLocationType)
        {
            return await IsVisibleTrailerLocationInDb(selectedLocationType);
        }

        public async Task<bool> RenameTruckGroupByCompanyId(int id, string name, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await RenameTruckGroupByCompanyIdInDb(id, name, result);
            }

            return false;
        }

        public async Task<bool> RenameTrailerGroupByCompanyId(int id, string name, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await RenameTrailerGroupByCompanyIdInDb(id, name, result);
            }

            return false;
        }

        public async Task<List<TruckStatusTheme>> GetAvailableTruckStatusThemes()
        {
            return await GetAvailableTruckStatusThemesInDb();
        }

        public async Task<List<TrailerStatusTheme>> GetAvailableTrailerStatusThemes()
        {
            return await GetAvailableTrailerStatusThemesInDb();
        }

        public async Task<bool> CheckTrailerHaveGroup(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await CheckTrailerHaveGroupInDb(result);
            }

            return false;
        }

        public async Task<List<TruckGroupDropdownVm>> GetTruckGroupsDropdownItems(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTruckGroupsDropdownItemsInDb(result);
            }

            return new List<TruckGroupDropdownVm>();
        }

        public async Task<List<TruckStatusDropdownVm>> GetTruckStatusDropdownItems(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTruckStatusDropdownItemsInDb(result);
            }

            return new List<TruckStatusDropdownVm>();
        }

        public async Task<List<TruckStatusDropdownVm>> GetTruckStatusWithoutWidgetsDropdownItems(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTruckStatusWithoutWidgetsDropdownItemsInDb(result);
            }

            return new List<TruckStatusDropdownVm>();
        }
        
        public async Task<List<TrailerStatusDropdownVm>> GetTrailerStatusWithoutWidgetsDropdownItems(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrailerStatusWithoutWidgetsDropdownItemsInDb(result);
            }

            return new List<TrailerStatusDropdownVm>();
        }
        
        
        public async Task<List<TruckStatusDropdownVm>> GetTruckStatusWithoutWidgetsWithCurrentDropdownItems(int currentItemId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTruckStatusWithoutWidgetsWithCurrentDropdownItemsInDb(currentItemId,result);
            }

            return new List<TruckStatusDropdownVm>();
        }
        
        public async Task<List<TrailerStatusDropdownVm>> GetTrailerStatusWithoutWidgetsWithCurrentDropdownItems(int currentItemId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrailerStatusWithoutWidgetsWithCurrentDropdownItemsInDb(currentItemId,result);
            }

            return new List<TrailerStatusDropdownVm>();
        }

        public async Task<List<TrailerStatusDropdownVm>> GetTrailerStatusDropdownItems(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrailerStatusDropdownItemsInDb(result);
            }

            return new List<TrailerStatusDropdownVm>();
        }

        public async Task<List<TrailerGroupDropdownVm>> GetTrailerGroupsDropdownItems(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrailerGroupsDropdownItemsInDb(result);
            }

            return new List<TrailerGroupDropdownVm>();
        }

        public async Task<List<TruckGroup>> GetActualTruckGroupByCompanyId(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetActualTruckGroupByCompanyIdInDb(result);
            }

            return new List<TruckGroup>();
        }

        public async Task<List<TrailerGroup>> GetActualTrailerGroupByCompanyId(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetActualTrailerGroupByCompanyIdInDb(result);
            }

            return new List<TrailerGroup>();
        }

        public async Task<TruckGroup> GetActualTruckGroupById(int id)
        {
            return await GetActualTruckGroupByIdInDb(id);
        }

        public async Task<TrailerGroup> GetActualTrailerGroupById(int id)
        {
            return await GetActualTrailerGroupByIdInDb(id);
        }

        private async Task<List<TruckGroup>> GetActualTruckGroupByCompanyIdInDb(int companyId)
        {
            using (var context = new Context())
            {
                var actualCompanyTrucksGroupsList = await context.TruckGroups
                    .Include(tg => tg.Trucks)
                    .OrderByDescending(tg => tg.Trucks.Count())
                    .Where(tg => tg.CompanyId == companyId)
                    .ToListAsync();

                return actualCompanyTrucksGroupsList;
            }
        }

        private async Task<List<TrailerGroup>> GetActualTrailerGroupByCompanyIdInDb(int companyId)
        {
            using (var context = new Context())
            {
                var actualCompanyTrailerGroupsList = await context.TrailerGroups
                    .Include(tg => tg.Trailers)
                    .OrderByDescending(tg => tg.Trailers.Count())
                    .Where(tg => tg.CompanyId == companyId)
                    .ToListAsync();

                return actualCompanyTrailerGroupsList;
            }
        }

        private async Task<TruckGroup> GetActualTruckGroupByIdInDb(int groupId)
        {
            using (var context = new Context())
            {
                var group = await context.TruckGroups.FirstOrDefaultAsync(tg => tg.Id == groupId);

                return group;
            }
        }

        private async Task<TrailerGroup> GetActualTrailerGroupByIdInDb(int groupId)
        {
            using (var context = new Context())
            {
                var group = await context.TrailerGroups.FirstOrDefaultAsync(tg => tg.Id == groupId);

                return group;
            }
        }

        public async Task<(Dictionary<TruckGroup, List<Truck>>, int)> GetTrucks(TruckFiltersViewModel filters, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrucksDb(filters, result);
            }

            return (new Dictionary<TruckGroup, List<Truck>>(), 0);
        }

        public async Task<(TruckGroup, List<Truck>)> GetTrucksInGroupByFilters(TruckFilterViewModel filters, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrucksInGroupByFiltersDb(filters, result);
            }

            return (new TruckGroup(), new List<Truck>());
        }

        public async Task<bool> SaveTruckStatus(int truckId, int truckStatusId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await SaveTruckStatusInDb(truckId, truckStatusId, result);
            }

            return false;
        }
        
        public async Task<bool> SaveTruckLocation(int truckId, string truckLocation, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await SaveTruckLocationInDb(truckId, truckLocation, result);
            }

            return false;
        }
        
        public async Task<WidgetViewModel> GetTruckWidgetById(int widgetId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTruckWidgetByIdInDb(widgetId, result);
            }

            return null;
        }
        
        public async Task<WidgetViewModel> GetTrailerWidgetById(int widgetId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrailerWidgetByIdInDb(widgetId, result);
            }

            return null;
        }
        
        public async Task<List<TruckStatusWidget>> GetCurrentCompanyTruckWidgets(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetCurrentCompanyTruckWidgetsInDb(result);
            }

            return null;
        }
        
        public async Task<List<TrailerStatusWidget>> GetCurrentCompanyTrailerWidgets(string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetCurrentCompanyTrailerWidgetsInDb(result);
            }

            return null;
        }
        
        public async Task<bool> SaveTrailerLocation(int trailerId, string trailerLocation, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await SaveTrailerLocationInDb(trailerId, trailerLocation, result);
            }

            return false;
        }
        
        public async Task<bool> SaveTrailerStatus(int trailerId, int trailerStatusId, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await SaveTrailerStatusInDb(trailerId, trailerStatusId, result);
            }

            return false;
        }

        public async Task<(TrailerGroup, List<Trailer>)> GetTrailerInGroupByFilters(TrailerFilterViewModel filters, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrailersInGroupByFiltersDb(filters, result);
            }

            return (new TrailerGroup(), new List<Trailer>());
        }

        public int GetCountTrucksPagesByCountEntites(int countEntites)
        {
            return GetCountTrucksPagesInDb(countEntites);
        }

        public async Task<(Dictionary<TrailerGroup, List<Trailer>>, int)> GetTrailers(TrailerFiltersViewModel filters, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetTrailersDb(filters, result);
            }

            return (new Dictionary<TrailerGroup, List<Trailer>>(), 0);
        }

        public int GetCountTrailersPagesByCountEntites(int countEntites)
        {
            return GetCountTrailersPagesInDb(countEntites);
        }

        public async Task<TruckViewModel> GetTruckById(int truckId)
        {
            return await GetTruckByIdDb(truckId);
        }

        public TrailerViewModel GetTrailerById(int idTrailer)
        {
            return GetTrailersByIdDb(idTrailer);
        }

        public async Task<Trailer> GetTrailer(string idDriver)
        {
            return await GetTrailerDb(idDriver);
        }

        public async Task<Trailer> GetTrailerByPlate(string trailerPlate)
        {
            return await GetTrailerByPlateDb(trailerPlate);
        }

        public async Task RemoveTrailer(int id)
        {
            await RemoveTrailerDb(id);
        }

        public async Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, int trailerId, string dateTimeUpload)
        {
            if (uploadedFile.Length > maxFileLength) return;

            var path = $"../Document/Traile/{trailerId}/" + uploadedFile.FileName;

            if (!Directory.Exists("../Document/Traile"))
            {
                Directory.CreateDirectory($"../Document/Traile");
            }

            if (!Directory.Exists($"../Document/Traile/{trailerId}"))
            {
                Directory.CreateDirectory($"../Document/Traile/{trailerId}");
            }

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }

            await SaveDocTrailerDb(path, trailerId, nameDoc, dateTimeUpload);
        }

        public string GetTypeTransport(int idTr, string typeTransport)
        {
            var tr = GetTr(idTr, typeTransport);

            return tr is TruckViewModel ? TypeTransportVehikle.Truck.ToString() : TypeTransportVehikle.Trailer.ToString();
        }

        public async Task<ITr> GetTr(int idTr, string typeTransport)
        {
            ITr tr = null;

            switch (typeTransport)
            {
                case TruckAndTrailerConstants.Truck:
                    tr = await GetTruckById(idTr);
                    break;
                case TruckAndTrailerConstants.Trailer:
                    tr = GetTrailerById(idTr);
                    break;
            }

            return tr;
        }

        public async Task RemoveDocTrailer(int docId)
        {
            var documentTrailer = await db.DocumentsTrailers.FirstOrDefaultAsync(d => d.Id == docId);

            if (documentTrailer == null) return;

            db.DocumentsTrailers.Remove(documentTrailer);

            await db.SaveChangesAsync();
        }

        public async Task RemoveDocTruck(int docId)
        {
            var documentTruck = await db.DocumentsTrucks.FirstOrDefaultAsync(d => d.Id == docId);

            if (documentTruck == null) return;

            db.DocumentsTrucks.Remove(documentTruck);

            await db.SaveChangesAsync();
        }

        public async Task<List<DocumentTruck>> GetTruckDoc(int id)
        {
            return await db.DocumentsTrucks
               .Where(dt => dt.TruckId == id)
               .ToListAsync();
        }

        public async Task<List<DocumentTrailer>> GetTrailerDocsById(int id)
        {
            return await db.DocumentsTrailers
                .Where(dt => dt.TrailerId == id)
                .ToListAsync();
        }

        public List<DriverInspection> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date)
        {
            return GetInspectionTrucksDb(idDriver, idTruck, idTrailer, date);
        }

        public async Task CreateTrailer(TrailerViewModel trailer, string idCompany,
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc, string dateTimeLocal)
        {
            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            trailer.CompanyId = Convert.ToInt32(idCompany);
            trailer.DateTimeLastUpdate = dateTimeCreate;
            trailer.DateTimeRegistration = dateTimeCreate;

            var trailerId = await CreateTrailerDb(trailer);

            if (trailerRegistrationDoc != null)
            {
                await SaveDocTrailer(trailerRegistrationDoc, DocAndFileConstants.TrailerRegistration, trailerId, dateTimeLocal);
            }

            if (trailerAnnualInspectionDoc != null)
            {
                await SaveDocTrailer(trailerAnnualInspectionDoc, DocAndFileConstants.TrailerInspection, trailerId, dateTimeLocal);
            }

            if (leaseAgreementDoc != null)
            {
                await SaveDocTrailer(leaseAgreementDoc, DocAndFileConstants.LeaseAgreement, trailerId, dateTimeLocal);
            }
        }

        public async Task CreateTruck(TruckViewModel truck, string idCompany, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string dateTimeLocal)
        {
            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            truck.CompanyId = Convert.ToInt32(idCompany);
            truck.DateTimeLastUpload = dateTimeCreate;
            truck.DateTimeRegistration = dateTimeCreate;

            var truckId = await CreateTruckDb(truck);

            if (truckRegistrationDoc != null)
            {
                await SaveDocTruck(truckRegistrationDoc, DocAndFileConstants.TruckRegistration, truckId, dateTimeLocal);
            }

            if (truckLeaseAgreementDoc != null)
            {
                await SaveDocTruck(truckLeaseAgreementDoc, DocAndFileConstants.TruckAgreement, truckId, dateTimeLocal);
            }

            if (truckAnnualInspection != null)
            {
                await SaveDocTruck(truckAnnualInspection, DocAndFileConstants.TruckInspection, truckId, dateTimeLocal);

            }

            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, DocAndFileConstants.BobTailPhysicalDamage, truckId, dateTimeLocal);
            }

            if (nYHUTDoc != null)
            {
                await SaveDocTruck(nYHUTDoc, DocAndFileConstants.NyHit, truckId, dateTimeLocal);
            }
        }
        public async Task EditTrailer(TrailerViewModel model, string companyId, string localDate)
        {
            await EditTrailerDb(model, companyId, localDate);
        }

        public async Task<List<TruckType>> GetTruckTypes(string category)
        {
            var categoryId = Int32.Parse(category);

            var truckTypes = await db.TruckTypes.Where(tt => tt.VehicleCategoryId == categoryId).ToListAsync();

            return truckTypes;
        }

        public async Task<List<TrailerType>> GetTrailerTypes(string category)
        {
            var categoryId = Int32.Parse(category);

            var trailerTypes = await db.TrailerTypes.Where(tt => tt.VehicleCategoryId == categoryId).ToListAsync();

            return trailerTypes;
        }

        public async Task<List<VehicleCategory>> GetTruckVehicleCategiries()
        {
            using (var context = new Context())
            {
                var vehicleCategories = await context.VehiclesCategories
                    .Include(vc => vc.TruckTypes)
                    .Where(vc => vc.TruckTypes.Any())
                    .Select(vc => new VehicleCategory
                    {
                        Id = vc.Id,
                        Name = vc.Name,
                    })
                    .ToListAsync();

                return vehicleCategories;
            }
        }

        public async Task<List<VehicleCategory>> GetTrailerVehicleCategiries()
        {
            using (var context = new Context())
            {
                var vehicleCategories = await context.VehiclesCategories
                .Include(vc => vc.TrailerTypes)
                .Where(vc => vc.TrailerTypes.Any())
                .Select(vc => new VehicleCategory
                {
                    Id = vc.Id,
                    Name = vc.Name,
                })
                .ToListAsync();

                return vehicleCategories;
            }
        }

        public async Task<List<Truck>> GetTruckWithApproachingPlateExpiration(string companyId, int countDays)
        {
            if (!int.TryParse(companyId, out var result))
                return new List<Truck>();

            var today = DateTime.Today;
            var thresholdDatePlate = today.AddDays(countDays);

            using (var context = new Context())
            {
                var trucks = await context.Trucks
                    .Include(t => t.TruckGroup)
                    .Where(t => t.PlateExpires <= thresholdDatePlate && t.TruckGroup.CompanyId == result)
                    .ToListAsync();

                return trucks;
            }
        }
        
        public async Task<List<Trailer>> GetTrailerWithApproachingPlateExpiration(string companyId, int countDays)
        {
            if (!int.TryParse(companyId, out var result))
                return new List<Trailer>();

            var today = DateTime.Today;
            var thresholdDatePlate = today.AddDays(countDays);

            using (var context = new Context())
            {
                var trailers = await context.Trailers
                    .Include(t => t.TrailerGroup)
                    .Where(t => t.PlateExpires <= thresholdDatePlate && t.TrailerGroup.CompanyId == result)
                    .ToListAsync();

                return trailers;
            }
        }
        
        public async Task<List<Truck>> GetTruckWithApproachingAnnualInspectionExpiration(string companyId, int countDays)
        {
            if (!int.TryParse(companyId, out var result))
                return new List<Truck>();

            var today = DateTime.Today;
            var thresholdDateLastInspection = today.AddDays(countDays);

            using (var context = new Context())
            {
                var trucks = await context.Trucks
                    .Include(t => t.Inspections)
                    .Where(t =>  t.AnnualIns <= thresholdDateLastInspection && t.TruckGroup.CompanyId == result)
                    .ToListAsync();

                return trucks;
            }
        }

        public async Task<List<Trailer>> GetTrailerWithApproachingAnnualInspectionExpiration(string companyId, int countDays)
        {
            if (!int.TryParse(companyId, out var result))
                return new List<Trailer>();

            var today = DateTime.Today;
            var thresholdDateLastInspection = today.AddDays(countDays);

            using (var context = new Context())
            {
                var trailers = await context.Trailers
                    .Include(t => t.Inspections)
                    .Include(t => t.TrailerGroup)
                    .Where(t => t.AnnualIns <= thresholdDateLastInspection && t.TrailerGroup.CompanyId == result)
                    .ToListAsync();
                return trailers;
            }
        }

        public async Task<string> GetTruckTypeSlugByName(string truckName)
        {
            var slug = await db.TruckTypes.FirstOrDefaultAsync(tt => tt.Slug != null && tt.Name == truckName);

            return slug == null ? null : slug.Slug;
        }

        public async Task<string> GetTrailerTypeSlugByName(string trailerName)
        {
            var slug = await db.TrailerTypes.FirstOrDefaultAsync(tt => tt.Slug != null && tt.Name == trailerName);

            return slug == null ? null : slug.Slug;
        }

        public async Task AddTruckGroupInCompany(string groupName, string dateTimeLocalTruck, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocalTruck) ? DateTime.Now : DateTime.ParseExact(dateTimeLocalTruck, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var isHaveGroup = await context.TruckGroups.AnyAsync(tsw => tsw.Name == groupName);

                if (isHaveGroup) return;

                var newTruckGroup = new TruckGroup { Name = groupName.ToUpper(), CompanyId = result, DateTimeLastUpdate = dateTimeCreate, DateTimeCreate = dateTimeCreate };

                await context.TruckGroups.AddAsync(newTruckGroup);

                await context.SaveChangesAsync();
            }
        }

        public async Task AddTrailerGroupInCompany(string groupName, string dateTimeLocalTruck, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocalTruck) ? DateTime.Now : DateTime.ParseExact(dateTimeLocalTruck, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var isHaveGroup = await context.TrailerGroups.AnyAsync(tsw => tsw.Name == groupName);

                if (isHaveGroup) return;

                var newTrailerGroup = new TrailerGroup { Name = groupName.ToUpper(), CompanyId = result, DateTimeLastUpdate = dateTimeCreate, DateTimeCreate = dateTimeCreate };

                await context.TrailerGroups.AddAsync(newTrailerGroup);

                await context.SaveChangesAsync();
            }
        }

        public async Task AddTruckWidgetInCompany(CreateWidgetVm model, string dateTimeLocal, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var isHaveWidget = await context.TruckStatusWidgets.AnyAsync(tsw => tsw.StatusId == model.StatusId);
                if (isHaveWidget) return;

                var status = await context.TruckStatuses.FirstOrDefaultAsync(ts => ts.Id == model.StatusId);
                if (status == null) return;

                var newStatusWidgetStatus = new TruckStatusWidget
                {
                    StatusId = model.StatusId,
                    StatusThemeId = status.StatusThemeId,
                    CompanyId = result
                };

                await context.TruckStatusWidgets.AddAsync(newStatusWidgetStatus);

                await context.SaveChangesAsync();
            }
        }
        
        public async Task UpdateTruckWidgetInCompany(WidgetViewModel model, string dateTimeLocal, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var actualWidget = await context.TruckStatusWidgets.FirstOrDefaultAsync(tsw => tsw.CompanyId == result && tsw.Id == model.Id);
                if (actualWidget == null) return;

                var isHaveStatus = await context.TruckStatuses.AnyAsync(tsw => (tsw.TruckStatusType == TruckStatusType.System && tsw.Id == model.StatusId) || (tsw.TruckStatusType == TruckStatusType.Custom && tsw.CompanyId == result && tsw.Id == model.StatusId));
                if (!isHaveStatus) return;

                var status = await context.TruckStatuses.FirstOrDefaultAsync(ts => ts.Id == model.StatusId);
                if (status == null) return;

                actualWidget.StatusId = model.StatusId;
                actualWidget.StatusThemeId = status.StatusThemeId;

                await context.SaveChangesAsync();
            }
        }
        
        public async Task UpdateTrailerWidgetInCompany(WidgetViewModel model, string dateTimeLocal, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var actualWidget = await context.TrailerStatusWidgets.FirstOrDefaultAsync(tsw => tsw.CompanyId == result && tsw.Id == model.Id);
                if (actualWidget == null) return;

                var isHaveStatus = await context.TrailerStatuses.AnyAsync(tsw => (tsw.TrailerStatusType == TrailerStatusType.System && tsw.Id == model.StatusId) || (tsw.TrailerStatusType == TrailerStatusType.Custom && tsw.CompanyId == result && tsw.Id == model.StatusId));
                if (!isHaveStatus) return;

                var status = await context.TrailerStatuses.FirstOrDefaultAsync(ts => ts.Id == model.StatusId);
                if (status == null) return;

                actualWidget.StatusId = model.StatusId;
                actualWidget.StatusThemeId = status.StatusThemeId;

                await context.SaveChangesAsync();
            }
        }
        
        public async Task AddTrailerWidgetInCompany(CreateWidgetVm model, string dateTimeLocal, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var isHaveWidget = await context.TrailerStatusWidgets.AnyAsync(tsw => tsw.StatusId == model.StatusId);
                if (isHaveWidget) return;

                var status = await context.TrailerStatuses.FirstOrDefaultAsync(ts => ts.Id == model.StatusId);
                if (status == null) return;

                var newStatusWidgetStatus = new TrailerStatusWidget
                {
                    StatusId = model.StatusId,
                    StatusThemeId = status.StatusThemeId,
                    CompanyId = result
                };

                await context.TrailerStatusWidgets.AddAsync(newStatusWidgetStatus);

                await context.SaveChangesAsync();
            }
        }
        
        public async Task AddCustomTruckStatusInCompany(CreateTruckStatusVm model, string dateTimeLocalTruck, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocalTruck) ? DateTime.Now : DateTime.ParseExact(dateTimeLocalTruck, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var newTruckStatus = new TruckStatus
                {
                    Name = model.Name,
                    StatusThemeId = model.SelectedThemeId.Value,
                    TruckStatusType = TruckStatusType.Custom,
                    CompanyId = result,
                    DateTimeCreate = dateTimeCreate
                };

                await context.TruckStatuses.AddAsync(newTruckStatus);

                await context.SaveChangesAsync();
            }
        }

        public async Task AddCustomTrailerStatusInCompany(CreateTrailerStatusVm model, string dateTimeLocalTrailer, string companyId)
        {
            if (!int.TryParse(companyId, out var result))
                return;

            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocalTrailer) ? DateTime.Now : DateTime.ParseExact(dateTimeLocalTrailer, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            using (var context = new Context())
            {
                var newTrailerStatus = new TrailerStatus
                {
                    Name = model.Name,
                    StatusThemeId = model.SelectedThemeId.Value,
                    TrailerStatusType = TrailerStatusType.Custom,
                    CompanyId = result,
                    DateTimeCreate = dateTimeCreate
                };

                await context.TrailerStatuses.AddAsync(newTrailerStatus);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateTruckAnnualInspectionWithDoc(int entryId, DateTime docExp, IFormFile annualInspection)
        {
            using (var context = new Context()) {
                var currentTruck = await context.Trucks.FirstOrDefaultAsync(t => t.Id == entryId);
                if (currentTruck == null) throw new Exception();

                currentTruck.AnnualIns = docExp;

                await context.SaveChangesAsync();
           }

            if (annualInspection != null)
            {
                await SaveDocTruck(annualInspection, $"{DocAndFileConstants.TruckInspection} - {docExp.ToString(DateTimeFormats.DateOfBirth)}", entryId, null);
            }
        }
        
        public async Task UpdateTrailerAnnualInspectionWithDoc(int entryId, DateTime docExp, IFormFile annualInspection)
        {
            using (var context = new Context()) {
                var currentTrailer = await context.Trailers.FirstOrDefaultAsync(t => t.Id == entryId);
                if (currentTrailer == null) throw new Exception();

                currentTrailer.AnnualIns = docExp;

                await context.SaveChangesAsync();
           }

            if (annualInspection != null)
            {
                await SaveDocTrailer(annualInspection, $"{DocAndFileConstants.TruckInspection} - {docExp.ToString(DateTimeFormats.DateOfBirth)}", entryId, null);
            }
        }
        
        public async Task UpdateTruckPlateWithDoc(int entryId, DateTime docExp, IFormFile plate)
        {
            using (var context = new Context()) {
                var currentTruck = await context.Trucks.FirstOrDefaultAsync(t => t.Id == entryId);
                if (currentTruck == null) throw new Exception();

                currentTruck.PlateExpires = docExp;

                await context.SaveChangesAsync();
           }

            if (plate != null)
            {
                await SaveDocTruck(plate, $"{DocAndFileConstants.TruckRegistration} - {docExp.ToString(DateTimeFormats.DateOfBirth)}", entryId, null);
            }
        }
        
        public async Task UpdateTrailerPlateWithDoc(int entryId, DateTime docExp, IFormFile plate)
        {
            using (var context = new Context()) {
                var currentTrailer = await context.Trailers.FirstOrDefaultAsync(t => t.Id == entryId);
                if (currentTrailer == null) throw new Exception();

                currentTrailer.PlateExpires = docExp;

                await context.SaveChangesAsync();
           }

            if (plate != null)
            {
                await SaveDocTrailer(plate, $"{DocAndFileConstants.TruckRegistration} - {docExp.ToString(DateTimeFormats.DateOfBirth)}", entryId, null);
            }
        }


        public async Task EditTruck(TruckViewModel model, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string companyId, string localDate)
        {
            if (model == null) return;

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var editTruck = await db.Trucks
                .Include(t => t.TruckGroup)
                .FirstOrDefaultAsync(t => t.Id == model.Id);

            if (editTruck == null) return;

            var editTruckViewModel = mapper.Map<TruckViewModel>(editTruck);
            var userId = (await GetUserByCompanyId(companyId)).Id;
            var authorId = (await GetUserByCompanyId(editTruck.TruckGroup.CompanyId.ToString())).Id;

            var actionData = new HistoryActionData
            {
                UserId = userId,
                AuthorId = authorId,
                IPAddress = GetIPAddress(),
                UserAgent = GetUserAgent(),
            };

            await historyActionService.CompareAndSaveUpdatedFields<HistoryTruckAction, TruckViewModel>(editTruckViewModel, model, editTruck.Id, actionData, dateTimeUpdate);
            editTruck.Name = model.NameTruck;
            editTruck.Year = Convert.ToInt32(model.Year);
            editTruck.Brand = model.Make;
            editTruck.Model = model.Model;
            editTruck.TruckTypeId = model.TruckTypeId;
            editTruck.TruckGroupId = model.TruckGroupId.Value;
            editTruck.TruckStatusId = model.TruckStatusId;
            editTruck.State = model.State;
            //editTruck.PlateExpires = model.Exp;
            editTruck.VIN = model.Vin;
            editTruck.Owner = model.Owner;
            editTruck.Plate = model.PlateTruck;
            editTruck.Color = model.ColorTruck;
            editTruck.TruckStatus = model.TruckStatus;
            editTruck.LocationType = model.LocationType;
            //editTruck.AnnualIns = model.AnnualIns;
            editTruck.DateTimeLastUpdate = dateTimeUpdate;

            if (!string.IsNullOrEmpty(model.LocationAddress) && model.LocationAddress != "none")
            {
                editTruck.LocationAddress = model.LocationAddress;
            }

            await db.SaveChangesAsync();

            if (truckRegistrationDoc != null)
            {
                await SaveDocTruck(truckRegistrationDoc, DocAndFileConstants.TruckRegistration, editTruck.Id, localDate);
            }

            if (truckLeaseAgreementDoc != null)
            {
                await SaveDocTruck(truckLeaseAgreementDoc, DocAndFileConstants.TruckAgreement, editTruck.Id, localDate);
            }

            if (truckAnnualInspection != null)
            {
                await SaveDocTruck(truckAnnualInspection, DocAndFileConstants.TruckInspection, editTruck.Id, localDate);

            }

            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, DocAndFileConstants.BobTailPhysicalDamage, editTruck.Id, localDate);
            }

            if (nYHUTDoc != null)
            {
                await SaveDocTruck(nYHUTDoc, DocAndFileConstants.NyHit, editTruck.Id, localDate);
            }
        }

        private async Task EditTrailerDb(TrailerViewModel model, string companyId, string localDate)
        {
            if (model == null) return;

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var editTrailer = await db.Trailers
                .Include(t => t.TrailerGroup)
                .FirstOrDefaultAsync(t => t.Id == model.Id);

            if (editTrailer == null) return;

            var editTrailerViewModel = mapper.Map<TrailerViewModel>(editTrailer);
            var userId = (await GetUserByCompanyId(companyId)).Id;
            var authorId = (await GetUserByCompanyId(editTrailer.TrailerGroup.CompanyId.ToString())).Id;

            var actionData = new HistoryActionData
            {
                UserId = userId,
                AuthorId = authorId,
                IPAddress = GetIPAddress(),
                UserAgent = GetUserAgent(),
            };

            await historyActionService.CompareAndSaveUpdatedFields<HistoryTrailerAction, TrailerViewModel>(editTrailerViewModel, model, editTrailer.Id, actionData, dateTimeUpdate);

            editTrailer.Name = model.Name;
            editTrailer.Type = model.Type;
            editTrailer.Year = Convert.ToInt32(model.Year);
            editTrailer.Brand = model.Make;
            editTrailer.Model = model.Model;
            editTrailer.HowLong = model.HowLong;
            editTrailer.TrailerGroupId = model.TrailerGroupId.Value;
            editTrailer.TrailerStatusId = model.TrailerStatusId;
            editTrailer.TrailerTypeId = model.TrailerTypeId;
            editTrailer.Vin = model.Vin;
            editTrailer.Owner = model.Owner;
            editTrailer.Color = model.Color;
            editTrailer.Plate = model.Plate;
            //editTrailer.PlateExpires = model.Exp;
            editTrailer.LocationType = model.LocationType;
            //editTrailer.AnnualIns = model.AnnualIns;
            editTrailer.DateTimeLastUpdate = dateTimeUpdate;

            if (!string.IsNullOrEmpty(model.LocationAddress) && model.LocationAddress != "none")
            {
                editTrailer.LocationAddress = model.LocationAddress;
            }

            await db.SaveChangesAsync();
        }

        private async Task SaveDocTruckDb(string path, int truckId, string nameDoc, string localDate)
        {
            var dateTimeUpload = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var pref = path.Remove(0, path.LastIndexOf(".") + 1);

            var documentTruck = new DocumentTruck()
            {
                DocPath = path,
                Name = nameDoc,
                TruckId = truckId,
                DateTimeUpload = dateTimeUpload,
            };

            await db.DocumentsTrucks.AddAsync(documentTruck);

            await db.SaveChangesAsync();
        }

        public async Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, int truckId, string dateTimeUpload)
        {
            if (uploadedFile.Length > maxFileLength) return;

            var path = $"../Document/Truck/{truckId}/" + uploadedFile.FileName;

            if (!Directory.Exists("../Document/Truck"))
            {
                Directory.CreateDirectory($"../Document/Truck");
            }

            if (!Directory.Exists($"../Document/Truck/{truckId}"))
            {
                Directory.CreateDirectory($"../Document/Truck/{truckId}");
            }

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }

            await SaveDocTruckDb(path, truckId, nameDoc, dateTimeUpload);
        }

        private List<DriverInspection> GetInspectionTrucksDb(string driverId, string idTruck, string idTrailer, string date)
        {
            var inspectionDrivers = new List<DriverInspection>();

            db.Drivers
                .Include(d => d.Inspections)
                .ToList()
                .Where(d => driverId == DocAndFileConstants.ZeroLevel || d.Id.ToString() == driverId)
                .ToList()
                .ForEach((item) =>
                {
                    inspectionDrivers.AddRange(item.Inspections
                        .Where(iD => (date == DocAndFileConstants.ZeroLevel ||
                                      (Convert.ToDateTime(iD.DateTimeInspection).Month == Convert.ToDateTime(date).Month
                                       && Convert.ToDateTime(iD.DateTimeInspection).Year == Convert.ToDateTime(date).Year))
                                     && (idTruck == DocAndFileConstants.ZeroLevel || iD.TruckId.ToString() == idTruck)
                                     && (idTrailer == DocAndFileConstants.ZeroLevel || iD.TrailerId.ToString() == idTrailer)));
                });

            return inspectionDrivers;
        }

        private async Task<int> CreateTrailerDb(TrailerViewModel trailerViewModel)
        {
            var trailer = new Trailer
            {
                Name = trailerViewModel.Name,
                Year = Convert.ToInt32(trailerViewModel.Year),
                Brand = trailerViewModel.Make,
                Model = trailerViewModel.Model,
                Type = trailerViewModel.Type,
                HowLong = trailerViewModel.HowLong,
                Vin = trailerViewModel.Vin,
                Plate = trailerViewModel.Plate,
                PlateExpires = trailerViewModel.Exp,
                Color = trailerViewModel.Color,
                TrailerGroupId = trailerViewModel.TrailerGroupId.Value,
                TrailerStatusId = trailerViewModel.TrailerStatusId,
                TrailerTypeId = trailerViewModel.TrailerTypeId,
                AnnualIns = trailerViewModel.AnnualIns,
                LocationType = trailerViewModel.LocationType,
                Owner = trailerViewModel.Owner,
            };

            if (!string.IsNullOrEmpty(trailerViewModel.LocationAddress) && trailerViewModel.LocationAddress != "none")
            {
                trailer.LocationAddress = trailerViewModel.LocationAddress;
            }

            await db.Trailers.AddAsync(trailer);

            await db.SaveChangesAsync();

            return trailer.Id;
        }

        private async Task<int> CreateTruckDb(TruckViewModel truckModel)
        {
            try
            {
                var truck = new Truck
                {
                    Name = truckModel.NameTruck,
                    Year = Convert.ToInt32(truckModel.Year),
                    Brand = truckModel.Make,
                    Model = truckModel.Model,
                    State = truckModel.State,
                    PlateExpires = truckModel.Exp,
                    Plate = truckModel.PlateTruck,
                    VIN = truckModel.Vin,
                    Owner = truckModel.Owner,
                    AnnualIns = truckModel.AnnualIns,
                    Color = truckModel.ColorTruck,
                    TruckStatus = truckModel.TruckStatus,
                    TruckTypeId = truckModel.TruckTypeId,
                    TruckGroupId = truckModel.TruckGroupId.Value,
                    TruckStatusId = truckModel.TruckStatusId,
                    LocationType = truckModel.LocationType,
                    DateTimeCreate = truckModel.DateTimeRegistration,
                    DateTimeLastUpdate = truckModel.DateTimeLastUpload,
                };

                if(!string.IsNullOrEmpty(truckModel.LocationAddress) && truckModel.LocationAddress != "none" )
                {
                    truck.LocationAddress = truckModel.LocationAddress;
                }

                await db.Trucks.AddAsync(truck);

                await db.SaveChangesAsync();

                return truck.Id;
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        private async Task SaveDocTrailerDb(string path, int trailerId, string nameDoc, string localDate)
        {
            var dateTimeUpload = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var pref = path.Remove(0, path.LastIndexOf(".") + 1);

            var documentTruck = new DocumentTrailer()
            {
                DocPath = path,
                Name = nameDoc,
                TrailerId = trailerId,
                DateTimeUpload = dateTimeUpload,
            };

            await db.DocumentsTrailers.AddAsync(documentTruck);

            await db.SaveChangesAsync();
        }

        private async Task RemoveTrailerDb(int id)
        {
            var trailer = await db.Trailers.FirstOrDefaultAsync(t => t.Id == id);

            if (trailer == null) return;

            db.Trailers.Remove(trailer);

            await db.SaveChangesAsync();
        }

        private async Task RemoveTruckDb(int id)
        {
            var truck = await db.Trucks.FirstOrDefaultAsync(t => t.Id == id);
            if (truck == null) return;

            db.Trucks.Remove(truck);

            await db.SaveChangesAsync();
        }

        private async Task RemoveTruckWidgetInDb(int entryId, int companyId)
        {
            using (var context = new Context())
            {
                var truckWidget = await db.TruckStatusWidgets.FirstOrDefaultAsync(tsw => tsw.Id == entryId && tsw.CompanyId == companyId);
                if (truckWidget == null) return;

                db.TruckStatusWidgets.Remove(truckWidget);

                await db.SaveChangesAsync();
            }
        }
        
        private async Task RemoveTrailerWidgetInDb(int entryId, int companyId)
        {
            using (var context = new Context())
            {
                var trailerWidget = await db.TrailerStatusWidgets.FirstOrDefaultAsync(tsw => tsw.Id == entryId && tsw.CompanyId == companyId);
                if (trailerWidget == null) return;

                db.TrailerStatusWidgets.Remove(trailerWidget);

                await db.SaveChangesAsync();
            }
        }

        private async Task<bool> RemoveTruckGroupByCompanyIdInDb(int id)
        {
            var truckGroup = await db.TruckGroups.Include(tg => tg.Trucks).FirstOrDefaultAsync(t => t.Id == id);
            if (truckGroup == null || truckGroup.Trucks.Count > 0) return false;

            db.TruckGroups.Remove(truckGroup);

            await db.SaveChangesAsync();
            return true;
        }

        private async Task<bool> RemoveTrailerGroupByCompanyIdInDb(int id)
        {
            var trailerGroup = await db.TrailerGroups.Include(tg => tg.Trailers).FirstOrDefaultAsync(t => t.Id == id);
            if (trailerGroup == null || trailerGroup.Trailers.Count > 0) return false;

            db.TrailerGroups.Remove(trailerGroup);

            await db.SaveChangesAsync();
            return true;
        }

        private async Task<bool> RenameTruckGroupByCompanyIdInDb(int id, string name, int companyId)
        {
            var currentTruckGroup = await db.TruckGroups.FirstOrDefaultAsync(tg => tg.Id == id && tg.CompanyId == companyId);

            if (currentTruckGroup == null) return false;

            currentTruckGroup.Name = name.ToUpper();

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<bool> RenameTrailerGroupByCompanyIdInDb(int id, string name, int companyId)
        {
            var currentTrailerGroup = await db.TrailerGroups.FirstOrDefaultAsync(tg => tg.Id == id && tg.CompanyId == companyId);
            if (currentTrailerGroup == null) return false;

            currentTrailerGroup.Name = name.ToUpper();

            await db.SaveChangesAsync();

            return true;
        }

        private async Task<(TrailerGroup, List<Trailer>)> GetTrailersInGroupByFiltersDb(TrailerFilterViewModel filters, int companyId)
        {
            var trailerQuery = db.Trailers
                    .Include(t => t.TrailerType)
                    .Include(t => t.TrailerGroup)
                    .Include(t => t.TrailerStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.BackgroundColor)
                    .Include(t => t.TrailerStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.TextColor)
                    .Where(t => t.TrailerGroupId == filters.GroupId && t.TrailerGroup.CompanyId == companyId)
                    .AsQueryable();

            var trailerList = await trailerQuery.ToListAsync();

            var groupedTrailerByGroupNameDictionary = trailerList
                .GroupBy(tq => tq.TrailerGroup)
                .ToDictionary(g => g.Key, g =>
                    ApplySortingTrailer(g, filters).ToList());

            var groupedTrailerByGroupName = groupedTrailerByGroupNameDictionary.Count() == 0
                   ? new KeyValuePair<TrailerGroup, List<Trailer>>(await GetActualTrailerGroupByIdInDb(filters.GroupId), new List<Trailer>())
                    : groupedTrailerByGroupNameDictionary.First();

            return (groupedTrailerByGroupName.Key, groupedTrailerByGroupName.Value);
        }

        private async Task<(Dictionary<TrailerGroup, List<Trailer>>, int)> GetTrailersDb(TrailerFiltersViewModel filters, int companyId)
        {
            var groupedTrailer = new Dictionary<TrailerGroup, List<Trailer>>();
            var countEntities = 0;

            if (filters.GroupId != 0)
            {
                var trailerQuery = db.Trailers
                    .Include(t => t.TrailerType)
                    .Include(t => t.TrailerGroup)
                    .Include(t => t.TrailerStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.BackgroundColor)
                    .Include(t => t.TrailerStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.TextColor)
                    .Where(t => t.TrailerGroupId == filters.GroupId && t.TrailerGroup.CompanyId == companyId)
                    .AsQueryable();

                countEntities = await trailerQuery.CountAsync();

                var filter = filters.TrailerFiltersViewModels.FirstOrDefault(tf => tf.GroupId == filters.GroupId);

                if (filter == null)
                {
                    filters.TrailerFiltersViewModels.Add(new TrailerFilterViewModel() { GroupId = filters.GroupId });
                    filter = filters.TrailerFiltersViewModels.FirstOrDefault(tf => tf.GroupId == filters.GroupId);
                }

                var trailerList = await trailerQuery.ToListAsync();

                var groupedTrailerByGroupNameDictionary = trailerList
                    .GroupBy(tq => tq.TrailerGroup)
                    .ToDictionary(g => g.Key, g =>
                        ApplySortingTrailer(g, filter).ToList());

                var groupedTrailerByGroupName = groupedTrailerByGroupNameDictionary.Count() == 0
                   ? new KeyValuePair<TrailerGroup, List<Trailer>>(await GetActualTrailerGroupByIdInDb(filter.GroupId), new List<Trailer>())
                    : groupedTrailerByGroupNameDictionary.First();

                groupedTrailer.Add(groupedTrailerByGroupName.Key, groupedTrailerByGroupName.Value);
            }
            else
            {
                filters.GroupId = 0;

                var allGroups = await db.TrailerGroups
                    .Include(tg => tg.Trailers)
                    .Where(tg => tg.CompanyId == companyId)
                    .OrderByDescending(tg => tg.Trailers.Count())
                    .ToListAsync();

                foreach (var group in allGroups)
                {
                    var trailerQuery = db.Trailers
                        .Include(t => t.TrailerType)
                        .Include(t => t.TrailerGroup)
                        .Include(t => t.TrailerStatus)
                        .ThenInclude(t => t.StatusTheme)
                        .ThenInclude(t => t.BackgroundColor)
                        .Include(t => t.TrailerStatus)
                        .ThenInclude(t => t.StatusTheme)
                        .ThenInclude(t => t.TextColor)
                        .Where(t => t.TrailerGroupId == group.Id && t.TrailerGroup.CompanyId == companyId)
                        .AsQueryable();

                    countEntities += await trailerQuery.CountAsync();

                    var filter = filters.TrailerFiltersViewModels.FirstOrDefault(tf => tf.GroupId == group.Id);

                    if (filter == null)
                    {
                        filters.TrailerFiltersViewModels.Add(new TrailerFilterViewModel() { GroupId = group.Id });
                        filter = filters.TrailerFiltersViewModels.FirstOrDefault(tf => tf.GroupId == group.Id);
                    }

                    var sortedTrailer = ApplySortingTrailer(trailerQuery, filter).ToList();
                    groupedTrailer.Add(group, sortedTrailer);
                }
            }

            return (groupedTrailer, countEntities);
        }

        private async Task<Trailer> GetTrailerByPlateDb(string trailerPlate)
        {
            return await db.Trailers.FirstOrDefaultAsync(t => t.Plate == trailerPlate);
        }

        private async Task<Trailer> GetTrailerDb(string idDriver)
        {
            var driver = await db.Drivers
                .Include(d => d.Inspections)
                .FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);

            if (driver == null && driver.Inspections == null && driver.Inspections.Count == 0)
            {
                return null;
            }

            var inspectionDriver = driver.Inspections.Last();
            var trailer = await db.Trailers.FirstOrDefaultAsync(t => t.Id == inspectionDriver.TrailerId);

            return trailer;
        }

        private async Task<User> GetUserByCompanyId(string companyId)
        {
            int companyIndex = Convert.ToInt32(companyId);

            var userInfo = await db.Users
                .Include(u => u.CompanyUsers)
                .FirstOrDefaultAsync(x => x.CompanyUsers.First(cu => cu.CompanyId == companyIndex).UserId == x.Id);

            if (userInfo == null) return null;

            return userInfo;
        }

        private async Task<bool> CheckTruckHaveGroupInDb(int companyId)
        {
            var companyGroup = await GetCountTruckGroupsByCompanyId(companyId);

            return IsHaveRequiredTruckGroupsInCompany(companyGroup);
        }

        private async Task<bool> CheckTrailerHaveGroupInDb(int companyId)
        {
            var companyGroup = await GetCountTrailerGroupsByCompanyId(companyId);

            return IsHaveRequiredTrailerGroupsInCompany(companyGroup);
        }
        
        private async Task<bool> IsVisibleTruckLocationInDb(LocationType selectedLocationType)
        {
            return LocationType.Later != selectedLocationType;
        }
        
        private async Task<bool> IsVisibleTrailerLocationInDb(LocationType selectedLocationType)
        {
            return LocationType.Later != selectedLocationType;
        }

        private async Task<List<TruckGroupDropdownVm>> GetTruckGroupsDropdownItemsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var truckGroups = await context.TruckGroups
                    .Where(tg => tg.CompanyId == companyId)
                    .OrderByDescending(tg => tg.DateTimeLastUpdate)
                    .Select(tg => new TruckGroupDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                return truckGroups;
            }
        }

        private async Task<bool> SaveTruckStatusInDb(int truckId, int truckStatusId, int companyId)
        {
            using (var context = new Context())
            {
                var currentStatus = context.TruckStatuses
                    .Where(ts => ts.Id == truckStatusId)
                    .FirstOrDefault();

                if (currentStatus != null)
                {
                    if (currentStatus.TruckStatusType != TruckStatusType.System && currentStatus.CompanyId != companyId)
                    {
                        currentStatus = null;
                    }
                }

                if (currentStatus == null) return false;

                var currentTruck = await context.Trucks.FirstOrDefaultAsync(t => t.Id == truckId);
                if (currentTruck == null) return false;

                currentTruck.TruckStatusId = truckStatusId;

                await context.SaveChangesAsync();

                return true;
            }
        }
        
        private async Task<bool> SaveTruckLocationInDb(int truckId, string truckLocation, int companyId)
        {
            using (var context = new Context())
            {
                var currentTruck = context.Trucks
                    .Include(ts => ts.TruckGroup)
                    .Where(ts => ts.Id == truckId && ts.TruckGroup.CompanyId == companyId)
                    .FirstOrDefault();

                if (currentTruck == null) return false;

                currentTruck.LocationAddress = truckLocation;

                await context.SaveChangesAsync();

                return true;
            }
        }
        
        private async Task<bool> SaveTrailerLocationInDb(int trailerId, string trailerLocation, int companyId)
        {
            using (var context = new Context())
            {
                var currentTrailer = context.Trailers
                    .Include(ts => ts.TrailerGroup)
                    .Where(ts => ts.Id == trailerId && ts.TrailerGroup.CompanyId == companyId)
                    .FirstOrDefault();

                if (currentTrailer == null) return false;

                currentTrailer.LocationAddress = trailerLocation;

                await context.SaveChangesAsync();

                return true;
            }
        }

        private async Task<bool> SaveTrailerStatusInDb(int trailerId, int trailerStatusId, int companyId)
        {
            using (var context = new Context())
            {
                var currentStatus = context.TrailerStatuses
                    .Where(ts => ts.Id == trailerStatusId)
                    .FirstOrDefault();

                if (currentStatus != null)
                {
                    if (currentStatus.TrailerStatusType != TrailerStatusType.System && currentStatus.CompanyId != companyId)
                    {
                        currentStatus = null;
                    }
                }

                if (currentStatus == null) return false;

                var currentTrailer = await context.Trailers.FirstOrDefaultAsync(t => t.Id == trailerId);
                if (currentTrailer == null) return false;

                currentTrailer.TrailerStatusId = trailerStatusId;

                await context.SaveChangesAsync();

                return true;
            }
        }
        
        private async Task<List<TruckStatusDropdownVm>> GetTruckStatusDropdownItemsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var truckStatuses = await context.TruckStatuses
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.TextColor)
                    .Where(tg => tg.CompanyId == companyId || tg.CompanyId == null)
                    .Select(tg => new TruckStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                return truckStatuses;
            }
        }
        
        private async Task<List<TruckStatusDropdownVm>> GetTruckStatusWithoutWidgetsDropdownItemsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var truckStatuses = await context.TruckStatuses
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.TextColor)
                    .Where(tg => (tg.CompanyId == companyId || tg.CompanyId == null) &&
                            !context.TruckStatusWidgets.Any(tsw => tsw.StatusId == tg.Id))
                    .Select(tg => new TruckStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                return truckStatuses;
            }
        }
        
        private async Task<List<TrailerStatusDropdownVm>> GetTrailerStatusWithoutWidgetsDropdownItemsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var trailerStatuses = await context.TrailerStatuses
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.TextColor)
                    .Where(tg => (tg.CompanyId == companyId || tg.CompanyId == null) &&
                            !context.TrailerStatusWidgets.Any(tsw => tsw.StatusId == tg.Id))
                    .Select(tg => new TrailerStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                return trailerStatuses;
            }
        }
        
        private async Task<List<TruckStatusDropdownVm>> GetTruckStatusWithoutWidgetsWithCurrentDropdownItemsInDb(int currentStatusId,int companyId)
        {
            using (var context = new Context())
            {
                var currentItem = await context.TruckStatuses
                    .Include(ts => ts.StatusTheme)
                        .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                        .ThenInclude(tst => tst.TextColor)
                    .Where(tg => tg.Id == currentStatusId)
                    .Select(tg => new TruckStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .FirstOrDefaultAsync();

                var truckStatuses = await context.TruckStatuses
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.TextColor)
                    .Where(tg => (tg.CompanyId == companyId || tg.CompanyId == null) &&
                            !context.TruckStatusWidgets.Any(tsw => tsw.StatusId == tg.Id))
                    .Select(tg => new TruckStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                if (currentItem != null)
                {
                    truckStatuses.Add(currentItem);
                }

                return truckStatuses.OrderBy(x => x.Id).ToList();
            }
        }
        
        private async Task<List<TrailerStatusDropdownVm>> GetTrailerStatusWithoutWidgetsWithCurrentDropdownItemsInDb(int currentStatusId,int companyId)
        {
            using (var context = new Context())
            {
                var currentItem = await context.TrailerStatuses
                    .Include(ts => ts.StatusTheme)
                        .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                        .ThenInclude(tst => tst.TextColor)
                    .Where(tg => tg.Id == currentStatusId)
                    .Select(tg => new TrailerStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .FirstOrDefaultAsync();

                var trailerStatuses = await context.TrailerStatuses
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.TextColor)
                    .Where(tg => (tg.CompanyId == companyId || tg.CompanyId == null) &&
                            !context.TrailerStatusWidgets.Any(tsw => tsw.StatusId == tg.Id))
                    .Select(tg => new TrailerStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                if (currentItem != null)
                {
                    trailerStatuses.Add(currentItem);
                }

                return trailerStatuses.OrderBy(x => x.Id).ToList();
            }
        }

        private async Task<WidgetViewModel> GetTruckWidgetByIdInDb(int widgetId, int companyId)
        {
            using (var context = new Context())
            {
                var widget = await context.TruckStatusWidgets
                    .FirstOrDefaultAsync(tsw => tsw.Id == widgetId && tsw.CompanyId == companyId);

                var mapperWidget = mapper.Map<WidgetViewModel>(widget);

                return mapperWidget;
            }
        }
        
        private async Task<WidgetViewModel> GetTrailerWidgetByIdInDb(int widgetId, int companyId)
        {
            using (var context = new Context())
            {
                var widget = await context.TrailerStatusWidgets
                    .FirstOrDefaultAsync(tsw => tsw.Id == widgetId && tsw.CompanyId == companyId);

                var mapperWidget = mapper.Map<WidgetViewModel>(widget);

                return mapperWidget;
            }
        }
        
        private async Task<List<TruckStatusWidget>> GetCurrentCompanyTruckWidgetsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var currentTruckWidgets = await context.TruckStatusWidgets
                    .Include(tsw => tsw.Status)
                    .Include(tsw => tsw.StatusTheme)
                    .ThenInclude(wst => wst.BackgroundColor)
                    .Include(tsw => tsw.StatusTheme)
                    .ThenInclude(wst => wst.TextColor)
                    .Where(tsw => tsw.CompanyId == companyId)
                    .ToListAsync();

                return currentTruckWidgets;
            }
        }
        
        private async Task<List<TrailerStatusWidget>> GetCurrentCompanyTrailerWidgetsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var currentTrailerWidgets = await context.TrailerStatusWidgets
                    .Include(tsw => tsw.Status)
                    .Include(tsw => tsw.StatusTheme)
                    .ThenInclude(wst => wst.BackgroundColor)
                    .Include(tsw => tsw.StatusTheme)
                    .ThenInclude(wst => wst.TextColor)
                    .Where(tsw => tsw.CompanyId == companyId)
                    .ToListAsync();

                return currentTrailerWidgets;
            }
        }

        private async Task<List<TrailerStatusDropdownVm>> GetTrailerStatusDropdownItemsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var trailerStatuses = await context.TrailerStatuses
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.BackgroundColor)
                    .Include(ts => ts.StatusTheme)
                    .ThenInclude(tst => tst.TextColor)
                    .Where(tg => tg.CompanyId == companyId || tg.CompanyId == null)
                    .Select(tg => new TrailerStatusDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                return trailerStatuses;
            }
        }

        private async Task<List<TrailerGroupDropdownVm>> GetTrailerGroupsDropdownItemsInDb(int companyId)
        {
            using (var context = new Context())
            {
                var trailerGroups = await context.TrailerGroups
                    .Where(tg => tg.CompanyId == companyId)
                    .OrderByDescending(tg => tg.DateTimeLastUpdate)
                    .Select(tg => new TrailerGroupDropdownVm { Name = tg.Name, Id = tg.Id })
                    .ToListAsync();

                return trailerGroups;
            }
        }

        private async Task<int> GetCountTruckGroupsByCompanyId(int companyId)
        {
            using (var context = new Context())
            {
                var countCompanyTruckGroups = await context.TruckGroups.CountAsync(tg => tg.CompanyId == companyId);

                return countCompanyTruckGroups;
            }
        }

        private async Task<int> GetCountTrailerGroupsByCompanyId(int companyId)
        {
            using (var context = new Context())
            {
                var countCompanyTrailerGroups = await context.TrailerGroups.CountAsync(tg => tg.CompanyId == companyId);

                return countCompanyTrailerGroups;
            }
        }

        private bool IsHaveRequiredTruckGroupsInCompany(int countGroups)
        {
            return countGroups > 0;
        }

        private bool IsHaveRequiredTrailerGroupsInCompany(int countGroups)
        {
            return countGroups > 0;
        }

        private async Task<Truck> GetTruckDb(string idDriver)
        {
            var driver = await db.Drivers
                .Include(d => d.Inspections)
                .FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);

            if (driver == null && driver.Inspections == null && driver.Inspections.Count == 0)
            {
                return null;
            }

            var inspectionDriver = driver.Inspections.Last();
            var truck = await db.Trucks.FirstOrDefaultAsync(t => t.Id == inspectionDriver.TruckId);

            return truck;
        }

        private TrailerViewModel GetTrailersByIdDb(int idTr)
        {
            var trailer = db.Trailers.Include(t => t.TrailerType).FirstOrDefault(t => t.Id == idTr);

            return mapper.Map<TrailerViewModel>(trailer);
        }

        private async Task<TruckViewModel> GetTruckByIdDb(int idTr)
        {
            var truck = await db.Trucks.Include(t => t.TruckType).FirstOrDefaultAsync(t => t.Id == idTr);

            return mapper.Map<TruckViewModel>(truck);
        }

        private async Task<(TruckGroup, List<Truck>)> GetTrucksInGroupByFiltersDb(TruckFilterViewModel filters, int companyId)
        {
            var trucksQuery = db.Trucks
                    .Include(t => t.TruckType)
                    .Include(t => t.TruckGroup)
                    .Include(t => t.TruckStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.BackgroundColor)
                    .Include(t => t.TruckStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.TextColor)
                    .Where(t => t.TruckGroupId == filters.GroupId && t.TruckGroup.CompanyId == companyId)
                    .AsQueryable();

            var trucksList = await trucksQuery.ToListAsync();

            var groupedTruckByGroupNameDictionary = trucksList
                .GroupBy(tq => tq.TruckGroup)
                .ToDictionary(g => g.Key, g =>
                    ApplySortingTruck(g, filters).ToList());

            var groupedTruckByGroupName = groupedTruckByGroupNameDictionary.Count() == 0
                   ? new KeyValuePair<TruckGroup, List<Truck>>(await GetActualTruckGroupByIdInDb(filters.GroupId), new List<Truck>())
                    : groupedTruckByGroupNameDictionary.First();

            return (groupedTruckByGroupName.Key, groupedTruckByGroupName.Value);
        }

        private async Task<(Dictionary<TruckGroup, List<Truck>>, int)> GetTrucksDb(TruckFiltersViewModel filters, int companyId)
        {
            var groupedTrucks = new Dictionary<TruckGroup, List<Truck>>();
            var countEntities = 0;

            if (filters.GroupId != 0)
            {
                var trucksQuery = db.Trucks
                    .Include(t => t.TruckType)
                    .Include(t => t.TruckGroup)
                    .Include(t => t.TruckStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.BackgroundColor)
                    .Include(t => t.TruckStatus)
                    .ThenInclude(t => t.StatusTheme)
                    .ThenInclude(t => t.TextColor)
                    .Where(t => t.TruckGroupId == filters.GroupId && t.TruckGroup.CompanyId == companyId)
                    .AsQueryable();

                countEntities = await trucksQuery.CountAsync();

                var filter = filters.TrucksFiltersViewModels.FirstOrDefault(tf => tf.GroupId == filters.GroupId);

                if (filter == null)
                {
                    filters.TrucksFiltersViewModels.Add(new TruckFilterViewModel() { GroupId = filters.GroupId });
                    filter = filters.TrucksFiltersViewModels.FirstOrDefault(tf => tf.GroupId == filters.GroupId);
                }

                var trucksList = await trucksQuery.ToListAsync();

                var groupedTruckByGroupNameDictionary = trucksList
                    .GroupBy(tq => tq.TruckGroup)
                    .ToDictionary(g => g.Key, g =>
                        ApplySortingTruck(g, filter).ToList());

                var groupedTruckByGroupName = groupedTruckByGroupNameDictionary.Count() == 0
                   ? new KeyValuePair<TruckGroup, List<Truck>>(await GetActualTruckGroupByIdInDb(filter.GroupId), new List<Truck>())
                    : groupedTruckByGroupNameDictionary.First();

                groupedTrucks.Add(groupedTruckByGroupName.Key, groupedTruckByGroupName.Value);
            }
            else
            {
                filters.GroupId = 0;

                var allGroups = await db.TruckGroups
                    .Include(tg => tg.Trucks)
                    .Where(tg => tg.CompanyId == companyId)
                    .OrderByDescending(tg => tg.Trucks.Count())
                    .ToListAsync();

                foreach (var group in allGroups)
                {
                    var trucksQuery = db.Trucks
                        .Include(t => t.TruckType)
                        .Include(t => t.TruckGroup)
                        .Include(t => t.TruckStatus)
                        .ThenInclude(t => t.StatusTheme)
                        .ThenInclude(t => t.BackgroundColor)
                        .Include(t => t.TruckStatus)
                        .ThenInclude(t => t.StatusTheme)
                        .ThenInclude(t => t.TextColor)
                        .Where(t => t.TruckGroupId == group.Id && t.TruckGroup.CompanyId == companyId)
                        .AsQueryable();

                    countEntities += await trucksQuery.CountAsync();

                    var filter = filters.TrucksFiltersViewModels.FirstOrDefault(tf => tf.GroupId == group.Id);

                    if (filter == null)
                    {
                        filters.TrucksFiltersViewModels.Add(new TruckFilterViewModel() { GroupId = group.Id });
                        filter = filters.TrucksFiltersViewModels.FirstOrDefault(tf => tf.GroupId == group.Id);
                    }

                    var sortedTrucks = ApplySortingTruck(trucksQuery, filter).ToList();
                    groupedTrucks.Add(group, sortedTrucks);
                }
            }

            return (groupedTrucks, countEntities);
        }

        /*private async Task<(Dictionary<string, List<Truck>>, int)> GetTrucksDb(TruckFiltersViewModel filters, int companyId)
        {
            var trucksQuery = db.Trucks
                .Include(t => t.TruckType)
                .Include(t => t.TruckGroup)
                .Include(t => t.TruckStatus)
                .ThenInclude(t => t.TruckStatusTheme)
                .ThenInclude(t => t.BackgroundColor)
                .Include(t => t.TruckStatus)
                .ThenInclude(t => t.TruckStatusTheme)
                .ThenInclude(t => t.TextColor)
                .Where(t => t.TruckGroup.CompanyId == companyId)
                .AsQueryable();

            if (filters.GroupId != 0)
            {
                if(filters.GroupId != -1)
                    trucksQuery = trucksQuery.Where(t => t.TruckGroupId == filters.GroupId);
            }
            else
            {
                var companyGroups = await db.TruckGroups
                    .Include(tg => tg.Trucks)
                    .Where(tg => tg.CompanyId == companyId)
                    .OrderByDescending(tg => tg.Trucks.Count())
                    .ToListAsync();

                if (companyGroups.Count > 0)
                {
                    filters.GroupId = -1;
                }
            }

            //var sortedTrucksQuery = ApplySortingTruck(trucksQuery, filters);

            var countEntities = await trucksQuery.CountAsync();

            if (filters.Page == UserConstants.AllPagesNumber)
            {
                var groupedTrucks = await trucksQuery
                    .GroupBy(t => t.TruckGroup.Name)
                    .ToDictionaryAsync(g => g.Key, g => ApplySortingTruck(g, filters).ToList());

                return (groupedTrucks, countEntities);
            }

            try
            {
                *//*var sortedTruckList = await sortedTrucksQuery
                    .Skip(UserConstants.LongPageCount * filters.Page - UserConstants.LongPageCount)
                    .Take(UserConstants.LongPageCount)
                    .ToListAsync();*//*

                var pagedTrucks = await trucksQuery
                    .OrderBy(t => t.TruckGroup.Name)
                    .ThenBy(t => t.Name)
                    .Skip(UserConstants.LongPageCount * filters.Page - UserConstants.LongPageCount)
                    .Take(UserConstants.LongPageCount)
                    .ToListAsync();

                var pagedGroupedTrucks = pagedTrucks
                    .GroupBy(t => t.TruckGroup.Name)
                    .ToDictionary(g => g.Key, g => ApplySortingTruck(g, filters).ToList());

                return (pagedGroupedTrucks, countEntities);
            }
            catch (Exception)
            {
                return (new Dictionary<string, List<Truck>>(), countEntities);
            }
        }*/

        private IOrderedEnumerable<Truck> ApplySortingTruck(IEnumerable<Truck> trucks, TruckFilterViewModel filters)
        {
            switch (filters.SortField)
            {
                case TruckSortField.Name:
                    return filters.SortType == TruckSortType.Ascending ? trucks.OrderBy(x => x.Name) : trucks.OrderByDescending(t => t.Name);
                case TruckSortField.Year:
                    return filters.SortType == TruckSortType.Ascending ? trucks.OrderBy(x => x.Year) : trucks.OrderByDescending(t => t.Year);
                case TruckSortField.Brand:
                    return filters.SortType == TruckSortType.Ascending ? trucks.OrderBy(x => x.Brand) : trucks.OrderByDescending(t => t.Brand);
                case TruckSortField.Model:
                    return filters.SortType == TruckSortType.Ascending ? trucks.OrderBy(x => x.Model) : trucks.OrderByDescending(t => t.Model);
                case TruckSortField.PlateExpires:
                    return filters.SortType == TruckSortType.Ascending ? trucks.OrderBy(x => x.PlateExpires) : trucks.OrderByDescending(t => t.PlateExpires);
                case TruckSortField.State:
                    return filters.SortType == TruckSortType.Ascending ? trucks.OrderBy(x => x.State) : trucks.OrderByDescending(t => t.State);
                case TruckSortField.Owner:
                    return filters.SortType == TruckSortType.Ascending ? trucks.OrderBy(x => x.Owner) : trucks.OrderByDescending(t => t.Owner);
                default:
                    return trucks.OrderByDescending(x => x.Id);
            }
        }

        private IOrderedEnumerable<Trailer> ApplySortingTrailer(IEnumerable<Trailer> trailers, TrailerFilterViewModel filters)
        {
            switch (filters.SortField)
            {
                case TrailerSortField.Name:
                    return filters.SortType == TrailerSortType.Ascending ? trailers.OrderBy(x => x.Name) : trailers.OrderByDescending(t => t.Name);
                case TrailerSortField.Year:
                    return filters.SortType == TrailerSortType.Ascending ? trailers.OrderBy(x => x.Year) : trailers.OrderByDescending(t => t.Year);
                case TrailerSortField.Brand:
                    return filters.SortType == TrailerSortType.Ascending ? trailers.OrderBy(x => x.Brand) : trailers.OrderByDescending(t => t.Brand);
                case TrailerSortField.Model:
                    return filters.SortType == TrailerSortType.Ascending ? trailers.OrderBy(x => x.Model) : trailers.OrderByDescending(t => t.Model);
                case TrailerSortField.PlateExpires:
                    return filters.SortType == TrailerSortType.Ascending ? trailers.OrderBy(x => x.PlateExpires) : trailers.OrderByDescending(t => t.PlateExpires);
                case TrailerSortField.State:
                    return filters.SortType == TrailerSortType.Ascending ? trailers.OrderBy(x => x.State) : trailers.OrderByDescending(t => t.State);
                case TrailerSortField.Owner:
                    return filters.SortType == TrailerSortType.Ascending ? trailers.OrderBy(x => x.Owner) : trailers.OrderByDescending(t => t.Owner);
                default:
                    return trailers.OrderByDescending(x => x.Id);
            }
        }

        private string GetUserAgent()
        {
            return httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }

        private string GetIPAddress()
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        private int GetCountTrucksPagesInDb(int countEntites)
        {
            var countPages = GetCountPage(countEntites, UserConstants.LongPageCount);

            return countPages;
        }

        private int GetCountPage(int countElements, int countElementsInOnePage)
        {
            var countPages = (countElements / countElementsInOnePage) + ((countElements % countElementsInOnePage) > 0 ? 1 : 0);

            return countPages;
        }

        private int GetCountTrailersPagesInDb(int countEntites)
        {
            var countPages = GetCountPage(countEntites, UserConstants.LongPageCount);

            return countPages;

        }

        private async Task<List<TruckStatusTheme>> GetAvailableTruckStatusThemesInDb()
        {
            using (var context = new Context())
            {
                var truckStatusThemes = await context.TruckStatusThemes
                    .Include(tst => tst.BackgroundColor)
                    .Include(tst => tst.TextColor)
                    .ToListAsync();

                return truckStatusThemes;
            }
        }

        private async Task<List<TrailerStatusTheme>> GetAvailableTrailerStatusThemesInDb()
        {
            using (var context = new Context())
            {
                var trailerStatusThemes = await context.TrailerStatusThemes
                    .Include(tst => tst.BackgroundColor)
                    .Include(tst => tst.TextColor)
                    .ToListAsync();

                return trailerStatusThemes;
            }
        }

        private async Task<Truck> GetTruckByPlateDb(string truckPlate)
        {
            return await db.Trucks.FirstOrDefaultAsync(t => t.Plate == truckPlate);
        }

        private async Task<Dictionary<string, string>> GetBaseTruckDocDb(string id)
        {
            var docs = await db.DocumentsTrucks
                .Where(d => d.Id.ToString() == id)
                .ToListAsync();

            var keyValues = new Dictionary<string, string>();

            foreach (var item in docs)
            {
                keyValues.Add(item.Name, GetFileNameWithPath(item.DocPath));
            }


            return keyValues;
        }

        private string GetFileNameWithPath(string path)
        {
            var fileName = path.Remove(0, path.LastIndexOf("/") + 1);

            return fileName;
        }
    }
}