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
using Microsoft.AspNetCore.Authorization;
using WebDispacher.Constants.Identity;
using Microsoft.AspNetCore.Identity;
using DaoModels.DAO.Enum;
using System.Security.Claims;

namespace WebDispacher.Controellers
{
    [Route("Company")]
    public class CommpanyController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public CommpanyController(
            IUserService userService,
            IDriverService driverService,
            ICompanyService companyService,
            UserManager<User> userManager,
            SignInManager<User> signInManager) : base(userService)
        {
            this.driverService = driverService;
            this.companyService = companyService;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("Companies")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> GetCompanies(int page = 1)
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

                var companies = await companyService.GetCompaniesWithUsers(page);

                var countPages = await companyService.GetCountCompaniesPages();

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View("Companies", companies);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("SendEmail")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<bool> SendEmail(string companyId, string subject, string message, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                //ViewBag.NameCompany = GetCookieCompanyName();
                
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var result = await companyService.SendEmailToUserByCompanyId(companyId, subject, message);

                return result;
            }
            catch (Exception e)
            {

            }

            return false;
        }

        [HttpGet]
        [Route("CreateCompany")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateCompany()
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

                //ViewBag.Subscriptions = companyService.GetSubscriptions();
                    
                return View("CreateCommpany");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("CreateCompany")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> CreateCompany(CreateCompanyViewModel model, List<IFormFile> MCNumberConfirmation, IFormFile IFTA, IFormFile KYU,
            IFormFile logbookPapers, IFormFile COI, IFormFile permits, string dateTimeLocal)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        DateTimeRegistration = DateTime.UtcNow,
                        DateTimeLastUpdate = DateTime.UtcNow,
                    };

                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, RolesIdentityConstants.UserRole);

                        var company = await companyService.CreateCompanyByAdminWithDocs(model, 
                            MCNumberConfirmation.Count != 0 ? MCNumberConfirmation[0] : null, IFTA, KYU, logbookPapers, COI,
                                permits, dateTimeLocal);

                        if (company.Id != 0 && user.Id != null)
                        {
                            await companyService.AddUserToCompany(user, company);

                            return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
                        }

                    }

                    foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> LoginUser(string companyId)
        {
            await signInManager.SignOutAsync();

            var userInfo = await companyService.GetUserByCompanyId(companyId);

            if (userInfo == null) return null;

            try
            {
                var user = await userManager.FindByNameAsync(userInfo.Email);

                if (user != null)
                {
                    var existingClaims = await userManager.GetClaimsAsync(user);

                    var userCompanyActive = await companyService.GetActiveUserCompanyByCompanyTypeUserId(user.Id, CompanyType.Carrier);

                    await userManager.RemoveClaimsAsync(user, existingClaims);

                    var customClaims = new List<Claim>();

                    if (await userManager.IsInRoleAsync(user, RolesIdentityConstants.AdminRole))
                    {
                        customClaims.Add(new Claim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyCarrierAdminValue));
                    }
                    else
                    {
                        customClaims.Add(new Claim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyCarrierValue));
                    }

                    if (userCompanyActive != null)
                    {
                        customClaims.Add(new Claim(ClaimsIdentityConstants.CompanyId, userCompanyActive.Id.ToString()));
                    }


                    await userManager.AddClaimsAsync(user, customClaims);

                    await signInManager.SignInAsync(user, isPersistent: false);

                    var b = User.Identity.IsAuthenticated;

                    return Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch(Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditCompany")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> EditCompany(int id)
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

                var companyViewModel = await companyService.GetCompanyById(id);

                return View(companyViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("EditCompany")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> EditCompany(CompanyViewModel model, string dateTimeLocal)
        {
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await userService.GetFirstUserByCompanyId(model.Id);

                    user.Email = model.Email.ToLower();
                    user.UserName = model.Email.ToLower();

                    var result = await userManager.UpdateAsync(user);

                    if(!result.Succeeded)
                        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                       
                    
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        await userManager.RemovePasswordAsync(user);
                        await userManager.AddPasswordAsync(user, model.Password);
                    }

                    //ViewBag.NameCompany = GetCookieCompanyName();


                    await companyService.EditCompany(model, dateTimeLocal);

                    return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View(model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> RemoveCompany(string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var result = await driverService.RemoveCompany(id);

                if (result)
                {
                    var userEmail = await userService.GetFirstUserEmailByCompanyId(Convert.ToInt32(id));

                    var user = await userManager.FindByEmailAsync(userEmail);

                    await userManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(100));
                }

                return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Activate")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> ActivateCompany(string companyId, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var result = await companyService.ActivateCompany(companyId);

                if (result)
                {
                    var userEmail = await userService.GetFirstUserEmailByCompanyId(Convert.ToInt32(companyId));

                    var user = await userManager.FindByEmailAsync(userEmail);

                    var companyActive = userService.GetCompanyById(companyId);

                    await userManager.SetLockoutEndDateAsync(user, null);

                    if(companyActive != null && await userManager.IsInRoleAsync(user, RolesIdentityConstants.AdminRole)) {
                        await companyService.UpdateCompanyStatus(companyActive.Id, CompanyStatus.Admin);
                    }
                }
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Company/Companies");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<bool> DeleteCompany(string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var result = await driverService.RemoveCompany(id);

                if (result)
                {
                    var userEmail = await userService.GetFirstUserEmailByCompanyId(Convert.ToInt32(id));

                    var user = await userManager.FindByEmailAsync(userEmail);

                    await userManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(100));
                    
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        [Route("Doc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> GoToViewCompanykDoc(string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
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

                var companyDocs = await companyService.GetCompanyDoc(id);

                ViewBag.CompanyId = id;
                    
                return View("CompanyDocuments", companyDocs);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("SaveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> SaveDoc(IFormFile uploadedFile, string nameDoc, string id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && !string.IsNullOrEmpty(id) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await companyService.SaveDocCompany(uploadedFile, nameDoc, Convert.ToInt32(id), localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Company/Doc?id={id}");
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> RemoveDoc(int docId, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await companyService.RemoveDocCompany(docId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Company/Doc?id={id}");
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GetUsers(int idCompanySelect)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                //ViewBag.Users = companyService.GetUsers(idCompanySelect);
                ViewBag.Companies = companyService.GetCompanies();
                ViewBag.IdCompanySelect = idCompanySelect;
                    
                return View("AllUsers");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Question")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> GetQuestionWithAnswer(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;

                var userQuestionWithAnswers = await companyService.GetUserQuestionWithAnswers(id);

                return PartialView("~/Views/PartView/AdminAnswerQuestions/AnswerModel.cshtml", userQuestionWithAnswers);
            }
            catch (Exception e)
            {
                return Redirect(Config.BaseReqvesteUrl);
            }
        }
        
        [HttpGet]
        [Route("Questions/New")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> GetNewQuestions(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;

                var userQuestions = await companyService.GetUserQuestionsWithoutAnswers(page);

                var countPages = await companyService.GetCountNewUserQuestionsPages();

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View("UserQuestionsNew", userQuestions);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }


        
        [HttpGet]
        [Route("Questions/Answered")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> GetAnsweredQuestions(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;

                var userQuestions = await companyService.GetUserQuestionsWithAnswers(page);

                var countPages = await companyService.GetCountAnsweredUserQuestionsPages();

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View("UserQuestionsAnswered", userQuestions);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Questions/All")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> GetAllQuestions(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;

                var userQuestions = await companyService.GetAllUserQuestions(page);

                var countPages = await companyService.GetCountAllUserQuestionsPages();

                ViewBag.CountPages = countPages;

                ViewBag.SelectedPage = page;

                return View("UserQuestionsAll", userQuestions);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Questions/Answer")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<bool> AnswerQuestion(AdminAnswerViewModel model, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                if (ModelState.IsValid)
                {
                    await companyService.SendUserAnswer(model, CompanyId, localDate);

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpGet]
        [Route("Company")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> CompanyInfo(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var companyInfo = await companyService.GetCompanyById(id);

                return View(companyInfo);
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