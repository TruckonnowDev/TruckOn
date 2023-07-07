using DaoModels.DAO.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Contact;

namespace WebDispacher.Controellers
{
    public class ContactsController : BaseController
    {
        private readonly ICompanyService companyService;

        public ContactsController(
            IUserService userService,
            ICompanyService companyService) : base(userService)
        {
            this.companyService = companyService;
        }

        [Route("Contact/Contacts")]
        public async Task<IActionResult> Contacts(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Contact, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    ViewBag.Contacts = await companyService.GetContactsViewModels(page, idCompany);

                    var countPages = await companyService.GetCountContactsPages(idCompany);

                    ViewBag.CountPages = countPages;

                    ViewBag.SelectedPage = page;

                    return View("FullContacts");
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


        [HttpGet]
        [Route("Contact/CreateContact")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateContact()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Contact, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    return View("CreateContact");
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
        [Route("Contact/CreateContact")]
        public IActionResult CreateDriver(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Contact, out var key, out var idCompany))
                    {
                        companyService.CreateContact(model, idCompany);

                        return Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
                    }

                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("CreateContact", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Contact/Edit")]
        public IActionResult EditContact(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Contact, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    var contact = companyService.GetContact(id);
                    
                    return View("EditContact", contact);
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
        [Route("Contact/Edit")]
        public IActionResult EditContact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Contact, out var key, out var idCompany))
                    {
                        ViewBag.NameCompany = GetCookieCompanyName();
                        ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                        companyService.EditContact(contact);

                        return Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
                    }

                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Contact/Delete")]
        public IActionResult DeleteContact(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Contact, out var key, out var idCompany))
                {
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    companyService.DeleteContactById(id);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
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
        [Route("Contact/Delete")]
        public async Task<bool> RemoveContact(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Contact, out var key, out var idCompany))
                {
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    await companyService.DeleteContactById(id);

                    return true;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
    }
}