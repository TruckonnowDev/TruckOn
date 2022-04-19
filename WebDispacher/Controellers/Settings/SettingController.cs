using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings")]
    public class SettingController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
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
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"]  = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    ViewBag.SelectSetingTruck = driverService.GetSelectSetingTruck(idCompany, idProfile, idTr, typeTransport);
                    ViewBag.SetingsTruck = driverService.GetSetingsTruck(idCompany, idProfile, idTr, typeTransport);
                    ViewBag.IdProfile = idProfile;
                    ViewBag.IdTr = idTr;
                    ViewBag.TypeTransport = typeTransport;
                    ViewBag.Pattern = typeTransport+"Pattern";
                    actionResult = View("~/Views/Settings/TrSettings.cshtml");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpGet]
        [Route("Trucks")]
        public IActionResult GetTrucks()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    ViewBag.Trucks = truckAndTrailerService.GetTrucks(idCompany);
                    actionResult = View("~/Views/Settings/Trucks.cshtml");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpGet]
        [Route("Trailers")]
        public IActionResult GetTrailers()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    ViewBag.Trailers = truckAndTrailerService.GetTrailers(idCompany);
                    actionResult = View("~/Views/Settings/Trailers.cshtml");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpGet]
        [Route("AddProfile")]
        public IActionResult AddProfile(int idTr, string typeTransport)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    int id = truckAndTrailerService.AddProfile(idCompany, idTr, typeTransport);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings?idTr={idTr}&page=s&idProfile={id}&typeTransport={typeTransport}");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpGet]
        [Route("RemoveProfile")]
        public IActionResult RemoveProfile(int idProfile, string typeTransport, int idTr)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    driverService.RemoveProfile(idCompany, idProfile);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings?idTr={idTr}&idProfile={0}&typeTransport={typeTransport}");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("SelectLayout")]
        public string SelectLayout(int idLayout)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    driverService.SelectLayout(idLayout);
                    actionResult = "";
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = null;
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("SelectProfile")]
        public string SelectProfile(int idProfile, string typeTransport, int idTr)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    driverService.SelectProfile(idProfile, typeTransport, idTr, idCompany);
                    actionResult = "";
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = null;
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("UnSelectLayout")]
        public string UnSelectLayout(int idLayout)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    driverService.UnSelectLayout(idLayout);
                    actionResult = "";
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = null;
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("LayoutUP")]
        public string LayoutUP(int idLayout, int idTransported)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    driverService.LayoutUp(idLayout, idTransported);
                    actionResult = "";
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = null;
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("LayoutDown")]
        public string LayoutDown(int idLayout, int idTransported)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    driverService.LayoutDown(idLayout, idTransported);
                    actionResult = "";
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = null;
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
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