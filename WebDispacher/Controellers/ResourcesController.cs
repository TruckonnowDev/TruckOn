using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
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
        [Route("Resources/{name?}")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult Index(string name)
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

                if (string.IsNullOrEmpty(name)) return View("~/Views/Resources/PartViews/AllItems.cshtml");
                var resourceVm = new ResourceViewModel();
                switch (name.ToLower())
                {
                    case ResourcesConstants.EldServiceCase:
                        resourceVm.Name = ResourcesConstants.EldServiceName;
                        resourceVm.UrlPicture = ResourcesConstants.EldServiceUrlPicture;
                        resourceVm.Info = ResourcesConstants.EldServiceInfo;
                        break;
                    case ResourcesConstants.InsuranceProviderseCase:
                        resourceVm.Name = ResourcesConstants.InsuranceProvidersName;
                        resourceVm.UrlPicture = ResourcesConstants.InsuranceProvidersUrlPicture;
                        resourceVm.Info = ResourcesConstants.InsuranceProvidersInfo;
                        break;
                    case ResourcesConstants.LegalHelpCase:
                        resourceVm.Name = ResourcesConstants.LegalHelpName;
                        resourceVm.UrlPicture = ResourcesConstants.LegalHelpUrlPicture;
                        resourceVm.Info = ResourcesConstants.LegalHelpInfo;
                        break;
                    case ResourcesConstants.ComplianceSafetyCase:
                        resourceVm.Name = ResourcesConstants.ComplianceSafetyName;
                        resourceVm.UrlPicture = ResourcesConstants.ComplianceSafetyUrlPicture;
                        resourceVm.Info = ResourcesConstants.ComplianceSafetyInfo;
                        break;
                    case ResourcesConstants.MechanicalShopsCase:
                        resourceVm.Name = ResourcesConstants.MechanicalShopsName;
                        resourceVm.UrlPicture = ResourcesConstants.MechanicalShopsUrlPicture;
                        resourceVm.Info = ResourcesConstants.MechanicalShopsInfo;
                        break;
                    case ResourcesConstants.BodyShopsCase:
                        resourceVm.Name = ResourcesConstants.BodyShopsName;
                        resourceVm.UrlPicture = ResourcesConstants.BodyShopsUrlPicture;
                        resourceVm.Info = ResourcesConstants.BodyShopsInfo;
                        break;
                    case ResourcesConstants.TruckTrailerRepairCase:
                        resourceVm.Name = ResourcesConstants.TruckTrailerRepairName;
                        resourceVm.UrlPicture = ResourcesConstants.TruckTrailerRepairUrlPicture;
                        resourceVm.Info = ResourcesConstants.TruckTrailerRepairInfo;
                        break;
                    default:
                        return Redirect(Config.BaseReqvesteUrl);

                }
                return View("~/Views/Resources/PartViews/ServiceItem.cshtml", resourceVm);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
