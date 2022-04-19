using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    [Route("Geolcation")]
    public class GeolcationController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly IUserService userService;
        private readonly IDriverService driverService;
        private readonly ICompanyService companyService;

        public GeolcationController(
            IUserService userService,
            IDriverService driverService,
            ICompanyService companyService)
        {
            this.companyService = companyService;
            this.driverService = driverService;
            this.userService = userService;
        }

        [Route("Map")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GeolocationPageGet()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Geolcation"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Drivers = await driverService.GetDrivers(idCompany);
                    actionResult = View("MapsGeoDriver");
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
            catch (Exception)
            {

            }
            return actionResult;
        }
    }
}