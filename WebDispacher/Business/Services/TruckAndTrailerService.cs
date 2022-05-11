using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Models;
using WebDispacher.Service;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Truck;

namespace WebDispacher.Business.Services
{
    public class TruckAndTrailerService : ITruckAndTrailerService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public TruckAndTrailerService(
            IMapper mapper,
            Context db,
            IUserService userService)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.db = db;
        }
        
        public int AddProfile(string idCompany, int idTr, string typeTransport)
        {
            var tr = GetTr(idTr, typeTransport);
            var transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);
            
            var profileSetting = new ProfileSetting()
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
            
            return AddProfileDb(profileSetting);
        }
        
        private int AddProfileDb(ProfileSetting profileSetting)
        {
            db.ProfileSettings.Add(profileSetting);
            
            db.SaveChanges();
            
            return profileSetting.Id;
        }

        public async Task<Truck> GetTruck(string idDriver)
        {
            return await GetTruckDb(idDriver);
        }
        
        public async Task<List<DocumentTruckAndTrailers>> GetTruckDoc(string id)
        {
            return await GetTruckDocDb(id);
        }
        
        public async Task<Truck> GetTruckByPlate(string truckPlate)
        {
            return await GetTruckByPlateDb(truckPlate);
        }
        
        public void RemoveTruck(string id)
        {
            RemoveTruckDb(id);
        }
        
        public List<Truck> GetTrucks(string idCompany)
        {
            return GetTrucksDb(idCompany);
        }
        
        public TruckViewModel GetTruckById(int idTruck)
        {
            return GetTruckByIdDb(idTruck);
        }

        public TrailerViewModel GetTrailerById(int idTrailer)
        {
            return GetTrailersByIdDb(idTrailer);
        }
        
        public async Task<Trailer> GetTrailer(string idDriver)
        {
            return await GetTrailerDb(idDriver);
        }
        
        public void EditTrailer(TrailerViewModel model)
        {
           EditTrailerDb(model);
        }
        
        public async Task<Trailer> GetTrailerByPlate(string trailerPlate)
        {
            return await GetTrailerByPlateDb(trailerPlate);
        }
        
        public List<Trailer> GetTrailers(string idCompany)
        {
            return GetTrailersDb(idCompany);
        }
        
        public void RemoveTrailer(string id)
        {
            RemoveTrailerDb(id);
        }
        
        public async Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, string id)
        {
            var path = $"../Document/Traile/{id}/" + uploadedFile.FileName;
            
            if (!Directory.Exists("../Document/Traile"))
            {
                Directory.CreateDirectory($"../Document/Traile");
            }
            
            if (!Directory.Exists($"../Document/Traile/{id}"))
            {
                Directory.CreateDirectory($"../Document/Traile/{id}");
            }
            
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            
            SaveDocTrailerDb(path, id, nameDoc);
        }

        public string GetTypeTransport(int idTr, string typeTransport)
        {
            var tr = GetTr(idTr, typeTransport);
            
            return tr is Truck ? TypeTransportVehikle.Truck.ToString() : TypeTransportVehikle.Trailer.ToString();
        }
        
        public ITr GetTr(int idTr, string typeTransport)
        {
            ITr tr = null;
            
            switch (typeTransport)
            {
                case TruckAndTrailerConstants.Truck:
                    tr = GetTruckById(idTr);
                    break;
                case TruckAndTrailerConstants.Trailer:
                    tr = GetTrailerById(idTr);
                    break;
            }
            
            return tr;
        }
        
        public async Task<List<DocumentTruckAndTrailers>> GetTrailerDoc(string id)
        {
            return await db.DocumentTruckAndTrailers
                .Where(d => d.TypeTr == TruckAndTrailerConstants.Trailer && d.IdTr.ToString() == id)
                .ToListAsync();
        }
        
        public List<InspectionDriver> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date)
        {
            return GetInspectionTrucksDb(idDriver, idTruck, idTrailer, date);
        }
        
        public async Task CreateTrailer(TrailerViewModel trailer, string idCompany, 
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc)
        {
            trailer.CompanyId = Convert.ToInt32(idCompany);
            var id = CreateTrailerDb(trailer);
            
            await SaveDocTrailer(trailerRegistrationDoc, DocAndFileConstants.TrailerRegistration, id.ToString());
            await SaveDocTrailer(trailerAnnualInspectionDoc, DocAndFileConstants.TrailerInspection, id.ToString());
            
            if (leaseAgreementDoc != null)
            {
                await SaveDocTrailer(leaseAgreementDoc, DocAndFileConstants.LeaseAgreement, id.ToString());
            }
        }
        
        public async Task CreateTruck(TruckViewModel truck ,string idCompany, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc)
        {
            truck.CompanyId = Convert.ToInt32(idCompany);
            var id = await CreateTruckDb(truck);
            
            await SaveDocTruck(truckRegistrationDoc, DocAndFileConstants.TruckRegistration, id.ToString());
            await SaveDocTruck(truckLeaseAgreementDoc, DocAndFileConstants.TruckAgreement, id.ToString());
            await SaveDocTruck(truckAnnualInspection, DocAndFileConstants.TruckInspection, id.ToString());
            
            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, DocAndFileConstants.BobTailPhysicalDamage, id.ToString());
            }
            
            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, DocAndFileConstants.NyHit, id.ToString());
            }
        }
        
        public void EditTruck(TruckViewModel model)
        {
            var editTruck = db.Trucks.FirstOrDefault(t => t.Id == model.Id);
            
            if (editTruck == null) return;

            editTruck.NameTruk = model.NameTruck;
            editTruck.Yera = model.Year;
            editTruck.Make = model.Make;
            editTruck.Model = model.Model;
            editTruck.Type = model.Type;
            editTruck.Satet = model.State;
            editTruck.Exp = model.Exp;
            editTruck.Vin = model.Vin;
            editTruck.Owner = model.Owner;
            editTruck.PlateTruk = model.PlateTruck;
            editTruck.ColorTruk = model.ColorTruck;
            
            db.SaveChanges();
        }
        
        private void EditTrailerDb(TrailerViewModel model)
        {
            var editTrailer = db.Trailers.FirstOrDefault(t => t.Id == model.Id);
            
            if (editTrailer == null) return;
            
            editTrailer.Name = model.Name;
            editTrailer.Type = model.Type;
            editTrailer.Year = model.Year;
            editTrailer.Make = model.Make;
            editTrailer.HowLong = model.HowLong;
            editTrailer.Vin = model.Vin;
            editTrailer.Owner = model.Owner;
            editTrailer.Color = model.Color;
            editTrailer.Plate = model.Plate;
            editTrailer.Exp = model.Exp;
            editTrailer.AnnualIns = model.AnnualIns;
                
            db.SaveChanges();

        }

        private void SaveDocTruckDb(string path, string id, string nameDoc)
        {
            var pref = path.Remove(0, path.LastIndexOf(".") + 1);
            
            var documentTruckAndTrailers = new DocumentTruckAndTrailers()
            {
                DocPath = path,
                IdTr = Convert.ToInt32(id),
                NameDoc = nameDoc,
                TypeTr = TruckAndTrailerConstants.Truck,
                TypeDoc = pref
            };
            
            db.DocumentTruckAndTrailers.Add(documentTruckAndTrailers);
            
            db.SaveChanges();
        }
        
        public async Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, string id)
        {
            var path = $"../Document/Truck/{id}/" + uploadedFile.FileName;
            
            if(!Directory.Exists("../Document/Truck"))
            {
                Directory.CreateDirectory($"../Document/Truck");
            }
            
            if (!Directory.Exists($"../Document/Truck/{id}"))
            {
                Directory.CreateDirectory($"../Document/Truck/{id}");
            }
            
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            
            SaveDocTruckDb(path, id, nameDoc);
        }
        
        private List<InspectionDriver> GetInspectionTrucksDb(string idDriver, string idTruck, string idTrailer, string date)
        {
            var inspectionDrivers = new List<InspectionDriver>();
            
            db.Drivers
                .Include(d => d.InspectionDrivers).ToList()
                .Where(d => idDriver == DocAndFileConstants.ZeroLevel || d.Id.ToString() == idDriver)
                .ToList()
                .ForEach((item) =>
                {
                    inspectionDrivers.AddRange(item.InspectionDrivers
                        .Where(iD => (date == DocAndFileConstants.ZeroLevel || 
                                      (Convert.ToDateTime(iD.Date).Month == Convert.ToDateTime(date).Month 
                                       && Convert.ToDateTime(iD.Date).Year == Convert.ToDateTime(date).Year))
                                     && (idTruck == DocAndFileConstants.ZeroLevel || iD.IdITruck.ToString() == idTruck)
                                     && (idTrailer == DocAndFileConstants.ZeroLevel || iD.IdITrailer.ToString() == idTrailer)));
                });

            return inspectionDrivers;
        }
        
        private async Task<int> CreateTrailerDb(TrailerViewModel trailer)
        {
            await db.Trailers.AddAsync(mapper.Map<Trailer>(trailer));
            
            await db.SaveChangesAsync();
            
            return trailer.Id;
        }
        
        private async Task<int> CreateTruckDb(TruckViewModel truck)
        {
            await db.Trucks.AddAsync(mapper.Map<Truck>(truck));
            
            await db.SaveChangesAsync();
            
            return truck.Id;
        }
        
        private void SaveDocTrailerDb(string path, string id, string nameDoc)
        {
            var pref = path.Remove(0, path.LastIndexOf(".") + 1);
            
            var documentTruckAndTrailers = new DocumentTruckAndTrailers()
            {
                DocPath = path,
                IdTr = Convert.ToInt32(id),
                NameDoc = nameDoc,
                TypeTr = TruckAndTrailerConstants.Trailer,
                TypeDoc = pref
            };
            
            db.DocumentTruckAndTrailers.Add(documentTruckAndTrailers);
            
            db.SaveChanges();
        }
        
        private void RemoveTrailerDb(string id)
        {
            var trailer = db.Trailers.FirstOrDefault(t => t.Id.ToString() == id);
            if (trailer == null) return;
            
            db.Trailers.Remove(trailer);
            
            db.SaveChanges();
        }
        
        private void RemoveTruckDb(string id)
        {
            var truck = db.Trucks.FirstOrDefault(t => t.Id.ToString() == id);
            if (truck == null) return;
            
            db.Trucks.Remove(truck);
            
            db.SaveChanges();
        }
        
        private List<Trailer> GetTrailersDb(string idCompany)
        {
            return db.Trailers.Where(t => t.CompanyId.ToString() == idCompany).ToList();
        }
        
        private async Task<Trailer> GetTrailerByPlateDb(string trailerPlate)
        {
            return await db.Trailers.FirstOrDefaultAsync(t => t.Plate == trailerPlate);
        }

        private async Task<Trailer> GetTrailerDb(string idDriver)
        {
            var driver = await db.Drivers
                .Include(d => d.InspectionDrivers)
                .FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);
            
            if (driver == null && driver.InspectionDrivers == null && driver.InspectionDrivers.Count == 0)
            {
                return null;
            }
            
            var inspectionDriver = driver.InspectionDrivers.Last();
            var trailer = await db.Trailers.FirstOrDefaultAsync(t => t.Id == inspectionDriver.IdITrailer);
            
            return trailer;
        }
        
        private async Task<Truck> GetTruckDb(string idDriver)
        {
            var driver = await db.Drivers
                .Include(d => d.InspectionDrivers)
                .FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);
            
            if (driver == null && driver.InspectionDrivers == null && driver.InspectionDrivers.Count == 0)
            {
                return null;
            }
            
            var inspectionDriver = driver.InspectionDrivers.Last();
            var truck = await db.Trucks.FirstOrDefaultAsync(t => t.Id == inspectionDriver.IdITruck);
            
            return truck; 
        }

        private TrailerViewModel GetTrailersByIdDb(int idTr)
        {
            var trailer = db.Trailers.FirstOrDefault(t => t.Id == idTr);
            
            return mapper.Map<TrailerViewModel>(trailer);
        }
        
        private TruckViewModel GetTruckByIdDb(int idTr)
        {
            var truck = db.Trucks.FirstOrDefault(t => t.Id == idTr);

            return mapper.Map<TruckViewModel>(truck);
        }
        
        private List<Truck> GetTrucksDb(string idCompany)
        {
            return db.Trucks.Where(t => t.CompanyId.ToString() == idCompany).ToList();
        }

        private async Task<Truck> GetTruckByPlateDb(string truckPlate)
        {
            return await db.Trucks.FirstOrDefaultAsync(t => t.PlateTruk == truckPlate);
        }
        
        private async Task<List<DocumentTruckAndTrailers>> GetTruckDocDb(string id)
        {
            return await db.DocumentTruckAndTrailers
                .Where(d => d.TypeTr == TruckAndTrailerConstants.Truck && d.IdTr.ToString() == id)
                .ToListAsync();
        }
    }
}