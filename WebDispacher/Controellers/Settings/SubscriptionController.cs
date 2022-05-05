using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Models.Subscription;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings/Subscription")]
    public class SubscriptionController : Controller
    {
        private readonly IUserService userService;
        private readonly ICompanyService companyService;

        public SubscriptionController(
            IUserService userService,
            ICompanyService companyService)
        {
            this.companyService = companyService;
            this.userService = userService;
        }

        [Route("Subscriptions")]
        public IActionResult GetSubscription(string errorText)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Subscription"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, isCancelSubscribe ? "Cancel" : "Settings");
                    ViewData["TextErrorSub"] = errorText;
                    ViewBag.Subscription = companyService.GetSubscription(idCompany);
                    
                    return View("~/Views/Settings/Subscription/Subscription.cshtml");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
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
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Subscription"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.Subscriptions = companyService.GetSubscriptions();
                    
                    return View("~/Views/Settings/Subscription/AllSubscriptions.cshtml");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
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
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Subscription"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    var errorText = companyService.SelectSub(idPrice, idCompany, priodDays);
                    
                    return Redirect($"~/Settings/Subscription/Subscriptions?errorText={errorText.Replace("customer", "Company")}");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
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
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Subscription"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    companyService.CancelSubscriptionsNext(idCompany);
                    
                    return Redirect("~/Settings/Subscription/Subscriptions");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
