using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
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

        [Route("Get")]
        public IActionResult Get(int idTr, int idProfile, string typeTransport)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar]  =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewBag.SelectSetingTruck = 
                        driverService.GetSelectSetingTruck(idCompany, idProfile, idTr, typeTransport);
                    
                    ViewBag.SetingsTruck = driverService.GetSetingsTruck(idCompany, idProfile, idTr, typeTransport);
                    ViewBag.IdProfile = idProfile;
                    ViewBag.IdTr = idTr;
                    ViewBag.TypeTransport = typeTransport;
                    ViewBag.Pattern = typeTransport+"Pattern";
                    
                    return View("~/Views/Settings/TrSettings.cshtml");
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
        [Route("Trucks")]
        public async Task<IActionResult> GetTrucks()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewBag.Trucks = await truckAndTrailerService.GetTrucks(0, idCompany);
                    
                    return View("~/Views/Settings/Trucks.cshtml");
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
        [Route("Trailers")]
        public async Task<IActionResult> GetTrailers()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsUser, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewBag.Trailers = await truckAndTrailerService.GetTrailers(0, idCompany);
                    
                    return View("~/Views/Settings/Trailers.cshtml");
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
        [Route("AddProfile")]
        public IActionResult AddProfile(int idTr, string typeTransport)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    var id = truckAndTrailerService.AddProfile(idCompany, idTr, typeTransport);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings?idTr={idTr}&page=s&idProfile={id}&typeTransport={typeTransport}");
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
        [Route("RemoveProfile")]
        public IActionResult RemoveProfile(int idProfile, string typeTransport, int idTr)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    driverService.RemoveProfile(idCompany, idProfile);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings?idTr={idTr}&idProfile={0}&typeTransport={typeTransport}");
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

        [HttpPost]
        [Route("SelectLayout")]
        public string SelectLayout(int idLayout)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    driverService.SelectLayout(idLayout);
                    
                    return string.Empty;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("SelectProfile")]
        public string SelectProfile(int idProfile, string typeTransport, int idTr)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    driverService.SelectProfile(idProfile, typeTransport, idTr, idCompany);
                    
                    return string.Empty;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("UnSelectLayout")]
        public string UnSelectLayout(int idLayout)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    ViewBag.NameCompany = GetCookieCompanyName();
                    driverService.UnSelectLayout(idLayout);
                    
                    return string.Empty;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("LayoutUP")]
        public string LayoutUP(int idLayout, int idTransported)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    driverService.LayoutUp(idLayout, idTransported);
                    
                    return string.Empty;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("LayoutDown")]
        public string LayoutDown(int idLayout, int idTransported)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Settings, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    driverService.LayoutDown(idLayout, idTransported);
                    
                    return string.Empty;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
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