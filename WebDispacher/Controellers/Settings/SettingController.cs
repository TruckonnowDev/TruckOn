using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings")]
    public class SettingController : Controller
    {
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly IUserService userService;
        private readonly IDriverService driverService;
        private readonly ICompanyService companyService;

        public SettingController(
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            ICompanyService companyService,
            IDriverService driverService)
        {
            this.companyService = companyService;
            this.driverService = driverService;
            this.userService = userService;
            this.truckAndTrailerService = truckAndTrailerService;
        }
        
        public IActionResult Get(int idTr, int idProfile, string typeTransport)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar]  =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
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
        public IActionResult GetTrucks()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
                    ViewBag.Trucks = truckAndTrailerService.GetTrucks(idCompany);
                    
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
        public IActionResult GetTrailers()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    ViewBag.NameCompany = companyName;
                    ViewBag.Trailers = truckAndTrailerService.GetTrailers(idCompany);
                    
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    ViewBag.NameCompany = companyName;
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Settings))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
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