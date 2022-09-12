using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class RAController : Controller
    {
        private readonly IUserService userService;
        private readonly IDriverService driverService;

        public RAController(
            IUserService userService, 
            IDriverService driverService)
        {
            this.driverService = driverService;
            this.userService = userService;
        }
        public IActionResult Index()
        {
            ViewData[NavConstants.TextError] = string.Empty;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData[NavConstants.TypeNavBar] = NavConstants.AllUsers;
            
            return View("Index");
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
        public IActionResult CarrierLogin(string error)
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
            
            return View("carrier-login");
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
            
            return View("carrier-reg");
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

            return Redirect(isEmail ? Config.BaseReqvesteUrl : $"/recovery-password-send-mail?error=No user found with this mail");
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
                    
                actionResult = Redirect($"/carrier-login?error={error}");

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