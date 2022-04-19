using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using WebDispacher.Models;

namespace WebDispacher.Business.Interfaces
{
    public interface ITruckAndTrailerService
    {
        Task<Truck> GetTruck(string idDriver);
        List<Truck> GetTrucks(string idCompany);
        Task<List<DocumentTruckAndTrailers>> GetTruckDoc(string id);
        Task<Truck> GetTruckByPlate(string truckPlate);
        Truck GetTruckById(int idTruck);
        Trailer GetTrailerById(int idTrailer);
        Task<Trailer> GetTrailer(string idDriver);

        void EditTrailer(int idTrailer, string name, string typeTrailer, string year, string make, string howLong,
            string vin, string owner, string color, string plate, string exp, string annualIns);

        Task<Trailer> GetTrailerkByPlate(string trailerPlate);
        List<Trailer> GetTrailers(string idCompany);
        void RemoveTrailer(string id);
        ITr GetTr(int idTr, string typeTransport);
        Task<List<DocumentTruckAndTrailers>> GetTraileDoc(string id);
        List<InspectionDriver> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date);
        void RemoveTruck(string id);
        Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, string id);
        string GetTypeTransport(int idTr, string typeTransport);

        void CreateTrailer(string name, string typeTrailer, string year, string make, string howLong, string vin,
            string owner, string color, string plate, string exp, string annualIns,
            string idCompany, IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc,
            IFormFile leaseAgreementDoc);

        void CreateTruk(string nameTruk, string yera, string make, string model, string typeTruk, string state,
            string exp, string vin, string owner, string plateTruk,
            string color, string idCompany, IFormFile truckRegistrationDoc, IFormFile truckLeaseAgreementDoc,
            IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, IFormFile nYHUTDoc);

        void EditTruck(int idTruck, string nameTruck, string year, string make,
            string model, string typeTruck, string state, string exp, string vin, string owner, string plateTruck,
            string color);
        int AddProfile(string idCompany, int idTr, string typeTransport);
        Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, string id);
    }
}