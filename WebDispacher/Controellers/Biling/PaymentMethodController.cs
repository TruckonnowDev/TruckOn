using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Mosels;
using WebDispacher.Service;

namespace WebDispacher.Controellers.Biling
{
    [Route("Biling")]
    public class PaymentMethodController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly IUserService userService;
        private readonly ICompanyService companyService;

        public PaymentMethodController(
            IUserService userService,
            ICompanyService companyService)
        {
            this.companyService = companyService;
            this.userService = userService;
        }

        [HttpGet]
        [Route("PaymentMethod")]
        public IActionResult PaymentMethod()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    List<PaymentMethod> paymentMethods = companyService.GetPaymentMethod(idCompany);
                    List<PaymentMethod_ST> paymentMethod_STs = companyService.GetPaymentMethodsST(idCompany);
                    List<PaymentMethodDTO> paymentMethodDTOs = paymentMethods.Select(z => new PaymentMethodDTO()
                    {
                        Id = z.Id,
                        Brand = z.Card.Brand,
                        Country = z.Card.Country,
                        CvcCheck = z.Card.Checks.CvcCheck,
                        ExpMonth = z.Card.ExpMonth.ToString(),
                        ExpYear = z.Card.ExpYear.ToString(),
                        Last4 = z.Card.Last4,
                        Name = z.Metadata["name"],
                        IsDefault = paymentMethod_STs.FirstOrDefault(pm => pm.IdPaymentMethod_ST == z.Id) != null ? paymentMethod_STs.FirstOrDefault(pm => pm.IdPaymentMethod_ST == z.Id).IsDefault : false
                    }).ToList();
                    ViewBag.PaymentMethods = paymentMethodDTOs;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    actionResult = View("~/Views/Settings/Biling/PaymentMethod.cshtml");
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
        [Route("AddPaymentMethod")]
        public IActionResult AddPaymentMethod()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.TxtError = "";
                    ViewBag.Numbercard = "";
                    ViewBag.FullName = "";
                    ViewBag.Expire = "";
                    ViewBag.Cvv = "";
                    actionResult = View("~/Views/Settings/Biling/AddPaymentMethod.cshtml");
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
        [Route("AddPaymentMethod")]
        public IActionResult AddPaymentMethod(string number, string name, string expiry, string cvc)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    WebDispacher.Models.ResponseStripe responseStripe =  companyService.AddPaymentCard(idCompany, number, name, expiry, cvc);
                    if(responseStripe.IsError)
                    {
                        ViewBag.TxtError = responseStripe.Message;
                        ViewBag.Numbercard = number;
                        ViewBag.FullName = name;
                        ViewBag.Expire = expiry;
                        ViewBag.Cvv = cvc;
                        actionResult = View("~/Views/Settings/Biling/AddPaymentMethod.cshtml");
                    }
                    else
                    {
                        actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
                    }
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
        [Route("SelectDefauldPaymentMethod")]
        public IActionResult SelectDefault(string idPayment)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    companyService.SelectDefaultPaymentMethod(idPayment, idCompany);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
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
        [Route("DeletePaymentMethod")]
        public IActionResult DeletePaymentMethod(string idPayment)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    companyService.DeletePaymentMethod(idPayment, idCompany);
                    //List<PaymentMethod> paymentMethods = managerDispatch.GetpaymentMethod(idCompany);
                    //ViewBag.PaymentMethods = paymentMethods;
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
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
    }
}
