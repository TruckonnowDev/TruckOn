using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GeolocationPageGet()
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
                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                    
                return View("MapsGeoDriver", drivers);
            }
            catch (Exception)
            {

            }
            return Redirect(Config.BaseReqvesteUrl);;
        }
    }
}