using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Service;
using WebDispacher.ViewModels.Contact;
using WebDispacher.ViewModels.RA.Carrier.Registration;
using WebDispacher.ViewModels.Shipping;

namespace WebDispacher.Controellers
{
    public class RAController : Controller
    {
        private readonly IUserService userService;
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;

        public RAController(
            IUserService userService,
            ICompanyService companyService, 
            IDriverService driverService)
        {
            this.driverService = driverService;
            this.userService = userService;
            this.companyService = companyService;
        }

        public IActionResult Index(string alert)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            TempData["Alert"] = alert;
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
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
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
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
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;
            ViewData[NavConstants.Reg] = NavConstants.CarrierReg;
            ViewData["UserEmail"] = email;
            return View("carrier-login");
        }

        [HttpPost]
        public bool ContactForm(string username, string email, string phone, string message)
        {
            // etc

            return true;
        }

        [HttpGet]
        [Route("shipper-login")]
        public IActionResult ShipperLogin(string error)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
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

            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
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
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
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

            if (!string.IsNullOrEmpty(model.Email))
            {
                var email = userService.CheckEmailDb(model.Email);

                if(email)
                {
                    ModelState.AddModelError(nameof(model.Email).ToString(), "Email already exists");
                }
            }
            
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
        public IActionResult CarrierRegSecondStep(FewMoreDetailsViewModel model)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (ModelState.IsValid)
            {
                try
                {
                    var company = companyService.AddShortCompany(model);

                    return View("carrier-reg-congratulation");
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
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
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
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            ViewData[NavConstants.TextError] = error;
            
            return View("SendMailRecoveryPassword");
        }

        [HttpPost]
        [Route("recovery-password-send-mail")]
        public IActionResult RecoveryPasswordCheckkMail(string email)
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            var isEmail = userService.CheckEmail(email);

            return Redirect(isEmail ? Config.BaseReqvesteUrl +"?alert=SuccessSendEmail"  : $"/recovery-password-send-mail?error=No user found with this Email");
        }

        [HttpPost]
        public IActionResult Avthorization(string Email, string Password, string accept)
        {
            IActionResult actionResult = null;
            ViewData[NavConstants.TypeNavBar] = NavConstants.NavTryForFree;
            try
            {
                if (Email == null || Password == null)
                    throw new Exception();
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                if (userService.Authorization(Email, Password))
                {
                    ViewData[NavConstants.Hidden] = string.Empty;

                    var key = userService.CreateKey(Email, Password);
                    var Commpany = userService.GetUserByKeyUser(key);
                    
                    Response.Cookies.Append(CookiesKeysConstants.CarKey, key.ToString());
                    Response.Cookies.Append(CookiesKeysConstants.CompanyIdKey, Commpany.Id.ToString());
                    Response.Cookies.Append(CookiesKeysConstants.CompanyNameKey, Commpany.Name);
                    
                    return Redirect("/Dashbord/Order/NewLoad");
                }

                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                ViewData[NavConstants.TextError] = UserConstants.PasswordEmailIncorrectly;
                var error = UserConstants.PasswordEmailIncorrectly;

                actionResult = Redirect($"/carrier-login?error={error}&email={Email}");

            }
            catch (Exception e)
            {
                ViewData[NavConstants.Hidden] = NavConstants.Hidden;
                var error = UserConstants.PasswordEmailIncorrectly;
                
                return Redirect($"/carrier-login?error={error}");
            }
            
            return actionResult;
        }

        [HttpGet]
        [Route("Exsit")]
        public IActionResult Exisit()
        {
            Response.Cookies.Delete(CookiesKeysConstants.CarKey);
            Response.Cookies.Delete(CookiesKeysConstants.CompanyIdKey);
            Response.Cookies.Delete(CookiesKeysConstants.CompanyNameKey);
            
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
                    if(users?.KeyAuthorized != null && users.KeyAuthorized != string.Empty)
                    {
                        return users.KeyAuthorized;
                    }
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
    }
}