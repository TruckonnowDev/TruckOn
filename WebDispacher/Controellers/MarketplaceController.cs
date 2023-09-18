using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
using WebDispacher.ViewModels.Marketplace;

namespace WebDispacher.Controellers
{
    public class MarketplaceController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IMarketplaceService marketplaceService;
        public MarketplaceController(
            IUserService userService, 
            ICompanyService companyService,
            IMarketplaceService marketplaceService) 
            : base(userService)
        {
            this.companyService = companyService;
            this.marketplaceService = marketplaceService;
        }

        [HttpGet]
        [Route("Marketplace")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult Index()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Classifieds")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult Classifieds()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Classifieds/CategoryBuy")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CategoryBuy(int page = 1)
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

                var buyItems = await marketplaceService.GetPublicBuyItemsMarketPosts(page);

                var countPages = await companyService.GetCountContactsPages(CompanyId);

                ViewBag.SelectedPage = page;

                return View(new BuyItemMarketPostShortVmList
                {
                    Items = buyItems,
                    CountPage = countPages
                });
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Classifieds/CategorySell")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CategorySell()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Classifieds/CategoryBuy/Item")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult BuyLotPage()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Classifieds/CategorySell/Item")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult SellLotPage()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Classifieds/CategoryBuy/CreateBuyLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CreateBuyLot()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Marketplace/Classifieds/CategoryBuy/CreateBuyLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CreateBuyLot(CreateBuyLotViewModel model, string localDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await marketplaceService.CreateBuyLot(model, CompanyId, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/Classifieds/CategoryBuy/Item");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("Marketplace/Classifieds/CategoryBuy/CreateBuyLot", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
    
        [HttpGet]
        [Route("Marketplace/Classifieds/CategorySell/CreateSellLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CreateSellLot()
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
                return View();
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
    }
}
