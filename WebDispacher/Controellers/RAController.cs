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
        ManagerDispatch managerDispatch = new ManagerDispatch();
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
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                ViewData["TypeNavBar"] = "AllUsers";
                actionResult = View("Index");
            }
            return actionResult;
        }

        [HttpGet]
        [Route("try_for_free")]
        public IActionResult TryForFree()
        {
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                ViewData["TypeNavBar"] = "NavTry_for_free";
                actionResult = View("try_for_free");
            }
            return actionResult;
        }

        [HttpGet]
        [Route("carrier-login")]
        public IActionResult CarrierLogin(string error)
        {
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                ViewData["TypeNavBar"] = "NavTry_for_free";
                ViewData["TextError"] = error;
                ViewData["reg"] = "/carrier-reg";
                actionResult = View("carrier-login");
            }
            return actionResult;
        }

        [HttpGet]
        [Route("shipper-login")]
        public IActionResult ShipperLogin(string error)
        {
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                ViewData["TypeNavBar"] = "NavTry_for_free";
                ViewData["TextError"] = error;
                ViewData["reg"] = "/shipper-reg";
                actionResult = View("shipper-login");
            }
            return actionResult;
        }

        [HttpGet]
        [Route("carrier-reg")]
        public IActionResult CarrierReg(string error)
        {
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                ViewData["TypeNavBar"] = "NavTry_for_free";
                ViewData["TextError"] = error;
                actionResult = View("carrier-reg");
            }
            return actionResult;
        }

        [HttpGet]
        [Route("shipper-reg")]
        public IActionResult ShipperReg(string error)
        {
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                ViewData["TypeNavBar"] = "NavTry_for_free";
                ViewData["TextError"] = error;
                actionResult = View("shipper-reg");
            }
            return actionResult;
        }

        [HttpGet]
        [Route("recovery-password-send-mail")]
        public IActionResult RecoveryPasswordSendMail(string error)
        {
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            if (Request.Cookies.ContainsKey("KeyAvtho"))
            {
                actionResult = Redirect("/Dashbord/Order/NewLoad");
            }
            else
            {
                ViewData["TypeNavBar"] = "NavTry_for_free";
                ViewData["TextError"] = error;
                actionResult = View("SendMailRecoveryPassword");
            }
            return actionResult;
        }

        [HttpPost]
        [Route("recovery-password-send-mail")]
        public IActionResult RecoveryPasswordCheckkMail(string email)
        {
            IActionResult actionResult = null;
            ViewData["TextError"] = "";
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            bool isEmail = userService.CheckEmail(email);
            if(isEmail)
            {
                actionResult = Redirect(Config.BaseReqvesteUrl);
            }
            else
            {

                actionResult = Redirect($"/recovery-password-send-mail?error=No user found with this mail");
            }
            return actionResult;
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
                    actionResult = Redirect("/Dashbord/Order/NewLoad");
                    int key = userService.CreateKey(Email, Password);
                    Commpany Commpany = userService.GetUserByKeyUser(key);
                    Response.Cookies.Append("KeyAvtho", key.ToString());
                    Response.Cookies.Append("CommpanyId", Commpany.Id.ToString());
                    Response.Cookies.Append("CommpanyName", Commpany.Name);
                }
                else
                {
                    ViewData["hidden"] = "hidden";
                    ViewData["TextError"] = "Password or mail have been entered incorrectly";
                    string error = "Password or mail have been entered incorrectly";
                    actionResult = Redirect($"/carrier-login?error={error}");
                }

            }
            catch (Exception e)
            {
                ViewData["hidden"] = "hidden";
                string error = "Password or mail have been entered incorrectly";
                actionResult = Redirect($"/carrier-login?error={error}");
            }
            return actionResult;
        }

        [HttpGet]
        [Route("Exsit")]
        public IActionResult Exisit()
        {
            IActionResult actionResult = null;
            actionResult = Redirect(Config.BaseReqvesteUrl);
            Response.Cookies.Delete("KeyAvtho");
            Response.Cookies.Delete("CommpanyId");
            Response.Cookies.Delete("CommpanyName");
            return actionResult;
        }

        [HttpGet]
        [Route("Avthorization/Exst")]
        public string AvthorizationExst(string Email, string Password)
        {
            string actionResult = null;
            try
            {
                if (Email == null || Password == null)
                    throw new Exception();
                if (userService.Authorization(Email, Password))
                {
                    Users users = userService.GetUserByEmailAndPasswrod(Email, Password);
                    if(users != null && users.KeyAuthorized != null && users.KeyAuthorized != "")
                    {
                        actionResult = users.KeyAuthorized;
                    }
                    else
                    {
                        actionResult = "";
                    }
                }
                else
                {
                    actionResult = "";
                }

            }
            catch (Exception e)
            {
                actionResult = "";
            }
            return actionResult;
        }

        [HttpGet]
        [Route("Recovery/Password")]
        public IActionResult RecoveryPassword(string idDriver, string idUser, string token)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "AllUsers";
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.IdDriver = idDriver;
                ViewBag.IdUser = idUser;
                ViewBag.Token = token;
                ViewBag.isStateActual = idDriver != null ? driverService.CheckTokenFoDriver(idDriver, token) : userService.CheckTokenFoUser(idUser, token);
                ViewData["hidden"] = "hidden";
                actionResult = View("RecoveryPassword");
            }
            catch (Exception)
            {
            }
            return actionResult;
        }

        [HttpPost]
        [Route("Restore/Password")]
        public async Task<IActionResult> RestorePassword(string newPassword, string idDriver, string idUser, string token)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "AllUsers";
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.IdDriver = idDriver;
                ViewBag.Token = token;
                ViewBag.isStateActual = idDriver != null ? await driverService.ResetPasswordFoDriver(newPassword, idDriver, token) : await userService.ResetPasswordFoUser(newPassword, idUser, token);
                ViewData["hidden"] = "hidden";
                actionResult = View("RecoveryPassword");
            }
            catch (Exception)
            {
            }
            return actionResult;
        }
    }
}