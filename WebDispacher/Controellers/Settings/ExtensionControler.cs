using DaoModels.DAO.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;
using WebDispacher.ViewModels.Dispatcher;

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

        [Route("Dispatchs")]
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
        public IActionResult AddDispatch(DispatcherViewModel dispatcher)
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
                    companyService.CreateDispatch(dispatcher, Convert.ToInt32(idCompany));
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
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
        [Route("EditDispatch")]
        public IActionResult EditDispatch(int idDispatch)
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
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }

                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    
                    var dispatcher = companyService.GetDispatcherById(idDispatch);

                    return View("~/Views/Settings/Extension/EditDispatch.cshtml", dispatcher);
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }

                actionResult = Redirect(Config.BaseReqvesteUrl);
            }
            catch (Exception e)
            {
            }

            return actionResult;
        }

        [HttpPost]
        [Route("EditDispatch")]
        //public IActionResult EditDicpatch(int idDispatch, string typeDispatcher, string login, string password)
        public IActionResult EditDispatch(DispatcherViewModel dispatcher)
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
                    companyService.EditDispatch(dispatcher);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
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
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
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