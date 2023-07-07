using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    [Route("Geolcation")]
    public class GeolcationController : BaseController
    {
        private readonly IDriverService driverService;
        private readonly ICompanyService companyService;

        public GeolcationController(
            IUserService userService,
            IDriverService driverService,
            ICompanyService companyService) : base(userService)
        {
            this.companyService = companyService;
            this.driverService = driverService;
        }

        [Route("Map")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GeolocationPageGet()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Geolocation, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Drivers = await driverService.GetDrivers(idCompany);
                    
                    return View("MapsGeoDriver");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            return Redirect(Config.BaseReqvesteUrl);;
        }
    }
}