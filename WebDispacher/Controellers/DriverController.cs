using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
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

        [Route("Driver/Drivers")]
        public async Task<IActionResult> Drivers(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Drivers = await driverService.GetDrivers(page, idCompany);

                    var countPages = await driverService.GetCountDriversPages(idCompany);

                    ViewBag.CountPages = countPages;

                    ViewBag.SelectedPage = page;

                    return View("FullAllDrivers");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Driver/Check")]
        public IActionResult CheckDriver(string nameDriver, string driversLicense, string comment)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.DriversLicense = driversLicense;
                    ViewBag.NameDriver = nameDriver;
                    ViewBag.DriverReports = driverService.GetDriversReport(nameDriver, driversLicense);
                    
                    return View("DriverCheck");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Welcome/Driver/Check")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult WelcomeDriverCheckReport(string nameDriver, string driversLicense, string countDriverReports)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;
            try
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.DriversLicense = driversLicense;
                ViewBag.NameDriver = nameDriver;
                ViewBag.CountDriverReports = countDriverReports;
                ViewBag.DriverReports = driverService.GetDriversReport(nameDriver, driversLicense);
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
        public IActionResult AddReport()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    return View("ReportDriver", new DriverReportViewModel());
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/AddReport")]
        public IActionResult AddReportPost(DriverReportViewModel driverReport)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

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

                        driverService.AddNewReportDriver(driverReport);
                        return Redirect(Config.BaseReqvesteUrl);
                    }
                    else
                    {
                        return View("ReportDriver", driverReport);
                    }
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
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
        public IActionResult WelcomeAddReport(DriverReportViewModel driverReport)
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
                
                driverService.AddNewReportDriver(driverReport);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/Drivers/CreateDriver")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateDriver()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    return View("CreateDriver");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/Drivers/CreateDriver")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateDriver(DriverViewModel driver,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc,
            IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDoc, string dateTimeLocal)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    if (string.IsNullOrEmpty(driver.FullName) || string.IsNullOrEmpty(driver.EmailAddress) ||
                        string.IsNullOrEmpty(driver.Password)) return View("CreateDriver");
                    
                    driver.CompanyId = Convert.ToInt32(idCompany);
                        
                    driverService.CreateDriver(driver,
                        dLDoc, medicalCardDoc, sSNDoc, proofOfWorkAuthorizationOrGCDoc, dQLDoc, contractDoc, drugTestResultsDoc, dateTimeLocal);
                        
                    return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/Drivers/Remove")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult RemoveDriver(DriverReportModel model, string localDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                    {
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

                        driverService.RemoveDrive(idCompany, model, localDate);

                        return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
                    }

                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
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
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult EditeDriver(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    var driver = driverService.GetDriverByIdViewModel(id);
                    
                    if (driver != null)
                    {
                        return View("EditDriver", driver);
                    }

                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/Drivers/Edit")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult EditDriver(DriverViewModel driver)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    
                    if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                    {
                        driverService.EditDriver(driver);
                        
                        return Redirect($"{Config.BaseReqvesteUrl}/Driver/Drivers");
                    }

                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
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
        [Route("Driver/InspactionTrucks")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> ViewAllInspactionDate(string idDriver, string idTruck, string idTrailer, string date)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    var drivers = await driverService.GetDrivers(idCompany);
                    var trucks = await truckAndTrailerService.GetTrucks(0, idCompany);
                    var trailers = await truckAndTrailerService.GetTrailers(0, idCompany);
                    
                    var inspectionTruck = truckAndTrailerService.GetInspectionTrucks(idDriver, idTruck, idTrailer, date)
                        .Select(x => new InspectinView()
                        {
                            Id = x.Id,
                            Date = x.Date,
                            Trailer = trailers.FirstOrDefault(t => t.Id == x.IdITrailer) != null ? $"{trailers.FirstOrDefault(t => t.Id == x.IdITrailer).Make}, Plate: {trailers.FirstOrDefault(t => t.Id == x.IdITrailer).Plate}" : "---------------",
                            Truck = trucks.FirstOrDefault(t => t.Id == x.IdITruck) != null ? $"{trucks.FirstOrDefault(t => t.Id == x.IdITruck).Make} {trucks.FirstOrDefault(t => t.Id == x.IdITruck).Model}, Plate: {trucks.FirstOrDefault(t => t.Id == x.IdITruck).PlateTruk}" : "---------------",
                            NameDriver = drivers.FirstOrDefault(d => d.InspectionDrivers.FirstOrDefault(i => i.Id == x.Id) != null) == null ? "N/D" : drivers.FirstOrDefault(d => d.InspectionDrivers.FirstOrDefault(i => i.Id == x.Id) != null).FullName,
                        })
                        .ToList();
                    try
                    {
                        ViewBag.InspectionTruck = inspectionTruck.OrderBy(x => Convert.ToDateTime(x.Date)).ToList();
                    }
                    catch
                    {
                        ViewBag.InspectionTruck = inspectionTruck;
                    }

                    ViewBag.Drivers = drivers;
                    ViewBag.Trucks = trucks;
                    ViewBag.Trailers = trailers;
                    ViewBag.IdDriver = idDriver;
                    ViewBag.IdTruck = idTruck;
                    ViewBag.IdTrailer = idTrailer;
                    ViewBag.SelectData = date;
                    
                    return View("AllInspactionTruckData");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Driver/InspactionTruck")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> ViewInspaction(string idInspection, string idDriver, string date)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    var trucks = await truckAndTrailerService.GetTrucks(0, idCompany);
                    var trailers = await truckAndTrailerService.GetTrailers(0, idCompany);
                    var inspectionDriver = driverService.GetInspectionTruck(idInspection);
                    var drivers = driverService.GetDriver(inspectionDriver.Id.ToString());
                    
                    ViewBag.InspectionTruck = inspectionDriver;
                    ViewBag.Drivers = drivers;
                    ViewBag.Trailer = trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer) != null ? $"{trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer).Make}, Plate: {trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer).Plate}" : "---------------";
                    ViewBag.Truck = trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck) != null ? $"{trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck).Make} {trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck).Model}, Plate: {trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck).PlateTruk}" : "---------------";
                    ViewBag.SelectData = date;
                    
                    return View("OneInspektion");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Driver/Remind/Inspection")]
        public string SendRemindInspection(int idDriver)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isInspectionDriverToDay = orderService.SendRemindInspection(idDriver);
                    return isInspectionDriverToDay ? DocAndFileConstants.False : DocAndFileConstants.True;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {
                return NavConstants.Error;
            }
            
            return UserConstants.NotLogin;
        }

        [Route("Driver/Doc")]
        public async Task<IActionResult> GoToViewCompanykDoc(string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.DriverDoc = await driverService.GetDriverDoc(id);
                    ViewBag.DriverId = id;
                    
                    return View($"DriverDocument");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Driver/SaveDoc")]
        public IActionResult SaveDoc(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (!string.IsNullOrEmpty(nameDoc) && !string.IsNullOrEmpty(id) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                    {
                        driverService.SaveDocDriver(uploadedFile, nameDoc, id);

                        return Redirect($"{Config.BaseReqvesteUrl}/Driver/Doc?id={id}");
                    }

                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
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
        public IActionResult RemoveDoc(string idDock, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Driver, out var key, out var idCompany))
                {
                    driverService.RemoveDocDriver(idDock);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Driver/Doc?id={id}");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
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