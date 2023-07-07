using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
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
        public IActionResult GetSubscription(string errorText)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Subscription, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, isCancelSubscribe
                        ? NavConstants.TypeNavCancel : NavConstants.TypeNavSettings);
                    
                    ViewData[NavConstants.TextErrorSub] = errorText;
                    ViewBag.Subscription = companyService.GetSubscription(idCompany);
                    
                    return View("~/Views/Settings/Subscription/Subscription.cshtml");
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

        [Route("All")]
        public IActionResult GetSubscriptions()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Subscription, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.Subscriptions = companyService.GetSubscriptions();
                    
                    return View("~/Views/Settings/Subscription/AllSubscriptions.cshtml");
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

        [Route("Select")]
        public IActionResult SelectSubscriptions(string idPrice, string priodDays)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Subscription, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    var errorText = companyService.SelectSub(idPrice, idCompany, priodDays);
                    
                    return Redirect($"~/Settings/Subscription/Subscriptions?errorText={errorText.Replace("customer", "Company")}");
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

        [Route("CancelNext")]
        public IActionResult CancelSubscriptionsNext()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Subscription, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    companyService.CancelSubscriptionsNext(idCompany);
                    
                    return Redirect("~/Settings/Subscription/Subscriptions");
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
