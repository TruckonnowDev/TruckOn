using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class MarketplaceController : BaseController
    {
        private readonly ICompanyService companyService;
        public MarketplaceController(
            IUserService userService, 
            ICompanyService companyService) 
            : base(userService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        [Route("Marketplace")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult Index()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
