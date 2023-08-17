using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult GetUsers()
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
                    
                /*ViewBag.NameCompany = GetCookieCompanyName();
                ViewBag.Users = companyService.GetUsers(Convert.ToInt32(idCompany));*/
                    
                return Redirect(Config.BaseReqvesteUrl);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("CreateUser")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers()
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
                    
                /*ViewBag.NameCompany = GetCookieCompanyName();*/
                    
                return Redirect(Config.BaseReqvesteUrl);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("CreateUser")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult CreateUsers(SettingsUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);

                    /*ViewBag.NameCompany = GetCookieCompanyName();
                    userService.AddUser(idCompany, user);*/

                    return Redirect(Config.BaseReqvesteUrl);
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CreateUsers(string idUser)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                /*ViewBag.NameCompany = GetCookieCompanyName();*/
                userService.RemoveUserById(idUser);
                
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult EditUser(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] =
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                /*ViewBag.NameCompany = GetCookieCompanyName();
                var user = userService.GetUserById(id);*/
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult EditUser(SettingsUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    /* ViewBag.NameCompany = GetCookieCompanyName();*/
                    userService.EditUser(user);

                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/User/Users");
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