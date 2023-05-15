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
        Task<List<DocumentTruckAndTrailers>> GetTruckDoc(string id);
        Task<Dictionary<string, string>> GetBaseTruckDoc(string id);
        Task<Truck> GetTruckByPlate(string truckPlate);
        TruckViewModel GetTruckById(int idTruck);
        Task<Trailer> GetTrailer(string idDriver);

        void EditTrailer(TrailerViewModel model);
        TrailerViewModel GetTrailerById(int idTrailer);

        Task<Trailer> GetTrailerByPlate(string trailerPlate);
        Task<List<Trailer>> GetTrailers(int page, string idCompany);
        Task<int> GetCountTrailersPages(string idCompany);
        Task RemoveTrailer(string id);
        ITr GetTr(int idTr, string typeTransport);
        Task<List<DocumentTruckAndTrailers>> GetTrailerDoc(string id);
        List<InspectionDriver> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date);
        Task RemoveTruck(string id);
        Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, string id);
        string GetTypeTransport(int idTr, string typeTransport);

        Task CreateTrailer(TrailerViewModel trailer,string idCompany, IFormFile trailerRegistrationDoc,
            IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc);

        Task CreateTruck(TruckViewModel truck, string idCompany, IFormFile truckRegistrationDoc, IFormFile truckLeaseAgreementDoc,
            IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, IFormFile nYHUTDoc);

        Task EditTruck(TruckViewModel model, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc);
        int AddProfile(string idCompany, int idTr, string typeTransport);
        Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, string id);
    }
}