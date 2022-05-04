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
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    
                    var paymentMethods = companyService.GetPaymentMethod(idCompany);
                    var paymentMethodSTs = companyService.GetPaymentMethodsST(idCompany);
                    var paymentMethodDTOs = paymentMethods.Select(z => new PaymentMethodDTO()
                    {
                        Id = z.Id,
                        Brand = z.Card.Brand,
                        Country = z.Card.Country,
                        CvcCheck = z.Card.Checks.CvcCheck,
                        ExpMonth = z.Card.ExpMonth.ToString(),
                        ExpYear = z.Card.ExpYear.ToString(),
                        Last4 = z.Card.Last4,
                        Name = z.Metadata["name"],
                        IsDefault = paymentMethodSTs.FirstOrDefault(pm => pm.IdPaymentMethod_ST == z.Id) != null 
                            ? paymentMethodSTs.FirstOrDefault(pm => pm.IdPaymentMethod_ST == z.Id).IsDefault : false
                    }).ToList();
                    
                    ViewBag.PaymentMethods = paymentMethodDTOs;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    
                    return View("~/Views/Settings/Biling/PaymentMethod.cshtml");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("AddPaymentMethod")]
        public IActionResult AddPaymentMethod()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    ViewBag.TxtError = "";
                    ViewBag.Numbercard = "";
                    ViewBag.FullName = "";
                    ViewBag.Expire = "";
                    ViewBag.Cvv = "";
                    
                    return View("~/Views/Settings/Biling/AddPaymentMethod.cshtml");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("AddPaymentMethod")]
        public IActionResult AddPaymentMethod(string number, string name, string expiry, string cvc)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    var responseStripe =  companyService.AddPaymentCard(idCompany, number, name, expiry, cvc);

                    if (!responseStripe.IsError)
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
                    
                    ViewBag.TxtError = responseStripe.Message;
                    ViewBag.Numbercard = number;
                    ViewBag.FullName = name;
                    ViewBag.Expire = expiry;
                    ViewBag.Cvv = cvc;
                        
                    return View("~/Views/Settings/Biling/AddPaymentMethod.cshtml");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("SelectDefauldPaymentMethod")]
        public IActionResult SelectDefault(string idPayment)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    companyService.SelectDefaultPaymentMethod(idPayment, idCompany);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("DeletePaymentMethod")]
        public IActionResult DeletePaymentMethod(string idPayment)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "PaymentMethod"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany, "Settings");
                    companyService.DeletePaymentMethod(idPayment, idCompany);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
