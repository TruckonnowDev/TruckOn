using DaoModels.DAO.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Dispatcher;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings/Extension")]
    public class ExtensionController : BaseController
    {
        private readonly ICompanyService companyService;

        public ExtensionController(
            IUserService userService,
            ICompanyService companyService) : base(userService)
        {
            this.companyService = companyService;
        }

        [Route("Dispatchs")]
        public IActionResult GetUsers()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsExtension, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }

                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    var dispatchers = companyService.GetDispatchers(Convert.ToInt32(idCompany));
                    ViewBag.Dispatchers = dispatchers;
                    
                    return View("~/Views/Settings/Extension/AllDispatch.cshtml");
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
        [Route("CreateDispatch")]
        public IActionResult AddDicpatch()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsExtension, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }

                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    
                    return View("~/Views/Settings/Extension/AddDispatch.cshtml");
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
        [Route("RefreshToken")]
        public string RefreshTokenDispatch(string idDispatch)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsExtension, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    
                    return companyService.RefreshTokenDispatch(idDispatch);
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {
                
            }

            return null;
        }

        [HttpPost]
        [Route("CreateDispatch")]
        public IActionResult AddDispatch(DispatcherViewModel dispatcher)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsExtension, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    companyService.CreateDispatch(dispatcher, Convert.ToInt32(idCompany));
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
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
        [Route("EditDispatch")]
        public IActionResult EditDispatch(int idDispatch)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsExtension, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }

                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    
                    var dispatcher = companyService.GetDispatcherById(idDispatch);

                    return View("~/Views/Settings/Extension/EditDispatch.cshtml", dispatcher);
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
        [Route("EditDispatch")]
        public IActionResult EditDispatch(DispatcherViewModel dispatcher)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (CheckPermissionsByCookies(RouteConstants.SettingsExtension, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    companyService.EditDispatch(dispatcher);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
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
        [Route("RemoveDispatch")]
        public IActionResult RemoveDispatch(int idDispatch)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.SettingsExtension, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    companyService.RemoveDispatchById(idDispatch);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
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