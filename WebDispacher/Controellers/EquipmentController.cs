using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Hosting.Internal;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Truck;

namespace WebDispacher.Controellers
{
    [Route("Equipment")]
    public class EquipmentController : BaseController
    {
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;

        public EquipmentController(
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            IOrderService orderService,
            ICompanyService companyService) : base(userService)
        {
            this.orderService = orderService;
            this.companyService = companyService;
            this.truckAndTrailerService = truckAndTrailerService;
        }

        [Route("Trucks")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Trucks(int page = 1)
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

                var trucks = await truckAndTrailerService.GetTrucks(page, CompanyId);

                var countPages = await truckAndTrailerService.GetCountTrucksPages(CompanyId);

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View($"AllTruck", trucks);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailers")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Trailers(int page = 1)
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
                
                var trailers = await truckAndTrailerService.GetTrailers(page, CompanyId);

                var countPages = await truckAndTrailerService.GetCountTrailersPages(CompanyId);

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View($"AllTrailer", trailers);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        public IActionResult LoadVehicleImages(string vehicleName)
        {
            ViewData["VehicleName"] = vehicleName;
            return PartialView("~/Views/Equipment/VehicleImages.cshtml", ViewData);
        }

        [HttpGet]
        [Route("GetImages")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetImages(string vehicleSlug)
        {
            if (string.IsNullOrEmpty(vehicleSlug)) return Json(new string[0]);

            var directoryPath = $"../TruckPattern/" + vehicleSlug;

            try
            {
                var imageFiles = Directory.GetFiles(directoryPath)
                    .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    .Select(Path.GetFileName);

                return Json(imageFiles);
            }
            catch(Exception ex) 
            { 
            
            }

            return Json(new string[0]);
        }

        [Route("GetTruckSlug")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTruckSlugByName(string vehicleName)
        {
            var vehicleSlug = await truckAndTrailerService.GetTruckTypeSlugByName(vehicleName);

            return Json(vehicleSlug);
        }

        [Route("GetTruckTypes")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTruckTypes(string categoryId)
        {
            var truckTypes = await truckAndTrailerService.GetTruckTypes(categoryId);

            return Json(truckTypes);
        }
        
        [Route("GetVehicleCategiries")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetVehicleCategiries()
        {
            var vehicleCategories = await truckAndTrailerService.GetVehicleCategiries();

            return Json(vehicleCategories);
        }

        [Route("Truck/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTruck(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTruck(id);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Truck/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> DeleteTruck(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTruck(id);

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        [HttpGet]
        [Route("CreateTruck")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CreateTruck()
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
                    
                return View("CreateTruck");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("CreateTruck")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CreateTruck(TruckViewModel truck, IFormFile truckRegistrationDoc, 
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, 
            IFormFile nYHUTDoc, string dateTimeLocal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.CreateTruck(truck, CompanyId, truckRegistrationDoc,
                        truckLeaseAgreementDoc, truckAnnualInspection, bobTailPhysicalDamage, nYHUTDoc, dateTimeLocal);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                if(truck.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = truck.VehicleCategoryId;
                }
                
                if(truck.TruckTypeId != null)
                {
                    ViewData["SelectedTruckTypeId"] = truck.TruckTypeId;
                }
                return View("CreateTruck", truck);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTrailer(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTrailer(id);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Trailer/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> DeleteTrailer(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTrailer(id);

                return true;
            }
            catch (Exception)
            {

            }
            
            return false;
        }

        [HttpGet]
        [Route("CreateTrailer")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateTrailer()
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
                    
                return View("CreateTraler");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("CreateTrailer")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CreateTrailer(TrailerViewModel trailer,
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc, string dateTimeLocal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   /* if (DateTime.TryParseExact(trailer.Exp, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {

                    }*/

                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.CreateTrailer(trailer, CompanyId, trailerRegistrationDoc,
                        trailerAnnualInspectionDoc, leaseAgreementDoc, dateTimeLocal);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("CreateTraler", trailer);
            }
            

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditTruck")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditTruck(int truckId)
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
                    
                var truck = await truckAndTrailerService.GetTruckById(truckId);

                ViewBag.TruckDocs = await truckAndTrailerService.GetBaseTruckDoc(truckId.ToString());

                if (truck.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = truck.VehicleCategoryId;
                }

                if (truck.TruckTypeId != null)
                {
                    ViewData["SelectedTruckTypeId"] = truck.TruckTypeId;
                }

                return View(truck);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("EditTruck")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditTruck(TruckViewModel truck, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string localDate)
        {
            if (ModelState.IsValid && truck.Id != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.EditTruck(truck, truckRegistrationDoc,
                        truckLeaseAgreementDoc, truckAnnualInspection, bobTailPhysicalDamage, nYHUTDoc, CompanyId, localDate);


                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                if (truck.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = truck.VehicleCategoryId;
                }

                if (truck.TruckTypeId != null)
                {
                    ViewData["SelectedTruckTypeId"] = truck.TruckTypeId;
                }
                ViewBag.TruckDocs = await truckAndTrailerService.GetBaseTruckDoc(truck.Id.ToString());
                return View("EditTruck", truck);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditTrailer")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult EditTrailer(int trailerId)
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
                    
                var model = truckAndTrailerService.GetTrailerById(trailerId);
                    
                return View(model);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost] 
        [Route("EditTrailer")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditTrailer(TrailerViewModel trailer, string localDate)
        {
            if (ModelState.IsValid && trailer.Id != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.EditTrailer(trailer, CompanyId, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/trailers");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View(trailer);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("SaveFile")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string AddFile(IFormFile uploadedFile, string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (uploadedFile != null)
                {
                    var path = $"../Document/Truck/{id}/" + uploadedFile.FileName;
                    Directory.CreateDirectory($"../Document/Truck/{id}");
                    orderService.SavePath(id, path);
                        
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        uploadedFile.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception e)
            {
                return DocAndFileConstants.False;
            }
            
            return DocAndFileConstants.True;
        }

        [HttpGet]
        [Route("Document")]
        public async Task<IActionResult> Get(string id)
        {
            FileStream stream = null;
            try
            {
                var docPath = await orderService.GetDocument(id);
                
                stream = new FileStream(docPath, FileMode.Open);
            }
            catch (Exception)
            {
                stream = null;
            }
            
            return new FileStreamResult(stream, DocAndFileConstants.ContentTypePdf);
        }

        [Route("Truck/Doc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTruckDoc(int id)
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

                    var truckDocs = await truckAndTrailerService.GetTruckDoc(id);

                    ViewBag.TruckId = id;
                    
                    return View("DocTruck", truckDocs);
                }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/Doc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTraileDoc(int id)
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
                var trailerDocs = await truckAndTrailerService.GetTrailerDocsById(id);
                ViewBag.TrailerId = id;
                    
                return View("DocTrailer", trailerDocs);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Truck/SaveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> SaveDocTruck(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTruck(uploadedFile, nameDoc, id, localDate);
                        
                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Trailer/DocSaveById")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Trailer(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTrailer(uploadedFile, nameDoc, id, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Truck/DocSaveById")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> DocSaveById(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTruck(uploadedFile, nameDoc, id, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc/?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks/");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/SaveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveDocTrailer(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTrailer(uploadedFile, nameDoc, id, localDate);
                        
                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/RemoveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTrailerDoc(int docId, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveDocTrailer(docId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Truck/RemoveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTruckDoc(int docId, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveDocTruck(docId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc?id={id}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("GetDock")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDock(string docPath, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, type);
        }

        [Route("GetDockPDF")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDockPDF(string docPath)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, DocAndFileConstants.ContentTypePdf);
        }

        [HttpGet]
        [Route("Image")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetShiping(string name, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(name);
            
            return File(imageFileStream, $"image/{type}");
        }
    }
}