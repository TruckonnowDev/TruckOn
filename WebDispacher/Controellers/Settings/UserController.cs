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
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Setings/User"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    ViewBag.Users = companyService.GetUsers(Convert.ToInt32(idCompany));
                    
                    return View("~/Views/Settings/CompanyUsers.cshtml");
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

        [HttpGet]
        [Route("CreateUser")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Setings/User"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    
                    return View("~/Views/Settings/CreateUser.cshtml");
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

        [HttpPost]
        [Route("CreateUser")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers(string login, string password)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Setings/User"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    userService.AddUser(idCompany, login, password);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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

        [HttpGet]
        [Route("Remove")]
        public IActionResult CreateUsers(string idUser)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Setings/User"))
                {
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.NameCompany = companyName;
                    userService.RemoveUserById(idUser);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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