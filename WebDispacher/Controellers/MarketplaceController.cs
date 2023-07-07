using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
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
        public IActionResult Index()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                if (CheckPermissionsByCookies(RouteConstants.Marketplace, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);

                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    return View();
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
