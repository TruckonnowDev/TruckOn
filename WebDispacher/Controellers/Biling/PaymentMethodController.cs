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
using WebDispacher.Constants;
using WebDispacher.Mosels;
using WebDispacher.Service;
using WebDispacher.ViewModels.Payment;

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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.PaymentMethod))
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
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    return View("~/Views/Settings/Biling/PaymentMethod.cshtml");
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
        [Route("AddPaymentMethod")]
        public IActionResult AddPaymentMethod()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.PaymentMethod))
                {
                    ViewBag.NameCompany = companyName;
                    
                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    ViewBag.TxtError = string.Empty;
                    ViewBag.Numbercard = string.Empty;
                    ViewBag.FullName = string.Empty;
                    ViewBag.Expire = string.Empty;
                    ViewBag.Cvv = string.Empty;
                    
                    return View("~/Views/Settings/Biling/AddPaymentMethod.cshtml");
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
        [Route("AddPaymentMethod")]
        public IActionResult AddPaymentMethod(CardViewModel card)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                    Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);

                    if (userService.CheckPermissions(key, idCompany, RouteConstants.PaymentMethod))
                    {
                        ViewBag.NameCompany = companyName;
                        ViewData[NavConstants.TypeNavBar] =
                            companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);

                        var responseStripe = companyService.AddPaymentCard(idCompany, card);

                        if (!responseStripe.IsError)
                            return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");

                        ViewBag.TxtError = responseStripe.Message;

                        return View("~/Views/Settings/Biling/AddPaymentMethod.cshtml", card);
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
                return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
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
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.PaymentMethod))
                {
                    ViewBag.NameCompany = companyName;
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    companyService.SelectDefaultPaymentMethod(idPayment, idCompany);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
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
        [Route("DeletePaymentMethod")]
        public IActionResult DeletePaymentMethod(string idPayment)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.PaymentMethod))
                {
                    ViewBag.NameCompany = companyName;
                    
                    ViewData[NavConstants.TypeNavBar] = 
                        companyService.GetTypeNavBar(key, idCompany, NavConstants.TypeNavSettings);
                    
                    companyService.DeletePaymentMethod(idPayment, idCompany);
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
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
    }
}
