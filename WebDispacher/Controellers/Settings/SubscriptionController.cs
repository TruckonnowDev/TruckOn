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
        ManagerDispatch managerDispatch = new ManagerDispatch();
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
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Subscription"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, isCancelSubscribe ? "Cancel" : "Settings");
                    ViewData["TextErrorSub"] = errorText;
                    ViewBag.Subscription = companyService.GetSubscription(idCompany);
                    actionResult = View("~/Views/Settings/Subscription/Subscription.cshtml");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [Route("All")]
        public IActionResult GetSubscriptions()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Subscription"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.Subscriptions = companyService.GetSubscriptions();
                    actionResult = View("~/Views/Settings/Subscription/AllSubscriptions.cshtml");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [Route("Select")]
        public IActionResult SelectSubscriptions(string idPrice, string priodDays)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Subscription"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    string errorText = companyService.SelectSub(idPrice, idCompany, priodDays);
                    actionResult = Redirect($"~/Settings/Subscription/Subscriptions?errorText={errorText.Replace("customer", "Company")}");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [Route("CancelNext")]
        public IActionResult CancelSubscriptionsNext()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Subscription"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    companyService.CancelSubscriptionsNext(idCompany);
                    actionResult = Redirect("~/Settings/Subscription/Subscriptions");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }
    }
}
