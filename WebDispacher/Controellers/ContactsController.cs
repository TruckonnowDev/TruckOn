using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class ContactsController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly IUserService userService;
        private readonly ICompanyService companyService;

        public ContactsController(
            IUserService userService,
            ICompanyService companyService)
        {
            this.companyService = companyService;
            this.userService = userService;
        }

        [Route("Contact/Contacts")]
        public IActionResult Contacts(int page)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Contact"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Contacts = companyService.GetContacts(idCompany);
                    actionResult = View("FullContacts");
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
            catch (Exception)
            {

            }
            return actionResult;
        }


        [HttpGet]
        [Route("Contact/CreateContact")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateContact()
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Contact"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    actionResult = View("CreateContact");
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
            catch (Exception)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("Contact/CreateContact")]
        public IActionResult CreateDriver(string fullName, string emailAddress, string phoneNumbe)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Contact"))
                {
                    if ((fullName != null && fullName != "") && (emailAddress != null && emailAddress != "") && (emailAddress != null && emailAddress != "")
                       && (fullName != null && fullName != ""))
                    {
                        companyService.CreateContact(fullName, emailAddress, phoneNumbe, idCompany);
                        actionResult = Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
                    }
                    else
                    {
                        actionResult = View("CreateContact");
                    }
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
            catch (Exception)
            {

            }
            return actionResult;
        }

        [HttpGet]
        [Route("Contact/Edit")]
        public IActionResult EditeContact(int id)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Contact"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Contact = companyService.GetContact(id);
                    actionResult = View("EditContact");
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
            catch (Exception)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("Contact/Edit")]
        public IActionResult EditeContact(int id, string fullName, string emailAddress, string phoneNumbe)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Contact"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    companyService.EditContact(id, fullName, emailAddress, phoneNumbe);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
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
            catch (Exception)
            {

            }
            return actionResult;
        }

        [HttpGet]
        [Route("Contact/Delete")]
        public IActionResult DeleteContact(int id)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Contact"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    companyService.DeleteContactById(id);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
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
            catch (Exception)
            {

            }
            return actionResult;
        }
    }
}