﻿using System.Collections.Generic;
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
        List<Truck> GetTrucks(string idCompany);
        Task<List<DocumentTruckAndTrailers>> GetTruckDoc(string id);
        Task<Truck> GetTruckByPlate(string truckPlate);
        TruckViewModel GetTruckById(int idTruck);
        Task<Trailer> GetTrailer(string idDriver);

        void EditTrailer(TrailerViewModel model);
        TrailerViewModel GetTrailerById(int idTrailer);

        Task<Trailer> GetTrailerByPlate(string trailerPlate);
        List<Trailer> GetTrailers(string idCompany);
        void RemoveTrailer(string id);
        ITr GetTr(int idTr, string typeTransport);
        Task<List<DocumentTruckAndTrailers>> GetTrailerDoc(string id);
        List<InspectionDriver> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date);
        void RemoveTruck(string id);
        Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, string id);
        string GetTypeTransport(int idTr, string typeTransport);

        Task CreateTrailer(TrailerViewModel trailer,string idCompany, IFormFile trailerRegistrationDoc,
            IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc);

        Task CreateTruck(TruckViewModel truck, string idCompany, IFormFile truckRegistrationDoc, IFormFile truckLeaseAgreementDoc,
            IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, IFormFile nYHUTDoc);

        void EditTruck(TruckViewModel model);
        int AddProfile(string idCompany, int idTr, string typeTransport);
        Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, string id);
    }
}