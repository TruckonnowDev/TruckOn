using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Models.Subscription;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings/Subscription")]
    public class SubscriptionController : BaseController
    {
        private readonly ICompanyService companyService;

        public SubscriptionController(
            IUserService userService,
            ICompanyService companyService) : base(userService)
        {
            this.companyService = companyService;
        }

        [Route("Subscriptions")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult GetSubscription(string errorText)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, isCancelSubscribe
                    ? NavConstants.TypeNavCancel : NavConstants.TypeNavSettings);
                    
                ViewData[NavConstants.TextErrorSub] = errorText;
                ViewBag.Subscription = companyService.GetSubscription(CompanyId);
                    
                return View("~/Views/Settings/Subscription/Subscription.cshtml");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("All")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult GetSubscriptions()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                ViewBag.Subscriptions = companyService.GetSubscriptions();
                    
                return View("~/Views/Settings/Subscription/AllSubscriptions.cshtml");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Select")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SelectSubscriptions(string idPrice, string priodDays)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                var errorText = await companyService.SelectSub(idPrice, CompanyId, priodDays);
                    
                return Redirect($"~/Settings/Subscription/Subscriptions?errorText={errorText.Replace("customer", "Company")}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("CancelNext")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CancelSubscriptionsNext()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] =
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                companyService.CancelSubscriptionsNext(CompanyId);
                    
                return Redirect("~/Settings/Subscription/Subscriptions");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
