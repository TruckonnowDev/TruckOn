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
        public async Task<Dictionary<string, string>> GetBaseTruckDoc(string id)
        {
            return await GetBaseTruckDocDb(id);
        }

        public async Task<Truck> GetTruckByPlate(string truckPlate)
        {
            return await GetTruckByPlateDb(truckPlate);
        }
        
        public async Task RemoveTruck(string id)
        {
            await RemoveTruckDb(id);
        }
        
        public async Task<List<Truck>> GetTrucks(int page, string idCompany)
        {
            return await GetTrucksDb(page, idCompany);
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
        
        public async Task RemoveTrailer(string id)
        {
            await RemoveTrailerDb(id);
        }
        
        public async Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (uploadedFile.Length > maxFileLength) return;

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
            
            return tr is TruckViewModel ? TypeTransportVehikle.Truck.ToString() : TypeTransportVehikle.Trailer.ToString();
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

            if (trailerRegistrationDoc != null)
            {
                await SaveDocTrailer(trailerRegistrationDoc, DocAndFileConstants.TrailerRegistration, id.ToString());
            }

            if (trailerAnnualInspectionDoc != null)
            {
                await SaveDocTrailer(trailerAnnualInspectionDoc, DocAndFileConstants.TrailerInspection, id.ToString());
            }

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
            
            if(truckRegistrationDoc != null)
            {
                await SaveDocTruck(truckRegistrationDoc, DocAndFileConstants.TruckRegistration, id.ToString());
            }

            if (truckLeaseAgreementDoc != null)
            {
                await SaveDocTruck(truckLeaseAgreementDoc, DocAndFileConstants.TruckAgreement, id.ToString());
            }

            if (truckAnnualInspection != null)
            {
                await SaveDocTruck(truckAnnualInspection, DocAndFileConstants.TruckInspection, id.ToString());

            }
            
            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, DocAndFileConstants.BobTailPhysicalDamage, id.ToString());
            }
            
            if (nYHUTDoc != null)
            {
                await SaveDocTruck(nYHUTDoc, DocAndFileConstants.NyHit, id.ToString());
            }
        }
        
        public async Task EditTruck(TruckViewModel model, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc)
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

            if (truckRegistrationDoc != null)
            {
                await SaveDocTruck(truckRegistrationDoc, DocAndFileConstants.TruckRegistration, model.Id.ToString());
            }

            if (truckLeaseAgreementDoc != null)
            {
                await SaveDocTruck(truckLeaseAgreementDoc, DocAndFileConstants.TruckAgreement, model.Id.ToString());
            }

            if (truckAnnualInspection != null)
            {
                await SaveDocTruck(truckAnnualInspection, DocAndFileConstants.TruckInspection, model.Id.ToString());

            }

            if (bobTailPhysicalDamage != null)
            {
                await SaveDocTruck(bobTailPhysicalDamage, DocAndFileConstants.BobTailPhysicalDamage, model.Id.ToString());
            }

            if (nYHUTDoc != null)
            {
                await SaveDocTruck(nYHUTDoc, DocAndFileConstants.NyHit, model.Id.ToString());
            }
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

            var documentInDb = db.DocumentTruckAndTrailers.FirstOrDefault(x => x.NameDoc == nameDoc && x.IdTr == Convert.ToInt32(id));

            if (documentInDb != null)
            {
                documentInDb.DocPath = path;
                documentInDb.TypeDoc = pref;

                db.SaveChanges();
                return;
            }

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
            if (uploadedFile.Length > maxFileLength) return;

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
        
        private async Task<int> CreateTrailerDb(TrailerViewModel trailerViewModel)
        {
            var trailer = mapper.Map<Trailer>(trailerViewModel);
            await db.Trailers.AddAsync(trailer);
            
            await db.SaveChangesAsync();
            
            return trailer.Id;
        }
        
        private async Task<int> CreateTruckDb(TruckViewModel truckModel)
        {
            var truck = mapper.Map<Truck>(truckModel);
            await db.Trucks.AddAsync(truck);
            
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
        
        private async Task RemoveTrailerDb(string id)
        {
            var trailer = await db.Trailers.FirstOrDefaultAsync(t => t.Id.ToString() == id);
            if (trailer == null) return;
            
            db.Trailers.Remove(trailer);
            
            await db.SaveChangesAsync();
        }
        
        private async Task RemoveTruckDb(string id)
        {
            var truck = await db.Trucks.FirstOrDefaultAsync(t => t.Id.ToString() == id);
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
        
        private async Task<List<Truck>> GetTrucksDb(int page, string idCompany)
        {
            var trucks = db.Trucks.Where(t => t.CompanyId.ToString() == idCompany).OrderByDescending(x => x.Id).AsQueryable();

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
            return await db.Trucks.FirstOrDefaultAsync(t => t.PlateTruk == truckPlate);
        }
        
        private async Task<List<DocumentTruckAndTrailers>> GetTruckDocDb(string id)
        {
            return await db.DocumentTruckAndTrailers
                .Where(d => d.TypeTr == TruckAndTrailerConstants.Truck && d.IdTr.ToString() == id)
                .ToListAsync();
        }
        
        private async Task<Dictionary<string, string>> GetBaseTruckDocDb(string id)
        {
            var docs = await db.DocumentTruckAndTrailers
                .Where(d => d.TypeTr == TruckAndTrailerConstants.Truck && d.IdTr.ToString() == id)
                .ToListAsync();

            var keyValues = new Dictionary<string, string>();

            foreach(var item in docs)
            {
                keyValues.Add(item.NameDoc, GetFileNameWithPath(item.DocPath));
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