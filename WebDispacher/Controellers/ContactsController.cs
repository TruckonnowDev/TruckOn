using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Contacts(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var contacts = await companyService.GetContactsByCompanyId(page, CompanyId);

                var countPages = await companyService.GetCountContactsPages(CompanyId);

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View("FullContacts", contacts);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }


        [HttpGet]
        [Route("Contact/CreateContact")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateContact()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                return View("CreateContact");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Contact/CreateContact")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CreateContact(ContactViewModel model, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await companyService.CreateContact(model, CompanyId, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult EditContact(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var contact = companyService.GetContact(id);
                    
                return View("EditContact", contact);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Contact/Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditContact(ContactViewModel model, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    //ViewBag.NameCompany = GetCookieCompanyName();

                    await companyService.EditContact(model, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("EditContact", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Contact/Delete")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                await companyService.DeleteContactById(id);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Contact/Contacts");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Contact/Delete")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> RemoveContact(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                await companyService.DeleteContactById(id);

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }
    }
}