using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Company;
using WebDispacher.ViewModels.Contact;

namespace WebDispacher.Controellers
{
    [Route("Company")]
    public class CommpanyController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;

        public CommpanyController(
            IUserService userService,
            IDriverService driverService,
            ICompanyService companyService) : base(userService)
        {
            this.driverService = driverService;
            this.companyService = companyService;
        }

        [HttpGet]
        [Route("Companies")]
        public async Task<IActionResult> GetCompanies(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    ViewBag.Companies = await companyService.GetCompaniesViewModels(page);

                    var countPages = await companyService.GetCountCompaniesPages();

                    ViewBag.CountPages = countPages;

                    ViewBag.SelectedPage = page;

                    return View("Companies");
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
        [Route("SendEmail")]
        public async Task<bool> SendEmail(string companyId, string subject, string message, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                {
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    var result = await companyService.SendEmailToUserByCompanyId(companyId, subject, message);

                    return result;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception e)
            {

            }

            return false;
        }

        [HttpGet]
        [Route("CreateCompany")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateCompany()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    //ViewBag.Subscriptions = companyService.GetSubscriptions();
                    
                    return View("CreateCommpany");
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
        [Route("CreateCompany")]
        public async Task<IActionResult> CreateCompany(CreateCompanyViewModel company, List<IFormFile> MCNumberConfirmation, IFormFile IFTA, IFormFile KYU,
            IFormFile logbookPapers, IFormFile COI, IFormFile permits, string dateTimeLocal)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                    {
                        await companyService.AddCompany(company, MCNumberConfirmation.Count != 0 ? MCNumberConfirmation[0] : null, IFTA, KYU, logbookPapers, COI,
                            permits, dateTimeLocal);

                        return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("LoginUser")]
        public async Task<IActionResult> LoginUser(string companyId)
        {
            var userInfo = await companyService.GetUserByCompanyId(companyId);

            if (userInfo == null) return null;

            try
            {
                if (CheckPermissionsByCookies(RouteConstants.Company, out var _, out var _))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    Response.Cookies.Delete(CookiesKeysConstants.CompanyIdKey);
                    Response.Cookies.Delete(CookiesKeysConstants.CompanyNameKey);

                    var key = userService.CreateKey(userInfo.Login, userInfo.Password);
                    var company = userService.GetUserByKeyUser(key);

                    Response.Cookies.Append(CookiesKeysConstants.CarKey, key.ToString());
                    Response.Cookies.Append(CookiesKeysConstants.CompanyIdKey, company.Id.ToString());
                    Response.Cookies.Append(CookiesKeysConstants.CompanyNameKey, company.Name);

                    var companyType = companyService.GetTypeNavBar(key.ToString(), company.Id.ToString());

                    return Redirect(Config.BaseReqvesteUrl);
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch(Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditCompany")]
        public async Task<IActionResult> EditCompany(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);

                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }

                    ViewBag.NameCompany = GetCookieCompanyName();

                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    var companyViewModel = await companyService.GetCompanyById(id);

                    return View(companyViewModel);
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
        [Route("EditCompany")]
        public async Task<IActionResult> EditCompany(CompanyViewModel company)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                    {
                        ViewBag.NameCompany = GetCookieCompanyName();
                        ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                        await companyService.EditCompany(company);

                        return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
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
                return View(company);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Remove")]
        public IActionResult RemoveCompany(string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var _, out var _))
                {
                    driverService.RemoveCompany(id);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
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
        
        [Route("Activate")]
        public async Task<IActionResult> ActivateCompany(string companyId)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var _, out var _))
                {
                    await companyService.ActivateCompany(companyId);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
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
        [Route("Remove")]
        public async Task<bool> DeleteCompany(string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var _, out var _))
                {
                    await driverService.RemoveCompany(id);

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

        [Route("Doc")]
        public async Task<IActionResult> GoToViewCompanykDoc(string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.CompanyDoc = await companyService.GetCompanyDoc(id);
                    ViewBag.CompanyId = id;
                    
                    return View("CompanyDocuments");
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

        [Route("SaveDoc")]
        public IActionResult SaveDoc(IFormFile uploadedFile, string nameDoc, string id)
        {
            if (!string.IsNullOrEmpty(nameDoc) && !string.IsNullOrEmpty(id) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (CheckPermissionsByCookies(RouteConstants.Company, out var _, out var _))
                    {
                        companyService.SaveDocCompany(uploadedFile, nameDoc, id);

                        return Redirect($"{Config.BaseReqvesteUrl}/Company/Doc?id={id}");
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
                return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("RemoveDoc")]
        public IActionResult RemoveDoc(string idDock, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var _, out var _))
                {
                    companyService.RemoveDocCompany(idDock);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Company/Doc?id={id}");
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

        [Route("GetDock")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDock(string docPath, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, type);
        }

        [HttpGet]
        [Route("Users")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GetUsers(int idCompanySelect)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Users = companyService.GetUsers(idCompanySelect);
                    ViewBag.Companies = await companyService.GetCompaniesDTO(0);
                    ViewBag.IdCompanySelect = idCompanySelect;
                    
                    return View("AllUsers");
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
        [Route("Questions")]
        public async Task<IActionResult> GetQuestions()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (CheckPermissionsByCookies(RouteConstants.Company, out var _, out var idCompany))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    return View("UserQuestions");
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
        [Route("Company")]
        public async Task<IActionResult> CompanyInfo(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (CheckPermissionsByCookies(RouteConstants.Company, out var key, out var idCompany))
                {
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    var companyInfo = await companyService.GetCompanyById(id);

                    return View(companyInfo);
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

        [Route("GetDockPDF")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDockPDF(string docPath)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, DocAndFileConstants.ContentTypePdf);
        }
    }
}