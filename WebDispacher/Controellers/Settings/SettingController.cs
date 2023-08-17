using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings")]
    public class SettingController : BaseController
    {
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly IDriverService driverService;
        private readonly ICompanyService companyService;

        public SettingController(
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            ICompanyService companyService,
            IDriverService driverService) : base(userService)
        {
            this.companyService = companyService;
            this.driverService = driverService;
            this.truckAndTrailerService = truckAndTrailerService;
        }

        [HttpGet]
        [Route("Get")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Get([FromQuery] int idTr, [FromQuery] int idProfile, [FromQuery] string typeTransport)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                ViewData[NavConstants.TypeNavBar]  =
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewBag.SelectSetingTruck = await
                    driverService.GetSelectSettingTruck(CompanyId, idProfile, idTr, typeTransport);
                    
                ViewBag.SetingsTruck = await driverService.GetSettingsTruck(CompanyId, idProfile, idTr, typeTransport);
                ViewBag.IdProfile = idProfile;
                ViewBag.IdTr = idTr;
                ViewBag.TypeTransport = typeTransport;
                ViewBag.Pattern = typeTransport+"Pattern";
                    
                return View("~/Views/Settings/TrSettings.cshtml");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Trucks")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrucks(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();

                var trucks = await truckAndTrailerService.GetTrucks(page, CompanyId);
                    
                return View("~/Views/Settings/Trucks.cshtml", trucks);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Trailers")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailers(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                //ViewBag.NameCompany = GetCookieCompanyName();
                var trailers = await truckAndTrailerService.GetTrailers(page, CompanyId);
                    
                return View("~/Views/Settings/Trailers.cshtml", trailers);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("AddProfile")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> AddProfile(int idTr, string typeTransport)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                var id = await truckAndTrailerService.AddProfile(CompanyId, idTr, typeTransport);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Settings/Get?idTr={idTr}&page=s&idProfile={id}&typeTransport={typeTransport}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("RemoveProfile")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveProfile(int idProfile, string typeTransport, int idTr)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                await driverService.RemoveProfile(CompanyId, idProfile);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Settings?idTr={idTr}&idProfile={0}&typeTransport={typeTransport}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("SelectLayout")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string SelectLayout(int idLayout)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                driverService.SelectLayout(idLayout);
                    
                return string.Empty;
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("SelectProfile")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string SelectProfile(int idProfile, string typeTransport, int idTr)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] =
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                driverService.SelectProfile(idProfile, typeTransport, idTr, CompanyId);
                    
                return string.Empty;
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("UnSelectLayout")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string UnSelectLayout(int idLayout)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                //ViewBag.NameCompany = GetCookieCompanyName();
                driverService.UnSelectLayout(idLayout);
                    
                return string.Empty;
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("LayoutUP")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string LayoutUP(int idLayout, int idTransported)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                driverService.LayoutUp(idLayout, idTransported);
                    
                return string.Empty;
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("LayoutDown")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string LayoutDown(int idLayout, int idTransported)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                driverService.LayoutDown(idLayout, idTransported);
                    
                return string.Empty;
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpGet]
        [Route("Image")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetShiping(string name, string type)
        {
            if(!name.Contains(type))
            {
                name += "." + type;
            }
            
            var imageFileStream = System.IO.File.OpenRead(name);
            
            return File(imageFileStream, $"image/{type}");
        }
    }
}