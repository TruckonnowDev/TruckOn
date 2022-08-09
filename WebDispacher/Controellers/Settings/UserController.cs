﻿using System;
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.SettingsUser))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
                    ViewBag.Users = companyService.GetUsers(Convert.ToInt32(idCompany));
                    
                    return View("~/Views/Settings/CompanyUsers.cshtml");
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.SettingsUser))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
                    
                    return View("~/Views/Settings/CreateUser.cshtml");
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
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);

                    if (userService.CheckPermissions(key, idCompany, RouteConstants.SettingsUser))
                    {
                        ViewData[NavConstants.TypeNavBar] =
                            companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);

                        ViewBag.NameCompany = companyName;
                        userService.AddUser(idCompany, user);

                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.SettingsUser))
                {
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    ViewBag.NameCompany = companyName;
                    userService.RemoveUserById(idUser);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.SettingsUser))
                {
                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = companyName;
                    var user = userService.GetUserById(id);
                    
                    return View("~/Views/Settings/EditUser.cshtml", user);
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
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);

                    if (userService.CheckPermissions(key, idCompany, RouteConstants.SettingsUser))
                    {
                        ViewData[NavConstants.TypeNavBar] =
                            companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                        ViewBag.NameCompany = companyName;
                        userService.EditUser(user);

                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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