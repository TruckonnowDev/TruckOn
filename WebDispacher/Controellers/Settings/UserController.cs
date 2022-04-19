using System;
using System.Collections.Generic;
using System.Linq;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings/User")]
    public class UserController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly IUserService userService;
        private readonly ICompanyService companyService;

        public UserController(
            IUserService userService,
            ICompanyService companyService)
        {
            this.companyService = companyService;
            this.userService = userService;
        }

        [Route("Users")]
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/User"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    ViewBag.Users = companyService.GetUsers(Convert.ToInt32(idCompany));
                    actionResult = View("~/Views/Settings/CompanyUsers.cshtml");
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
        [Route("CreateUser")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/User"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    actionResult = View("~/Views/Settings/CreateUser.cshtml");
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
        [Route("CreateUser")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers(string login, string password)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/User"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    userService.AddUser(idCompany, login, password);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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
        [Route("Remove")]
        public IActionResult CreateUsers(string idUser)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Setings/User"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    userService.RemoveUserById(idUser);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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