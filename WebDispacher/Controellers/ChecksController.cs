using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> DriverCheck(DriverSearchViewModel model)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var removedDrivers = await driverService.GetDriverReportsByCompnayId(model, CompanyId);

                ViewBag.Drivers = removedDrivers;
                return View("DriverCheck", model);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CarrierCheck()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                return View("CarrierCheck");
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult BrokerCheck()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                return View("BrokerCheck");
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
            
        }
    }
}
