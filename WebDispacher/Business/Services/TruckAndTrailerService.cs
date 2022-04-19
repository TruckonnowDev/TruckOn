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
using WebDispacher.Models;
using WebDispacher.Service;

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
            ITr tr = GetTr(idTr, typeTransport);
            ITransportVehicle transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);
            ProfileSetting profileSetting = new ProfileSetting()
            {
                TransportVehicle = new TransportVehicle()
                {
                    CountPhoto = transportVehicle.CountPhoto,
                    Layouts = userService.GetLayoutsByTransportVehicle(transportVehicle),
                    Type = transportVehicle.Type,
                    TypeTransportVehicle = transportVehicle.TypeTransportVehicle
                },
                IdCompany = Convert.ToInt32(idCompany),
                Name = "Custom",
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
            return await GetTruckDocDB(id);
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
        
        public Truck GetTruckById(int idTruck)
        {
            return (Truck)GetTruckByIdDb(idTruck);
        }
        
        public Trailer GetTrailerById(int idTrailer)
        {
            return (Trailer)GetTrailersByIdDb(idTrailer);
        }
        
        public async Task<Trailer> GetTrailer(string idDriver)
        {
            return await GetTrailerDb(idDriver);
        }
        
        public void EditTrailer(int idTrailer, string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns)
        {
            EditTrailerDb(idTrailer, name, typeTrailer, year, make, howLong, vin, owner, color, plate, exp, annualIns);
        }
        
        public async Task<Trailer> GetTrailerkByPlate(string trailerPlate)
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
            string path = $"../Document/Traile/{id}/" + uploadedFile.FileName;
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
            
            SaveDocTrailekDb(path, id, nameDoc);
        }
        
        public async void CreateTrailer(string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns,
            string idCompany, IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc)
        {
            int id = CreateTrailerDb(name, typeTrailer, year, make, howLong, vin, owner, color, plate, exp, annualIns, idCompany);
            await SaveDocTrailer(trailerRegistrationDoc, "Trailer registration", id.ToString());
            await SaveDocTrailer(trailerAnnualInspectionDoc, "Trailer annual inspection", id.ToString());
            
            if (leaseAgreementDoc != null)
            {
                await SaveDocTrailer(leaseAgreementDoc, "Lease agreement", id.ToString());
            }
        }
        
        public string GetTypeTransport(int idTr, string typeTransport)
        {
            ITr tr = GetTr(idTr, typeTransport);
            return tr is Truck ? TypeTransportVehikle.Truck.ToString() : TypeTransportVehikle.Trailer.ToString();
        }
        
        public ITr GetTr(int idTr, string typeTransport)
        {
            ITr tr = null;
            if (typeTransport == "Truck")
            {
                tr = GetTruckById(idTr);
            }
            else if (typeTransport == "Trailer")
            {
                tr = GetTrailerById(idTr);
            }
            return tr;
        }
        
        public async Task<List<DocumentTruckAndTrailers>> GetTraileDoc(string id)
        {
            return await db.DocumentTruckAndTrailers.Where(d => d.TypeTr == "Trailer" && d.IdTr.ToString() == id).ToListAsync();
        }
        
        public List<InspectionDriver> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date)
        {
            return GetInspectionTrucksDb(idDriver, idTruck, idTrailer, date);
        }
        
        public async void CreateTruk(string nameTruk, string yera, string make, string model, string typeTruk, string state, string exp, string vin, string owner, string plateTruk,
            string color, string idCompany, IFormFile truckRegistrationDoc, IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, IFormFile nYHUTDoc)
        {
            int id = CreateTrukDb(nameTruk, yera, make, model, typeTruk, state, exp, vin, owner, plateTruk, color, idCompany);
            await SaveDocTruck(truckRegistrationDoc, "Truck registration", id.ToString());
            await SaveDocTruck(truckLeaseAgreementDoc, "Truck lease agreement", id.ToString());
            await SaveDocTruck(truckAnnualInspection, "Truck annual inspection", id.ToString());
            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, "Bob tail physical damage", id.ToString());
            }
            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, "NY HUT", id.ToString());
            }
        }
        
        public void EditTruck(int idTruck, string nameTruck, string year, string make,
            string model, string typeTruck, string state, string exp, string vin, string owner, string plateTruck, string color)
        {
            Truck truck = db.Trucks.FirstOrDefault(t => t.Id == idTruck);
            
            if(truck != null)
            {
                truck.NameTruk = nameTruck;
                truck.Yera = year;
                truck.Make = make;
                truck.Model = model;
                truck.Type = typeTruck;
                truck.Satet = state;
                truck.Exp = exp;
                truck.Vin = vin;
                truck.Owner = owner;
                truck.PlateTruk = plateTruck;
                truck.ColorTruk = color;
                db.SaveChanges();
            }
        }

        private int CreateTrukDb(string nameTruk, string yera, string make, string model, string typeTruk, string state,
            string exp, string vin, string owner, string plateTruk, string color, string idCompany)
        {
            Truck truck = new Truck()
            {
                ColorTruk = color,
                Exp = exp,
                Make = make,
                Model = model,
                NameTruk = nameTruk,
                Owner = owner,
                PlateTruk = plateTruk,
                Satet = state,
                Vin = vin,
                Yera = yera,
                Type = typeTruk,
                CompanyId = Convert.ToInt32(idCompany)
            };
            db.Trucks.Add(truck);
            db.SaveChanges();
            return truck.Id;
        }

        private void SaveDocTruckDb(string path, string id, string nameDoc)
        {
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            DocumentTruckAndTrailers documentTruckAndTrailers = new DocumentTruckAndTrailers()
            {
                DocPath = path,
                IdTr = Convert.ToInt32(id),
                NameDoc = nameDoc,
                TypeTr = "Truck",
                TypeDoc = pref
            };
            db.DocumentTruckAndTrailers.Add(documentTruckAndTrailers);
            db.SaveChanges();
        }
        
        public async Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, string id)
        {
            string path = $"../Document/Truck/{id}/" + uploadedFile.FileName;
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

            List<InspectionDriver> inspectionDrivers = new List<InspectionDriver>();
            db.Drivers
                .Include(d => d.InspectionDrivers).ToList()
                .Where(d => idDriver == "0" || d.Id.ToString() == idDriver)
                .ToList()
                .ForEach((item) =>
                {
                    inspectionDrivers.AddRange(item.InspectionDrivers
                        .Where(iD => (date == "0" || (Convert.ToDateTime(iD.Date).Month == Convert.ToDateTime(date).Month && Convert.ToDateTime(iD.Date).Year == Convert.ToDateTime(date).Year))
                                     && (idTruck == "0" || iD.IdITruck.ToString() == idTruck)
                                     && (idTrailer == "0" || iD.IdITrailer.ToString() == idTrailer)));
                });

            return inspectionDrivers;
        }
        
        private int CreateTrailerDb(string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns, string idCompany)
        {
            Trailer trailer = new Trailer()
            {
                AnnualIns = annualIns,
                Color = color,
                Exp = exp,
                HowLong = howLong,
                Make = make,
                Name = name,
                Owner = owner,
                Plate = plate,
                Vin = vin,
                Year = year,
                Type = typeTrailer,
                CompanyId = Convert.ToInt32(idCompany)
            };
            db.Trailers.Add(trailer);
            db.SaveChanges();
            return trailer.Id;
        }
        
        private void SaveDocTrailekDb(string path, string id, string nameDoc)
        {
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            DocumentTruckAndTrailers documentTruckAndTrailers = new DocumentTruckAndTrailers()
            {
                DocPath = path,
                IdTr = Convert.ToInt32(id),
                NameDoc = nameDoc,
                TypeTr = "Trailer",
                TypeDoc = pref
            };
            db.DocumentTruckAndTrailers.Add(documentTruckAndTrailers);
            db.SaveChanges();
        }
        
        private void RemoveTrailerDb(string id)
        {
            db.Trailers.Remove(db.Trailers.FirstOrDefault(t => t.Id.ToString() == id));
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
        
        private void EditTrailerDb(int idTrailer, string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns)
        {
            Trailer trailer = db.Trailers.FirstOrDefault(t => t.Id == idTrailer);
            if (trailer != null)
            {
                trailer.Name = name;
                trailer.Type = typeTrailer;
                trailer.Year = year;
                trailer.Make = make;
                trailer.HowLong = howLong;
                trailer.Vin = vin;
                trailer.Owner = owner;
                trailer.Color = color;
                trailer.Plate = plate;
                trailer.Exp = exp;
                trailer.AnnualIns = annualIns;
                db.SaveChanges();
            }
        }
        
        private async Task<Trailer> GetTrailerDb(string idDriver)
        {
            Trailer trailer = null;
            Driver driver = await db.Drivers.Include(d => d.InspectionDrivers).FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);
            if (driver != null && driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
            {
                InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
                trailer = await db.Trailers.FirstOrDefaultAsync(t => t.Id == inspectionDriver.IdITrailer);
            }
            return trailer;
        }
        
        private ITr GetTrailersByIdDb(int idTr)
        {
            return db.Trailers.FirstOrDefault(t => t.Id == idTr);
        }
        
        private ITr GetTruckByIdDb(int idTr)
        {
            return db.Trucks.FirstOrDefault(t => t.Id == idTr);
        }
        
        private List<Truck> GetTrucksDb(string idCompany)
        {
            return db.Trucks.Where(t => t.CompanyId.ToString() == idCompany).ToList();
        }
        
        private void RemoveTruckDb(string id)
        {
            db.Remove(db.Trucks.FirstOrDefault(t => t.Id.ToString() == id));
            db.SaveChanges();
        }
        
        private async Task<Truck> GetTruckByPlateDb(string truckPlate)
        {
            return await db.Trucks.FirstOrDefaultAsync(t => t.PlateTruk == truckPlate);
        }
        
        private async Task<List<DocumentTruckAndTrailers>> GetTruckDocDB(string id)
        {
            return await db.DocumentTruckAndTrailers.Where(d => d.TypeTr == "Truck" && d.IdTr.ToString() == id).ToListAsync();
        }
        
        private async Task<Truck> GetTruckDb(string idDriver)
        {
            Truck truck = null;
            Driver driver = await db.Drivers.Include(d => d.InspectionDrivers).FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);
            if (driver != null && driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
            {
                InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
                truck = await db.Trucks.FirstOrDefaultAsync(t => t.Id == inspectionDriver.IdITruck);
            }
            return truck; 
        }
    }
}