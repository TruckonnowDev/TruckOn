using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
using WebDispacher.ViewModels.RA.Carrier;
using WebDispacher.ViewModels.RA.Carrier.Login;
using WebDispacher.ViewModels.RA.Carrier.Registration;
using WebDispacher.ViewModels.Shipping;

namespace WebDispacher.Controellers
{
    public class RAController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public RAController(
            IUserService userService,
            ICompanyService companyService, 
            IDriverService driverService,
            UserManager<User> userManager,
            SignInManager<User> signInManager) : base(userService)
        {
            this.driverService = driverService;
            this.companyService = companyService;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Index(string alert)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            TempData["Alert"] = alert;

            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            
            return View("Index");
        }

        [HttpPost]
        public bool ActivePromo(string code)
        {
            // etc..

            return false;
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        [Route("try_for_free")]
        public IActionResult TryForFree()
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            
            return View("try_for_free");
        }

        [HttpGet]
        [Route("carrier-login")]
        public IActionResult CarrierLogin(string error, string email)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;
            ViewData[NavConstants.Reg] = NavConstants.CarrierReg;
            ViewData["UserEmail"] = email;

            return View("carrier-login");
        }
        
        [HttpGet]
        [Route("broker-login")]
        public IActionResult BrokerLogin(string error, string email)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData["UserEmail"] = email;

            return View("broker-login");
        }

        [HttpPost]
        public async Task<bool> ContactForm(UserMailQuestion userMailQuestion, string localDate)
        {
            try
            {
                var createdForm = await companyService.CreateUserQuestions(userMailQuestion, localDate);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        [Route("shipper-login")]
        public IActionResult ShipperLogin(string error)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;
            ViewData[NavConstants.Reg] = NavConstants.ShipperReg;
            
            return View("shipper-login");
        }

        [HttpGet]
        [Route("carrier-reg-congratulation")]
        public IActionResult CarrierRegCongratulation(string error)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;

            return View("carrier-reg-congratulation");
        }

        [HttpGet]
        [Route("carrier-reg")]
        public IActionResult CarrierReg(string error)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;
            
            return View("carrier-reg", new PersonalDataViewModel());
        }

        [HttpPost]
        [Route("carrier-reg")]
        public IActionResult CarrierReg(PersonalDataViewModel model)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (!string.IsNullOrEmpty(model.CompanyName))
            {
                var comanyName = companyService.CheckCompanyName(model.CompanyName);

                if(comanyName)
                {
                    ModelState.AddModelError(nameof(model.CompanyName).ToString(), "Name of the Company already exists");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    return RedirectToAction("CarrierRegSecondStep", "RA", model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                return View("carrier-reg", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("carrier-reg-second-step")]
        public IActionResult CarrierRegSecondStep(PersonalDataViewModel personalDataView = null)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
            if(!ModelState.IsValid)
            {
                return Redirect(Config.BaseReqvesteUrl);
            }

            return View("carrier-reg-second-step", new FewMoreDetailsViewModel {
                Type = new List<AccountType>
                {
                    new AccountType {Id = 1, TypeName = "Owner"},
                    new AccountType {Id = 2, TypeName = "Driver"}
                }, PersonalData = personalDataView});
        }
        
        [HttpPost]
        [Route("carrier-reg-second-step")]
        public async Task<IActionResult> CarrierRegSecondStep(FewMoreDetailsViewModel model)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        Email = model.PersonalData.Email,
                        UserName = model.PersonalData.Email,
                        FirstName = model.PersonalData.FirstName,
                        LastName = model.PersonalData.LastName,
                        DateTimeRegistration = DateTime.UtcNow,
                    };

                    var result = await userManager.CreateAsync(user, model.PersonalData.Password);
                    
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, RolesIdentityConstants.UserRole);
                        var company = await companyService.CreateCompany(model);

                        if (company.Id != 0 && user.Id != null)
                        {
                            await companyService.AddUserToCompany(user, company);

                            return View("carrier-reg-congratulation");
                        }
                        
                    }
                    foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                }
                catch
                {

                }
            }
            else
            {
                return View("carrier-reg-second-step", new FewMoreDetailsViewModel
                {
                    Type = new List<AccountType>
                {
                    new AccountType {Id = 1, TypeName = "Owner"},
                    new AccountType {Id = 2, TypeName = "Driver"}
                },
                    PersonalData = model.PersonalData,
                    SelectedType = model.SelectedType,
                    HowYouFindUs = model.HowYouFindUs,
                    Promo = model.Promo,
                    Units = model.Units
                });
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("shipper-reg")]
        public IActionResult ShipperReg(string error)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;
            
            return View("shipper-reg");
        }

        [HttpPost]
        [Route("shipper-reg")]
        public IActionResult ShipperReg(ShippingRegViewModel model)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    if (!ModelState.IsValid) return View("shipper-reg");

                    // etc //

                    return Redirect($"{Config.BaseReqvesteUrl}");

                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("shipper-reg", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("recovery-password-send-mail")]
        public IActionResult RecoveryPasswordSendMail(string error)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (User.Identity.IsAuthenticated)
            {
                return BaseRedirect();
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;
            
            return View("SendMailRecoveryPassword");
        }

        [HttpPost]
        [Route("recovery-password-send-mail")]
        public async Task<IActionResult> RecoveryPasswordCheckkMail(string email, string localDate)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return Redirect($"/recovery-password-send-mail?error=No user found with this Email");

            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "RA", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);

            await userService.SendRecoveryPasswordToEmail(user.Email, callbackUrl);

            await userService.CreatePasswordResets(user, code, localDate);

            return Redirect(Config.BaseReqvesteUrl +"?alert=SuccessSendEmail");
        }

        [HttpPost]
        public async Task<IActionResult> CarrierLogin(LoginViewModel model, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Email);
                    
                    if (user != null && await userManager
                            .CheckPasswordAsync(user, model.Password))
                    {
                        var isBlocked = await userManager.IsLockedOutAsync(user);
                        if (isBlocked)
                        {
                            ModelState.AddModelError("", "Your account is blocked, contact the administrator if an error has occurred.");
                            return View("carrier-login", model);
                        }
                        var existingClaims = await userManager.GetClaimsAsync(user);

                        var userCompanyActive = await companyService.GetActiveUserCompanyByCompanyTypeUserId(user.Id, CompanyType.Carrier);

                        await userManager.RemoveClaimsAsync(user, existingClaims);

                        var customClaims = new List<Claim>();

                        if(await userManager.IsInRoleAsync(user, RolesIdentityConstants.AdminRole))
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

                        await SetLastLoginDateToUser(model, localDate);

                        return Redirect(Config.BaseReqvesteUrl);
                    }
                        
                    ModelState.AddModelError("", UserConstants.PasswordEmailIncorrectly);

                    return View("carrier-login", model);
                }
                else
                {
                    return View("carrier-login", model);
                }
            }
            catch (Exception e)
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                var error = UserConstants.PasswordEmailIncorrectly;
                
                return Redirect($"/carrier-login?error={error}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BrokerLogin(LoginViewModel model, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(model.Email);

                    if (user != null && await userManager
                            .CheckPasswordAsync(user, model.Password))
                    {
                        var isBlocked = await userManager.IsLockedOutAsync(user);
                        if (isBlocked)
                        {
                            ModelState.AddModelError("", "Your account is blocked, contact the administrator if an error has occurred.");
                            return View("broker-login", model);
                        }
                        var existingClaims = await userManager.GetClaimsAsync(user);

                        var userCompanyActive = await companyService.GetActiveUserCompanyByCompanyTypeUserId(user.Id, CompanyType.Broker);

                        if (userCompanyActive == null)
                        {
                            ModelState.AddModelError("", UserConstants.PasswordEmailIncorrectly);

                            return View("broker-login", model);
                        }

                        await userManager.RemoveClaimsAsync(user, existingClaims);

                        var customClaims = new List<Claim>();

                        customClaims.Add(new Claim(ClaimsIdentityConstants.CompanyType, ClaimsIdentityConstants.CompanyBrokerValue));

                        if (userCompanyActive != null)
                        {
                            customClaims.Add(new Claim(ClaimsIdentityConstants.CompanyId, userCompanyActive.Id.ToString()));
                        }


                        await userManager.AddClaimsAsync(user, customClaims);

                        await signInManager.SignInAsync(user, isPersistent: false);

                        await SetLastLoginDateToUser(model, localDate);

                        return Redirect(Config.BaseReqvesteUrl);
                    }

                    ModelState.AddModelError("", UserConstants.PasswordEmailIncorrectly);

                    return View("broker-login", model);
                }
                else
                {
                    return View("broker-login", model);
                }
            }
            catch (Exception e)
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                var error = UserConstants.PasswordEmailIncorrectly;

                return Redirect($"/broker-login?error={error}");
            }
        }

        [HttpGet]
        [Route("Exsit")]
        [Authorize]
        public async Task<IActionResult> Exisit()
        {
            await signInManager.SignOutAsync();

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Avthorization/Exst")]
        public string AvthorizationExst(string Email, string Password)
        {
            try
            {
                if (Email == null || Password == null)
                    throw new Exception();
                
                if (userService.Authorization(Email, Password))
                {
                    var users = userService.GetUserByEmailAndPasswrod(Email, Password);
                    /*if(users?.KeyAuthorized != null && users.KeyAuthorized != string.Empty)
                    {
                        return users.KeyAuthorized;
                    }*/
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
            
            return string.Empty;
        }

        [HttpGet]
        [Route("Recovery/Password")]
        public IActionResult RecoveryPassword(string idDriver, string idUser, string token)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.IdDriver = idDriver;
                ViewBag.IdUser = idUser;
                ViewBag.Token = token;
                ViewBag.isStateActual = idDriver != null 
                    ? driverService.CheckTokenFoDriver(idDriver, token) : userService.CheckTokenFoUser(idUser, token);
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                
                return View("RecoveryPassword");
            }
            catch (Exception)
            {
            }
            
            return null;
        }
        
        [HttpGet]
        public IActionResult ResetPassword(string userId, string code)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            return View(new ResetPasswordViewModel { UserId = userId, Code = code });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", new { userId = model.UserId, code = model.Code });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPasswordConfirmation(string userId, string code)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;

            if (userId == null || code == null)
            {
                return View("ResetPasswordError");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("ResetPasswordError");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordError()
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            return View();
        }

        [HttpPost]
        [Route("Restore/Password")]
        public async Task<IActionResult> RestorePassword(string newPassword, string idDriver, string idUser, string token)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.IdDriver = idDriver;
                ViewBag.Token = token;
                ViewBag.isStateActual = idDriver != null ? await driverService.ResetPasswordFoDriver(newPassword, idDriver, token) : await userService.ResetPasswordFoUser(newPassword, idUser, token);
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                
                return View("RecoveryPassword");
            }
            catch (Exception)
            {
            }
            
            return null;
        }

        [HttpPost]
        private async Task SetLastLoginDateToUser(LoginViewModel model, string localDate)
        {
            await userService.SetLastLoginDateToUser(model, localDate);
        }

        private IActionResult BaseRedirect()
        {
            if (companyService.GetTypeNavBar(CompanyId) == NavConstants.BaseCompany)
                return Redirect("/Company/Companies");

            if (companyService.GetTypeNavBar(CompanyId) == NavConstants.BrokerCompany)
                return Redirect("/Checks/CarrierBrokerCheck");

            if (companyService.GetTypeNavBar(CompanyId) == NavConstants.NormalCompany)
                return Redirect("/Dashbord/Order/NewLoad");

            return Redirect("/error?code=404");
        }
    }
}