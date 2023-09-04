using System;
using System.Linq;
using DaoModels.DAO.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
using WebDispacher.ViewModels.Payment;

namespace WebDispacher.Controellers.Settings
{
    [Route("Settings/Biling")]
    public class PaymentMethodController : BaseController
    {
        private readonly ICompanyService companyService;
        
        public PaymentMethodController(
            IUserService userService,
            ICompanyService companyService) : base(userService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        [Route("PaymentMethod")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult PaymentMethod()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, isCancelSubscribe
                        ? NavConstants.TypeNavCancel : NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                var paymentMethods = companyService.GetPaymentMethod(CompanyId);
                var paymentMethodSTs = companyService.GetPaymentMethodsST(CompanyId);
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
                    IsDefault = paymentMethodSTs.FirstOrDefault(pm => pm.PaymentMethodSTId == z.Id) != null
                        ? paymentMethodSTs.FirstOrDefault(pm => pm.PaymentMethodSTId == z.Id).IsDefault : false
                }).ToList();
                    
                ViewBag.PaymentMethods = paymentMethodDTOs;
                    
                return View("~/Views/Settings/Biling/PaymentMethod.cshtml");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("AddPaymentMethod")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult AddPaymentMethod()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, isCancelSubscribe
                    ? NavConstants.TypeNavCancel : NavConstants.TypeNavSettings);
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewBag.TxtError = string.Empty;
                    
                return View("~/Views/Settings/Biling/AddPaymentMethod.cshtml", new CardViewModel());
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("AddPaymentMethod")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult AddPaymentMethod(CardViewModel card)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    //ViewBag.NameCompany = GetCookieCompanyName();

                    ViewData[NavConstants.TypeNavBar] =
                        companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);

                    var responseStripe = companyService.AddPaymentCard(CompanyId, card);

                    if (!responseStripe.IsError)
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");

                    ViewBag.TxtError = responseStripe.Message;

                    return View("~/Views/Settings/Biling/AddPaymentMethod.cshtml", card);
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult SelectDefault(string idPayment)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();
                    
                ViewData[NavConstants.TypeNavBar] = 
                    companyService.GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                companyService.SelectDefaultPaymentMethod(idPayment, CompanyId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("DeletePaymentMethod")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult DeletePaymentMethod(string idPayment)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();
                    
                ViewData[NavConstants.TypeNavBar] = 
                    companyService. GetTypeNavBar(CompanyId, NavConstants.TypeNavSettings);
                    
                companyService.DeletePaymentMethod(idPayment, CompanyId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Settings/Biling/PaymentMethod");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
