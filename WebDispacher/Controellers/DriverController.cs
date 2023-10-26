using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Models;
using WebDispacher.Models.Driver;
using WebDispacher.Service;
using WebDispacher.ViewModels.Driver;

namespace WebDispacher.Controellers
{
    public class DriverController : BaseController
    {
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly IDriverService driverService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;

        public DriverController(
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            IOrderService orderService,
            IDriverService driverService,
            ICompanyService companyService) : base(userService)
        {
            this.orderService = orderService;
            this.companyService = companyService;
            this.driverService = driverService;
            this.truckAndTrailerService = truckAndTrailerService;
        }

        [HttpGet]
        [Route("Driver/Drivers")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Drivers(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var drivers = await driverService.GetDriversByCompanyId(page, CompanyId);

                var countPages = await driverService.GetCountDriversPages(CompanyId);

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View("FullAllDrivers", drivers);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Driver/Check")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CheckDriver(string nameDriver, string driversLicense, string comment)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                ViewBag.DriversLicense = driversLicense;
                ViewBag.NameDriver = nameDriver;

                var drivers = await driverService.GetDriversReport(nameDriver, driversLicense, CompanyId);
                    
                return View("DriverCheck", drivers);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Welcome/Driver/Check")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> WelcomeDriverCheckReport(string nameDriver, string driversLicense, string countDriverReports)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;
            try
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.DriversLicense = driversLicense;
                ViewBag.NameDriver = nameDriver;
                ViewBag.CountDriverReports = countDriverReports;
                ViewBag.DriverReports = await driverService.GetDriversReport(nameDriver, driversLicense, CompanyId);
            }
            catch (Exception)
            {

            }
            
            return View("WelcomDriverCheck");
        }

        [HttpPost]
        [Route("Welcome/Driver/Check/Report")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public string WelcomeAddReport(string fullName, string driversLicenseNumber)
        {
            string actionResult = null;
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;
            try
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                var countDriverReports = driverService.CheckReportDriver(fullName, driversLicenseNumber);
                
                actionResult = countDriverReports > 0 ? $"true,{fullName},{driversLicenseNumber},{countDriverReports}" : DocAndFileConstants.False;
            }
            catch (Exception)
            {

            }
            
            return actionResult;
        }

        [HttpGet]
        [Route("Driver/AddReport")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult AddReport()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                return View("ReportDriver", new DriverReportViewModel());
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/AddReport")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> AddReportPost(DriverReportViewModel driverReport, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                if (ModelState.IsValid)
                {
                    if (driverReport.Terminated == DriverConstants.Undefined)
                    {
                        driverReport.Terminated = DriverConstants.Yes;
                    }

                    if (driverReport.Experience == DriverConstants.Undefined)
                    {
                        driverReport.Experience = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(driverReport.Experience)
                        && driverReport.Experience != DriverConstants.Undefined
                        && driverReport.Experience.LastIndexOf(',') == driverReport.Experience.Length - 2)
                    {
                        driverReport.Experience =
                            driverReport.Experience.Remove(driverReport.Experience.Length - 2);
                    }

                    driverReport.English = GetLevel(driverReport.English);
                    driverReport.ReturnedEquipmen = GetLevel(driverReport.ReturnedEquipmen);
                    driverReport.WorkingEfficiency = GetLevel(driverReport.WorkingEfficiency);
                    driverReport.EldKnowledge = GetLevel(driverReport.EldKnowledge);
                    driverReport.DrivingSkills = GetLevel(driverReport.DrivingSkills);
                    driverReport.PaymentHandling = GetLevel(driverReport.PaymentHandling);
                    driverReport.AlcoholTendency = GetLevel(driverReport.AlcoholTendency);
                    driverReport.DrugTendency = GetLevel(driverReport.DrugTendency);

                    await driverService.AddNewReportDriver(driverReport, localDate);

                    return Redirect(Config.BaseReqvesteUrl);
                }
                else
                {
                    return View("ReportDriver", driverReport);
                }
            }
            catch (Exception)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Welcome/AddReport")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult WelcomeAddReport()
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;
            try
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            }
            catch (Exception)
            {

            }
            
            return View("WelcomReportDriver");
        }

        [HttpPost]
        [Route("Welcome/AddReport")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> WelcomeAddReport(DriverReportViewModel driverReport, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;
            try
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                var s = driverReport.Experience.LastIndexOf(',');
                
                if (driverReport.Terminated == DriverConstants.Undefined)
                {
                    driverReport.Terminated = string.Empty;
                }
                if (driverReport.Experience == DriverConstants.Undefined)
                {
                    driverReport.Experience = string.Empty;
                }
                if (!string.IsNullOrEmpty(driverReport.Experience) 
                    && driverReport.Experience != DriverConstants.Undefined 
                    && driverReport.Experience.LastIndexOf(',') == driverReport.Experience.Length - 2)
                {
                    driverReport.Experience = driverReport.Experience.Remove(driverReport.Experience.Length - 2);
                }
                
                await driverService.AddNewReportDriver(driverReport, localDate);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/Drivers/CreateDriver")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateDriver()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                return View("CreateDriver");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/Drivers/CreateDriver")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CreateDriver(CreateDriverViewModel model,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc,
            IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDoc, string dateTimeLocal)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Email) ||
                    string.IsNullOrEmpty(model.DriverControl.Password)) return View("CreateDriver");

                    model.CompanyId = Convert.ToInt32(CompanyId);
                    
                    await driverService.CreateDriver(model,
                        dLDoc, medicalCardDoc, sSNDoc, proofOfWorkAuthorizationOrGCDoc, dQLDoc, contractDoc, drugTestResultsDoc, dateTimeLocal);

                }
                else
                {
                    return View("CreateDriver", model);
                }
                        
                        
                return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/Drivers/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> RemoveDriver(DriverReportModel model, string localDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (model.Terminated == DriverConstants.Undefined)
                    {
                        model.Terminated = DriverConstants.Yes;
                    }

                    if (model.Experience == DriverConstants.Undefined)
                    {
                        model.Experience = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(model.Experience) && model.Experience != DriverConstants.Undefined
                        && model.Experience.LastIndexOf(',') ==
                        model.Experience.Length - 2)
                    {
                        model.Experience = model.Experience.Remove(model.Experience.Length - 2);
                    }

                    await driverService.RemoveDriver(CompanyId, model, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/Drivers/Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditDriver(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var driver = await driverService.GetEditCompanyDriverById(CompanyId, id);
                
               return View(driver);

            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/Drivers/Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> EditDriver(EditDriverViewModel model, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    
                    await driverService.EditDriver(model, localDate);
                        
                    return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("EditDriver", model);
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/InspactionTrucks")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> ViewAllInspactionDate(string driverId, string truckId, string trailerId, string date)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                var trucks = await truckAndTrailerService.GetTrucks(0, CompanyId);
                var trailers = await truckAndTrailerService.GetTrailers(0, CompanyId);
                    
                var inspectionTruck = truckAndTrailerService.GetInspectionTrucks(driverId, truckId, trailerId, date)
                    .Select(x => new InspectinView()
                    {
                        Id = x.Id,
                        DateTimeInspection = x.DateTimeInspection,
                        TrailerInformation = trailers.FirstOrDefault(t => t.Id == x.TrailerId) != null ? $"{trailers.FirstOrDefault(t => t.Id == x.TrailerId).Brand}, Plate: {trailers.FirstOrDefault(t => t.Id == x.TrailerId).Plate}" : "---------------",
                        TruckInformation = trucks.FirstOrDefault(t => t.Id == x.TruckId) != null ? $"{trucks.FirstOrDefault(t => t.Id == x.TruckId).Brand} {trucks.FirstOrDefault(t => t.Id == x.TruckId).Model}, Plate: {trucks.FirstOrDefault(t => t.Id == x.TruckId).Plate}" : "---------------",
                        NameDriver = drivers.FirstOrDefault(d => d.Inspections.FirstOrDefault(i => i.Id == x.Id) != null) == null ? "N/D" : drivers.FirstOrDefault(d => d.Inspections.FirstOrDefault(i => i.Id == x.Id) != null).FirstName,
                    })
                    .ToList();
                try
                {
                    ViewBag.InspectionTruck = inspectionTruck.OrderBy(x => x.DateTimeInspection).ToList();
                }
                catch
                {
                    ViewBag.InspectionTruck = inspectionTruck;
                }

                ViewBag.IdDriver = driverId;
                ViewBag.IdTruck = truckId;
                ViewBag.IdTrailer = trailerId;
                ViewBag.SelectData = date;

                return View("AllInspactionTruckData", new FullInformationInspection
                {
                    Drivers = drivers,
                    Trucks = trucks,
                    Trailers = trailers,
                    Inspection = inspectionTruck
                });
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/InspactionTruck")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> ViewInspaction(string idInspection, string idDriver, string date)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                var trucks = await truckAndTrailerService.GetTrucks(0, CompanyId);
                var trailers = await truckAndTrailerService.GetTrailers(0, CompanyId);
                var inspectionDriver = driverService.GetInspectionTruck(idInspection);
                var drivers = driverService.GetDriver(inspectionDriver.Id.ToString());
                    
                ViewBag.InspectionTruck = inspectionDriver;
                ViewBag.Drivers = drivers;
                //ViewBag.Trailer = trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer) != null ? $"{trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer).Make}, Plate: {trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer).Plate}" : "---------------";
                //ViewBag.Truck = trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck) != null ? $"{trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck).Make} {trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck).Model}, Plate: {trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck).PlateTruk}" : "---------------";
                ViewBag.SelectData = date;
                    
                return View("OneInspektion");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/Remind/Inspection")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string SendRemindInspection(int driverId)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var isInspectionDriverToDay = orderService.SendRemindInspection(driverId);

                return isInspectionDriverToDay ? DocAndFileConstants.False : DocAndFileConstants.True;
            }
            catch (Exception)
            {
                return NavConstants.Error;
            }
        }

        [Route("Driver/Doc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GoToViewCompanykDoc(int id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var driverDocs = await driverService.GetDriverDoc(id);
                ViewBag.DriverId = id;
                    
                return View($"DriverDocument", driverDocs);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Driver/SaveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveDoc(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) &&  uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await driverService.SaveDocDriver(uploadedFile, nameDoc, id, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Driver/Doc?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Driver/RemoveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveDoc(int docId, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await driverService.RemoveDocDriver(docId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Driver/Doc?id={id}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Driver/GetDock")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDock(string docPath, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, type);
        }

        [HttpGet]
        [Route("Driver/Image")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GetShipping(string name, string type)
        {
            try
            {
                var imageFileStream = System.IO.File.OpenRead(name);
                var file = File(imageFileStream, $"image/{type}");
                file.FileDownloadName = "file1";
                return file;
            }
            catch(Exception e)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                response.Content = new StringContent(e.Message);

                return NotFound(await Task.FromResult(response));
            }
        }

        [Route("Driver/GetDockPDF")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDockPDF(string docPath)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, DocAndFileConstants.ContentTypePdf);
        }

        private string GetLevel(string value)
        {
            var level = DocAndFileConstants.NoneLevel;
            
            switch (value)
            {
                case DocAndFileConstants.ZeroLevel:
                    level = DocAndFileConstants.NoneLevel;
                    break;
                case DocAndFileConstants.OneLevel:
                    level = DocAndFileConstants.PoorLevel;
                    break;
                case DocAndFileConstants.TwoLevel:
                    level = DocAndFileConstants.MiddleLevel;
                    break;
                case DocAndFileConstants.ThreeLevel:
                    level = DocAndFileConstants.GoodLevel;
                    break;
            }
            
            return level;
        }
    }
}