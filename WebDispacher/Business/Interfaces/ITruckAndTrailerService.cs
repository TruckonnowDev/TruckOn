using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using WebDispacher.Models;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Truck;

namespace WebDispacher.Business.Interfaces
{
    public interface ITruckAndTrailerService
    {
        Task<Truck> GetTruck(string idDriver);
        Task<List<Truck>> GetTrucks(int page, string idCompany);
        Task<int> GetCountTrucksPages(string idCompany);
        Task<List<DocumentTruck>> GetTruckDoc(int id);
        Task<Dictionary<string, string>> GetBaseTruckDoc(string id);
        Task<Truck> GetTruckByPlate(string truckPlate);
        Task<TruckViewModel> GetTruckById(int truckId);
        Task<Trailer> GetTrailer(string idDriver);
        TrailerViewModel GetTrailerById(int idTrailer);
        Task<List<TruckType>> GetTruckTypes(string category);
        Task<Trailer> GetTrailerByPlate(string trailerPlate);
        Task<List<Trailer>> GetTrailers(int page, string idCompany);
        Task<int> GetCountTrailersPages(string idCompany);
        Task RemoveTrailer(int id);
        Task<List<VehicleCategory>> GetVehicleCategiries();
        Task<ITr> GetTr(int idTr, string typeTransport);
        Task<List<DocumentTrailer>> GetTrailerDocsById(int id);
        List<DriverInspection> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date);
        Task RemoveTruck(int id);
        Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, int trailerId, string dateTimeUpload);
        Task CreateTrailer(TrailerViewModel trailer, string idCompany,
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc, string dateTimeLocal);
        string GetTypeTransport(int idTr, string typeTransport);
        Task RemoveDocTrailer(int docId);
        Task RemoveDocTruck(int docId);

        Task CreateTruck(TruckViewModel truck, string idCompany, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string dateTimeLocal);

        Task EditTruck(TruckViewModel model, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string companyId, string localDate);

        Task EditTrailer(TrailerViewModel model, string companyId, string localDate);

        Task<int> AddProfile(string idCompany, int idTr, string typeTransport);
        Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, int truckId, string dateTimeUpload);

        Task<string> GetTruckTypeSlugByName(string truckName);
    }
}