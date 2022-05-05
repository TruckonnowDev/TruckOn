using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
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
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData["TypeNavBar"] = "AllUsers";
            
            return View("Index");
        }

        [HttpGet]
        [Route("try_for_free")]
        public IActionResult TryForFree()
        {
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData["TypeNavBar"] = "NavTry_for_free";
            
            return View("try_for_free");
        }

        [HttpGet]
        [Route("carrier-login")]
        public IActionResult CarrierLogin(string error)
        {
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData["TypeNavBar"] = "NavTry_for_free";
            ViewData["TextError"] = error;
            ViewData["reg"] = "/carrier-reg";
            
            return View("carrier-login");
        }

        [HttpGet]
        [Route("shipper-login")]
        public IActionResult ShipperLogin(string error)
        {
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData["TypeNavBar"] = "NavTry_for_free";
            ViewData["TextError"] = error;
            ViewData["reg"] = "/shipper-reg";
            
            return View("shipper-login");
        }

        [HttpGet]
        [Route("carrier-reg")]
        public IActionResult CarrierReg(string error)
        {
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData["TypeNavBar"] = "NavTry_for_free";
            ViewData["TextError"] = error;
            
            return View("carrier-reg");
        }

        [HttpGet]
        [Route("shipper-reg")]
        public IActionResult ShipperReg(string error)
        {
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData["TypeNavBar"] = "NavTry_for_free";
            ViewData["TextError"] = error;
            
            return View("shipper-reg");
        }

        [HttpGet]
        [Route("recovery-password-send-mail")]
        public IActionResult RecoveryPasswordSendMail(string error)
        {
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                return Redirect("/Dashbord/Order/NewLoad");
            }

            ViewData["TypeNavBar"] = "NavTry_for_free";
            ViewData["TextError"] = error;
            
            return View("SendMailRecoveryPassword");
        }

        [HttpPost]
        [Route("recovery-password-send-mail")]
        public IActionResult RecoveryPasswordCheckkMail(string email)
        {
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
            var isEmail = userService.CheckEmail(email);

            return Redirect(isEmail ? Config.BaseReqvesteUrl : $"/recovery-password-send-mail?error=No user found with this mail");
        }

        [HttpPost]
        public IActionResult Avthorization(string Email, string Password, string accept)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "NavTry_for_free";
            try
            {
                if (Email == null || Password == null)
                    throw new Exception();
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                if (userService.Authorization(Email, Password))
                {
                    ViewData["hidden"] = "";

                    var key = userService.CreateKey(Email, Password);
                    var Commpany = userService.GetUserByKeyUser(key);
                    
                    Response.Cookies.Append("KeyAvtho", key.ToString());
                    Response.Cookies.Append("CommpanyId", Commpany.Id.ToString());
                    Response.Cookies.Append("CommpanyName", Commpany.Name);
                    
                    return Redirect("/Dashbord/Order/NewLoad");
                }

                ViewData["hidden"] = "hidden";
                ViewData["TextError"] = "Password or mail have been entered incorrectly";
                var error = "Password or mail have been entered incorrectly";
                    
                actionResult = Redirect($"/carrier-login?error={error}");

            }
            catch (Exception e)
            {
                ViewData["hidden"] = "hidden";
                var error = "Password or mail have been entered incorrectly";
                return Redirect($"/carrier-login?error={error}");
            }
            
            return actionResult;
        }

        [HttpGet]
        [Route("Exsit")]
        public IActionResult Exisit()
        {
            Response.Cookies.Delete("KeyAvtho");
            Response.Cookies.Delete("CommpanyId");
            Response.Cookies.Delete("CommpanyName");
            
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
                    if(users?.KeyAuthorized != null && users.KeyAuthorized != "")
                    {
                        return users.KeyAuthorized;
                    }
                }
            }
            catch (Exception e)
            {
                return "";
            }
            
            return "";
        }

        [HttpGet]
        [Route("Recovery/Password")]
        public IActionResult RecoveryPassword(string idDriver, string idUser, string token)
        {
            ViewData["TypeNavBar"] = "AllUsers";
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.IdDriver = idDriver;
                ViewBag.IdUser = idUser;
                ViewBag.Token = token;
                ViewBag.isStateActual = idDriver != null ? driverService.CheckTokenFoDriver(idDriver, token) : userService.CheckTokenFoUser(idUser, token);
                ViewData["hidden"] = "hidden";
                
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
            ViewData["TypeNavBar"] = "AllUsers";
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.IdDriver = idDriver;
                ViewBag.Token = token;
                ViewBag.isStateActual = idDriver != null ? await driverService.ResetPasswordFoDriver(newPassword, idDriver, token) : await userService.ResetPasswordFoUser(newPassword, idUser, token);
                ViewData["hidden"] = "hidden";
                
                return View("RecoveryPassword");
            }
            catch (Exception)
            {
            }
            
            return null;
        }
    }
}