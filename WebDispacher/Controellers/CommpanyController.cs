using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    [Route("Company")]
    public class CommpanyController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly IUserService userService;
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;

        public CommpanyController(
            IUserService userService,
            IDriverService driverService,
            ICompanyService companyService)
        {
            this.driverService = driverService;
            this.companyService = companyService;
            this.userService = userService;
        }

        [HttpGet]
        [Route("Companies")]
        public IActionResult GetCompanies()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Companies = companyService.GetCompaniesDTO();
                    actionResult = View("Companies");
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
        [Route("CreateCompany")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateCompany()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    //ViewBag.Subscriptions = companyService.GetSubscriptions();
                    actionResult = View("CreateCommpany");
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
        [Route("CreateCompany")]
        public IActionResult CreateCompany(string nameCommpany, string emailCommpany, List<IFormFile> MCNumberConfirmation, IFormFile IFTA, IFormFile KYU,
            IFormFile logbookPapers, IFormFile COI, IFormFile permits)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    companyService.AddCompany(nameCommpany, emailCommpany, MCNumberConfirmation[0], IFTA, KYU, logbookPapers, COI, permits);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
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

        [Route("Remove")]
        public IActionResult RemoveCompany(string id)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    driverService.RemoveCompany(id);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
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
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Doc")]
        public async Task<IActionResult> GoToViewCompanykDoc(string id)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.CompanyDoc = await companyService.GetCompanyDoc(id);
                    ViewBag.CompanyId = id;
                    actionResult = View("CompanyDocuments");
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

        [Route("SaveDoc")]
        public void SaveDoc(IFormFile uploadedFile, string nameDoc, string id)
        {
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    companyService.SaveDocCompany(uploadedFile, nameDoc, id);
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        [Route("RemoveDoc")]
        public IActionResult RemoveDoc(string idDock, string id)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    companyService.RemoveDocCompany(idDock);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Company/Doc?id={id}");
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

        [Route("GetDock")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDock(string docPath, string type)
        {
            IActionResult actionResult = null;
            var imageFileStream = System.IO.File.OpenRead(docPath);
            actionResult = File(imageFileStream, type);
            return actionResult;
        }

        [HttpGet]
        [Route("Users")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetUsers(int idCompanySelect)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Company"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Users = companyService.GetUsers(idCompanySelect);
                    ViewBag.Companies = companyService.GetCompaniesDTO();
                    ViewBag.IdCompanySelect = idCompanySelect;
                    actionResult = View("AllUsers");
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

        [Route("GetDockPDF")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDockPDF(string docPath)
        {
            IActionResult actionResult = null;
            var imageFileStream = System.IO.File.OpenRead(docPath);
            actionResult = File(imageFileStream, "application/pdf");
            return actionResult;
        }
    }
}