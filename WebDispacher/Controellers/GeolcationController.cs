﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    [Route("Geolcation")]
    public class GeolcationController : Controller
    {
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
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                if (userService.CheckPermissions(key, idCompany, "Geolcation"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Drivers = await driverService.GetDrivers(idCompany);
                    
                    return View("MapsGeoDriver");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            return Redirect(Config.BaseReqvesteUrl);;
        }
    }
}