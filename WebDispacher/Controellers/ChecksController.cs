
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Driver;

namespace WebDispacher.Controellers
{
    public class ChecksController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;
        public ChecksController(
            IUserService userService,
            ICompanyService companyService,
            IDriverService driverService)
            : base(userService)
        {
            this.companyService = companyService;
            this.driverService = driverService;
        }
        
        [HttpGet]
        public async Task<IActionResult> DriverCheck(DriverSearchViewModel model)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                if (CheckPermissionsByCookies(RouteConstants.DriverCheck, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);

                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }

                  var removedDrivers = await driverService.GetDriverReportsByCompnayId(model, idCompany);

                   ViewBag.Drivers = removedDrivers;
                    return View("DriverCheck", model);
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
        public IActionResult CarrierCheck()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                if (CheckPermissionsByCookies(RouteConstants.CarrierCheck, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);

                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    return View("CarrierCheck");
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
        public IActionResult BrokerCheck()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                if (CheckPermissionsByCookies(RouteConstants.BrokerCheck, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);

                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }

                    return View("BrokerCheck");
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
    }
}
