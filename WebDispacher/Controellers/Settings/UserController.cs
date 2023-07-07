using System;
using System.Collections.Generic;
using System.Linq;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Settings;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings/User")]
    public class UserController : BaseController
    {
        private readonly ICompanyService companyService;

        public UserController(
            IUserService userService,
            ICompanyService companyService) : base(userService)
        {
            this.companyService = companyService;
        }

        [Route("Users")]
        public IActionResult GetUsers()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsUser, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    /*ViewBag.NameCompany = GetCookieCompanyName();
                    ViewBag.Users = companyService.GetUsers(Convert.ToInt32(idCompany));*/
                    
                    return Redirect(Config.BaseReqvesteUrl);
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

        [HttpGet]
        [Route("CreateUser")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsUser, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    /*ViewBag.NameCompany = GetCookieCompanyName();*/
                    
                    return Redirect(Config.BaseReqvesteUrl);
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

        [HttpPost]
        [Route("CreateUser")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers(SettingsUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.SettingsUser, out var key, out var idCompany))
                    {
                        ViewData[NavConstants.TypeNavBar] =
                            companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);

                        /*ViewBag.NameCompany = GetCookieCompanyName();
                        userService.AddUser(idCompany, user);
*/
                        return Redirect(Config.BaseReqvesteUrl);
                    }

                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsUser, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    /*ViewBag.NameCompany = GetCookieCompanyName();
                    userService.RemoveUserById(idUser);
                    */
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
        
        [HttpGet]
        [Route("Edit")]
        public IActionResult EditUser(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsUser, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    /*ViewBag.NameCompany = GetCookieCompanyName();
                    var user = userService.GetUserById(id);*/
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Edit")]
        public IActionResult EditUser(SettingsUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.SettingsUser, out var key, out var idCompany))
                    {
                        ViewData[NavConstants.TypeNavBar] =
                            companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                       /* ViewBag.NameCompany = GetCookieCompanyName();
                        userService.EditUser(user);

                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");*/
                    }

                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
                }
                catch (Exception e)
                {

                }
            }
            else
            { 
                return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}