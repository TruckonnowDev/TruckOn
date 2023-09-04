using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult Dispatchs()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                var dispatchers = companyService.GetDispatchers(Convert.ToInt32(CompanyId));
                    
                return View("~/Views/Settings/Extension/AllDispatch.cshtml", dispatchers);
            }
            catch (Exception e)
            {
                
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("CreateDispatch")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult AddDispatch()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                    
                return View("~/Views/Settings/Extension/AddDispatch.cshtml");
            }
            catch (Exception e)
            {
                
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("RefreshToken")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string RefreshTokenDispatch(string idDispatch)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                    
                return companyService.RefreshTokenDispatch(idDispatch);
            }
            catch (Exception e)
            {
                
            }

            return null;
        }

        [HttpPost]
        [Route("CreateDispatch")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult AddDispatch(DispatcherViewModel dispatcher)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                if (ModelState.IsValid)
                {
                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);

                    //ViewBag.NameCompany = GetCookieCompanyName();

                    companyService.CreateDispatch(dispatcher, Convert.ToInt32(CompanyId));

                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
                }
                else
                {
                    return View("~/Views/Settings/Extension/AddDispatch.cshtml", dispatcher);
                }
            }
            catch (Exception e)
            {
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditDispatch")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult EditDispatch(int idDispatch)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                    
                var dispatcher = companyService.GetDispatcherById(idDispatch);

                return View("~/Views/Settings/Extension/EditDispatch.cshtml", dispatcher);
            }
            catch (Exception e)
            {
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("EditDispatch")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult EditDispatch(DispatcherViewModel dispatcher)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (ModelState.IsValid)
                {
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                    //ViewBag.NameCompany = GetCookieCompanyName();
                    companyService.EditDispatch(dispatcher);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
                }
                else
                {
                    return View("~/Views/Settings/Extension/EditDispatch.cshtml", dispatcher);
                }
            }
            catch (Exception e)
            {
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("RemoveDispatch")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult RemoveDispatch(int idDispatch)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                companyService.RemoveDispatchById(idDispatch);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Settings/Extension/Dispatchs");
            }
            catch (Exception e)
            {
                
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}