using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.ViewModels.Trailer; 
using WebDispacher.ViewModels.Truck;

namespace WebDispacher.Business.Services
{
    public class TruckAndTrailerService : ITruckAndTrailerService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly int maxFileLength = 6 * 1024 * 1024;

        public TruckAndTrailerService(
            IMapper mapper,
            Context db,
            IUserService userService)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.db = db;
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
        
        public async Task<List<Truck>> GetTrucks(int page, string companyId)
        {
            if(int.TryParse(companyId, out var result))
            {
                return await GetTrucksDb(page, result);
            }

            return new List<Truck>();
        }

        public async Task<int> GetCountTrucksPages(string idCompany)
        {
            return await GetCountTrucksPagesInDb(idCompany);
        }

        public async Task<List<Trailer>> GetTrailers(int page, string idCompany)
        {
            return await GetTrailersDb(page, idCompany);
        }

        public async Task<int> GetCountTrailersPages(string idCompany)
        {
            return await GetCountTrailersPagesInDb(idCompany);
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
        
        public async Task CreateTruck(TruckViewModel truck ,string idCompany, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string dateTimeLocal)
        {
            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            truck.CompanyId = Convert.ToInt32(idCompany);
            truck.DateTimeLastUpload = dateTimeCreate;
            truck.DateTimeRegistration = dateTimeCreate;

            var truckId = await CreateTruckDb(truck);
            
            if(truckRegistrationDoc != null)
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
        public async Task EditTrailer(TrailerViewModel model, string localDate)
        {
            await EditTrailerDb(model, localDate);
        }

        public async Task EditTruck(TruckViewModel model, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string localDate)
        {
            if (model == null) return;

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var editTruck = await db.Trucks
                .FirstOrDefaultAsync(t => t.Id == model.Id);

            if (editTruck == null) return;

            editTruck.Name = model.NameTruck;
            editTruck.Year = Convert.ToInt32(model.Year);
            editTruck.Brand = model.Make;
            editTruck.Model = model.Model;
            editTruck.Type = model.Type;
            editTruck.State = model.State;
            editTruck.PlateExpires = model.Exp;
            editTruck.VIN = model.Vin;
            editTruck.Owner = model.Owner;
            editTruck.Plate = model.PlateTruck;
            editTruck.Color = model.ColorTruck;
            editTruck.DateTimeLastUpdate = dateTimeUpdate;

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
        
        private async Task EditTrailerDb(TrailerViewModel model, string localDate)
        {
            if (model == null) return;

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var editTrailer = await db.Trailers.FirstOrDefaultAsync(t => t.Id == model.Id);
            
            if (editTrailer == null) return;
            
            editTrailer.Name = model.Name;
            editTrailer.Type = model.Type;
            editTrailer.Year = Convert.ToInt32(model.Year);
            editTrailer.Brand = model.Make;
            editTrailer.Model = model.Model;
            editTrailer.HowLong = model.HowLong;
            editTrailer.Vin = model.Vin;
            editTrailer.Owner = model.Owner;
            editTrailer.Color = model.Color;
            editTrailer.Plate = model.Plate;
            editTrailer.PlateExpires = model.Exp;
            editTrailer.AnnualIns = model.AnnualIns;
            editTrailer.DateTimeLastUpdate = dateTimeUpdate;
                
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
            
            if(!Directory.Exists("../Document/Truck"))
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
                CompanyId = trailerViewModel.CompanyId,
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
                AnnualIns = trailerViewModel.AnnualIns,
                Owner = trailerViewModel.Owner,
            };

            await db.Trailers.AddAsync(trailer);
            
            await db.SaveChangesAsync();
            
            return trailer.Id;
        }
        
        private async Task<int> CreateTruckDb(TruckViewModel truckModel)
        {
            var truck = new Truck
            {
                CompanyId = truckModel.CompanyId,
                Name = truckModel.NameTruck,
                Year = Convert.ToInt32(truckModel.Year),
                Brand = truckModel.Make,
                Model = truckModel.Model,
                State = truckModel.State,
                PlateExpires = truckModel.Exp,
                Plate = truckModel.PlateTruck,
                VIN = truckModel.Vin,
                Owner = truckModel.Owner,
                Color = truckModel.ColorTruck,
                Type = truckModel.Type,
                DateTimeCreate = truckModel.DateTimeRegistration,
                DateTimeLastUpdate = truckModel.DateTimeLastUpload,
            };

            await db.Trucks.AddAsync(truck);
            
            await db.SaveChangesAsync();
            
            return truck.Id;
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
        
        private async Task<List<Trailer>> GetTrailersDb(int page, string idCompany)
        {
            var trailers = db.Trailers.Where(t => t.CompanyId.ToString() == idCompany).OrderByDescending(x => x.Id).AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await trailers.ToListAsync();

            try
            {
                trailers = trailers.Skip(UserConstants.LongPageCount * page - UserConstants.LongPageCount);

                trailers = trailers.Take(UserConstants.LongPageCount);
            }
            catch (Exception)
            {
                trailers = trailers.Skip((UserConstants.LongPageCount * page) - UserConstants.LongPageCount);
            }

            var listTrailers = await trailers.ToListAsync();

            return listTrailers;
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
            var trailer = db.Trailers.FirstOrDefault(t => t.Id == idTr);
            
            return mapper.Map<TrailerViewModel>(trailer);
        }
        
        private async Task<TruckViewModel> GetTruckByIdDb(int idTr)
        {
            var truck = await db.Trucks.FirstOrDefaultAsync(t => t.Id == idTr);

            return mapper.Map<TruckViewModel>(truck);
        }
        
        private async Task<List<Truck>> GetTrucksDb(int page, int companyId)
        {
            var trucks = db.Trucks
                .Where(t => t.CompanyId == companyId)
                .OrderByDescending(x => x.Id)
                .AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await trucks.ToListAsync();

            try
            {
                trucks = trucks.Skip(UserConstants.LongPageCount * page - UserConstants.LongPageCount);

                trucks = trucks.Take(UserConstants.LongPageCount);
            }
            catch (Exception)
            {
                trucks = trucks.Skip((UserConstants.LongPageCount * page) - UserConstants.LongPageCount);
            }

            var listTrucks = await trucks.ToListAsync();

            return listTrucks;
        }

        private async Task<int> GetCountTrucksPagesInDb(string idCompany)
        {
            var countTrucks = await db.Trucks.Where(t => t.CompanyId.ToString() == idCompany).CountAsync();

            var countPages = GetCountPage(countTrucks, UserConstants.LongPageCount);

            return countPages;

        }

        private int GetCountPage(int countElements, int countElementsInOnePage)
        {
            var countPages = (countElements / countElementsInOnePage) % countElementsInOnePage;

            return countPages > 0 ? countPages + 1 : countPages;
        }

        private async Task<int> GetCountTrailersPagesInDb(string idCompany)
        {
            var countTrailers = await db.Trailers.Where(t => t.CompanyId.ToString() == idCompany).CountAsync();

            var countPages = GetCountPage(countTrailers, UserConstants.LongPageCount);

            return countPages;

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

            foreach(var item in docs)
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