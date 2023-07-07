using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
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
        public async Task<IActionResult> Trucks(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Trucks = await truckAndTrailerService.GetTrucks(page, idCompany);

                    var countPages = await truckAndTrailerService.GetCountTrucksPages(idCompany);

                    ViewBag.CountPages = countPages;

                    ViewBag.SelectedPage = page;

                    return View($"AllTruck");
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

        [Route("Trailers")]
        public async Task<IActionResult> Trailers(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Trailers = await truckAndTrailerService.GetTrailers(page, idCompany);

                    var countPages = await truckAndTrailerService.GetCountTrailersPages(idCompany);

                    ViewBag.CountPages = countPages;

                    ViewBag.SelectedPage = page;

                    return View($"AllTrailer");
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

        [Route("Truck/Remove")]
        public IActionResult RemoveTruck(string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    truckAndTrailerService.RemoveTruck(id);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
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
        [Route("Truck/Remove")]
        public async Task<bool> DeleteTruck(string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    await truckAndTrailerService.RemoveTruck(id);

                    return true;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        [HttpGet]
        [Route("CreateTruck")]
        public IActionResult CreateDriver()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    return View("CreateTruck");
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
        [Route("CreateTruck")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> CreateDriver(TruckViewModel truck, IFormFile truckRegistrationDoc, 
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, 
            IFormFile nYHUTDoc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        await truckAndTrailerService.CreateTruck(truck, idCompany, truckRegistrationDoc,
                            truckLeaseAgreementDoc, truckAnnualInspection, bobTailPhysicalDamage, nYHUTDoc);

                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/Remove")]
        public IActionResult RemoveTrailer(string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    truckAndTrailerService.RemoveTrailer(id);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
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
        [Route("Trailer/Remove")]
        public async Task<bool> DeleteTrailer(string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    await truckAndTrailerService.RemoveTrailer(id);

                    return true;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return false;
        }

        [HttpGet]
        [Route("CreateTrailer")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateTrailer()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    return View("CreateTraler");
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
        [Route("CreateTrailer")]
        [DisableRequestSizeLimit]
        public IActionResult CreateTrailer(TrailerViewModel trailer,
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        truckAndTrailerService.CreateTrailer(trailer, idCompany, trailerRegistrationDoc,
                            trailerAnnualInspectionDoc, leaseAgreementDoc);

                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }
            

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditTruck")]
        public async Task<IActionResult> EditTruck(int idTruck)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    var truck = truckAndTrailerService.GetTruckById(idTruck);

                    ViewBag.TruckDocs = await truckAndTrailerService.GetBaseTruckDoc(idTruck.ToString());

                    return View(truck);
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
        [Route("EditTruck")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> EditTruck(TruckViewModel truck, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc)
        {
            if (ModelState.IsValid && truck.Id != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        await truckAndTrailerService.EditTruck(truck, truckRegistrationDoc,
                            truckLeaseAgreementDoc, truckAnnualInspection, bobTailPhysicalDamage, nYHUTDoc);

                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditTrailer")]
        public IActionResult EditTrailer(int idTrailer)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    var model = truckAndTrailerService.GetTrailerById(idTrailer);
                    
                    return View(model);
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
        [Route("EditTrailer")]
        [DisableRequestSizeLimit]
        public IActionResult EditTrailer(TrailerViewModel trailer)
        {
            if (ModelState.IsValid && trailer.Id != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        truckAndTrailerService.EditTrailer(trailer);

                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/trailers");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/trailers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("SaveFile")]
        public string AddFile(IFormFile uploadedFile, string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
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
                else
                {
                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKeyTaxi))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKeyTaxi);
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
        public async Task<IActionResult> GoToViewTruckDoc(string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.TruckDoc = await truckAndTrailerService.GetTruckDoc(id);
                    ViewBag.TruckId = id;
                    
                    return View("DocTruck");
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

        [Route("Trailer/Doc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTraileDoc(string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.TrailerDoc = await truckAndTrailerService.GetTrailerDoc(id);
                    ViewBag.TrailerId = id;
                    
                    return View("DocTrailer");
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
        
        [Route("Truck/SaveDoc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult SaveDocTruck(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (!string.IsNullOrEmpty(nameDoc) && !string.IsNullOrEmpty(id) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        truckAndTrailerService.SaveDocTruck(uploadedFile, nameDoc, id);
                        
                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc?id={id}");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Trailer/DocSaveById")]
        public IActionResult Trailer(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (!string.IsNullOrEmpty(nameDoc) && !string.IsNullOrEmpty(id) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        truckAndTrailerService.SaveDocTrailer(uploadedFile, nameDoc, id);

                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Truck/DocSaveById")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult DocSaveById(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (!string.IsNullOrEmpty(nameDoc) && !string.IsNullOrEmpty(id) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        truckAndTrailerService.SaveDocTruck(uploadedFile, nameDoc, id);

                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc/?id={id}");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks/");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/SaveDoc")]
        public IActionResult SaveDocTrailer(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (!string.IsNullOrEmpty(nameDoc) && !string.IsNullOrEmpty(id) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                    {
                        truckAndTrailerService.SaveDocTrailer(uploadedFile, nameDoc, id);
                        
                        return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("RemoveDoc")]
        public IActionResult RemoveDoc(string idDock, string id, string type)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Equipment, out var key, out var idCompany))
                {
                    orderService.RemoveDoc(idDock);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/{type}/Doc?id={id}");
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

        [Route("GetDock")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDock(string docPath, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, type);
        }

        [Route("GetDockPDF")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDockPDF(string docPath)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, DocAndFileConstants.ContentTypePdf);
        }

        [HttpGet]
        [Route("Image")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetShiping(string name, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(name);
            
            return File(imageFileStream, $"image/{type}");
        }
    }
}