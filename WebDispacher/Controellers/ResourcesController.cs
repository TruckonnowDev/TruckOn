using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
using WebDispacher.ViewModels.Contact;
using WebDispacher.ViewModels.Resources;

namespace WebDispacher.Controellers
{
    public class ResourcesController : BaseController
    {
        private readonly ICompanyService companyService;

        public ResourcesController(
            IUserService userService,
            ICompanyService companyService) : base(userService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        [Route("Resources")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var resources = await companyService.GetAllResources();

                return View("~/Views/Resources/PartViews/AllItems.cshtml", resources);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Resources/UpdatePositionIndexes")]
        public async Task<IActionResult> UpdatePositionIndexes([FromBody] List<ResourceViewModel> updates)
        {
            try
            {
                await companyService.UpdatePositionResources(updates);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating positions: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("Resources/Admin/{id:int}/Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> EditResource(int id)
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

                var resourceViewModel = await companyService.GetResourceById(id);

                return View("EditItem", resourceViewModel);
            }
            catch (Exception)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Resources/Admin/{id:int}/Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> EditResource(ResourceViewModel model, string localDate)
        {
            ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await companyService.EditResource(model, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Resources/Admin/{model.Id}/");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("EditItem", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Resources/Admin")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> AdminIndex()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var resources = await companyService.GetAllResources();

                return View("~/Views/Resources/PartViews/AdminAllItems.cshtml", resources);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Resources/Admin/{id:int}/")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> AdminResourceItem(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var resourceVm = await companyService.GetResourceById(id);

                if (resourceVm == null) { return View("~/Views/Resources/PartViews/AdminAllItems.cshtml"); }

                return View("~/Views/Resources/PartViews/AdminServiceItem.cshtml", resourceVm);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Resources/{id:int}")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> ResourceItem(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = NavConstants.NormalCompany;
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var resourceVm = await companyService.GetResourceById(id);

                if (resourceVm == null) { return View("~/Views/Resources/PartViews/AllItems.cshtml"); }

                return View("~/Views/Resources/PartViews/ServiceItem.cshtml", resourceVm);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
