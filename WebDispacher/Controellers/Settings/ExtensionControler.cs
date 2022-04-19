using DaoModels.DAO.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings/Extension")]
    public class ExtensionControler : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly IUserService userService;
        private readonly ICompanyService companyService;

        public ExtensionControler(
            IUserService userService,
            ICompanyService companyService)
        {
            this.userService = userService;
            this.companyService = companyService;
        }

        [Route("Dicpatchs")]
        public IActionResult GetUsers()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/Extension"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    List<Dispatcher> dispatchers = companyService.GetDispatchers(Convert.ToInt32(idCompany));
                    ViewBag.Dispatchers = dispatchers;
                    actionResult = View("~/Views/Settings/Extension/AllDispatch.cshtml");
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

        [HttpGet]
        [Route("CreateDispatch")]
        public IActionResult AddDicpatch()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/Extension"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    actionResult = View("~/Views/Settings/Extension/AddDispatch.cshtml");
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

        [HttpPost]
        [Route("RefreshToken")]
        public string RefreshTokenDispatch(string idDispatch)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/Extension"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    actionResult = companyService.RefreshTokenDispatch(idDispatch);
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                }
            }
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [HttpPost]
        [Route("CreateDispatch")]
        public IActionResult AddDicpatch(string typeDispatcher, string login, string password)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/Extension"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    companyService.CreateDispatch(typeDispatcher, login, password, Convert.ToInt32(idCompany));
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dicpatchs");
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

        [HttpGet]
        [Route("EditDicpatch")]
        public IActionResult EditDicpatch(int idDispatch)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/Extension"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    Dispatcher dispatcher = companyService.GetDispatcherById(idDispatch);
                    ViewBag.Dispatcher = dispatcher;
                    actionResult = View("~/Views/Settings/Extension/EditDispatch.cshtml");
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

        [HttpPost]
        [Route("EditDicpatch")]
        public IActionResult EditDicpatch(int idDispatch, string typeDispatcher, string login, string password)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/Extension"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    companyService.EditDispatch(idDispatch, typeDispatcher, login, password);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dicpatchs");
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

        [HttpGet]
        [Route("RemoveDispatch")]
        public IActionResult RemoveDispatch(int idDispatch)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/Extension"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    companyService.RemoveDispatchById(idDispatch);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dicpatchs");
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