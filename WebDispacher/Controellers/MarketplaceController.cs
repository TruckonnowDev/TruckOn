using AspNetCoreRateLimit;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
using WebDispacher.ViewModels;
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                try
                {
                    var allCategoryInfo = await marketplaceService.GetCountViewAndDateLastUpdateAll();

                    ViewBag.AllCategoryInfo = allCategoryInfo;
                }
                catch (Exception ex)
                {
                    ViewBag.AllCategoryInfo = (0, DateTime.MinValue);
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> Classifieds(string userId = null)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                
                if (string.IsNullOrEmpty(userId))
                {
                    userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                
                try
                {
                    var userCategoryInfo = await marketplaceService.GetCountViewAndDateLastUpdateByUserId(userId);
                    ViewBag.UserCategoryInfo = userCategoryInfo;
                }
                catch (Exception ex)
                {
                    ViewBag.UserCategoryInfo = (0, DateTime.MinValue);
                    
                }
                
                try
                {
                    var buyCategoryInfo = await marketplaceService.GetCountViewAndDateLastUpdateBuy();
                    ViewBag.BuyCategoryInfo = buyCategoryInfo;
                }
                catch (Exception ex)
                {
                    ViewBag.BuyCategoryInfo = (0, DateTime.MinValue);
                    
                }
                
                try
                {
                    var sellCategoryInfo = await marketplaceService.GetCountViewAndDateLastUpdateSell();
                    ViewBag.SellCategoryInfo = sellCategoryInfo;
                }
                catch (Exception ex)
                {
                    ViewBag.SellCategoryInfo = (0, DateTime.MinValue);
                    
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> CategoryBuy(BuyMarketPostsFiltersViewModel filters)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var user = await companyService.GetUserByCompanyId(CompanyId);

                var buyItems = await marketplaceService.GetBuyItemsMarketPosts(filters, user);
                
                return View(new BuyItemMarketPostShortVmList
                {
                    Items = buyItems,
                    Filters = filters,
                });
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Marketplace/AllPages")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> UserLotAll(int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;

                var lots = await marketplaceService.GetFullPendingItemsMarketPosts(new UserMarketPostsFiltersViewModel { Page = page } );

                ViewBag.CountPages = 1;

                ViewBag.SelectedPage = page;

                return View("UserLotAll", lots);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Marketplace/Classifieds/MyAds")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> UserLots(UserMarketPostsFiltersViewModel filters)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var user = await companyService.GetUserByCompanyId(CompanyId);

                var buyItems = await marketplaceService.GetUserItemsMarketPosts(filters, user);

                return View(new UserMarketPostShortVmList
                {
                    Items = buyItems,
                    Filters = filters,
                });
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Marketplace/Classifieds/CategorySell")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> CategorySell(SellMarketPostsFiltersViewModel filters)
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

                var user = await companyService.GetUserByCompanyId(CompanyId);

                var buyItems = await marketplaceService.GetSellItemsMarketPosts(filters, user);

                return View(new SellItemMarketPostShortVmList
                {
                    Items = buyItems,
                    Filters = filters,
                });
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Marketplace/Classifieds/CategoryBuy/{id:int}/")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> BuyLotPage(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var queryUser = await companyService.GetUserByCompanyId(CompanyId);
                ViewBag.Admin = await companyService.GetCompanyById(Int32.Parse(CompanyId));

                var buyLotItem = await marketplaceService.GetBuyItemMarketPostWithHistory(id, queryUser.Id);
                ViewBag.QueryUser = queryUser;

                if(buyLotItem != null)
                {
                    await marketplaceService.AddViewToMarketPost(id, queryUser.Id);

                    return View(buyLotItem);
                }

                return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/Classifieds/CategoryBuy");
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        

        [HttpGet]
        [Route("Marketplace/GetChanges")]
        public IActionResult GetMarketPlaceChanges(string date, int marketPostId)
        {
            try
            {
                var selectedDate = DateTime.ParseExact(date, DateTimeFormats.DateTimeInfoUS, null);

                var historyForDate = marketplaceService.GetHistoryForDate(selectedDate, marketPostId);

                return PartialView("~/Views/PartView/History/ChangeHistoryModalData.cshtml", historyForDate);

            }
            catch(Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }


        [HttpGet]
        [Route("Marketplace/Classifieds/CategoryBuy/{id:int}/Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> ManageBuyLot(int id)
        {
            try
            {
                 ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var queryUser = await companyService.GetUserByCompanyId(CompanyId);

                if (!marketplaceService.IsHavePermissionToEditMarketPost(id, queryUser.Id))
                {
                    return Forbid();
                }

                var buyLotItem = await marketplaceService.GetBuyItemMarketPost(id, queryUser.Id);

                ViewBag.QueryUser = queryUser;

                return View(buyLotItem);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Classifieds/CategorySell/{id:int}/Edit")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> ManageSellLot(int id)
        {
            try
            {
                 ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var queryUser = await companyService.GetUserByCompanyId(CompanyId);

                if (!marketplaceService.IsHavePermissionToEditMarketPost(id, queryUser.Id))
                {
                    return Forbid();
                }

                var sellLotItem = await marketplaceService.GetSellItemMarketPost(id, queryUser.Id);

                ViewBag.QueryUser = queryUser;

                return View(sellLotItem);
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        
        [HttpPost]
        [Route("Marketplace/RemoveImage")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> RemoveUploadedImage(int imageId, int postId)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Json(new { success = false });
                }

                var user = await companyService.GetUserByCompanyId(CompanyId);

                if (!marketplaceService.IsHavePermissionToEditMarketPost(postId, user.Id))
                {
                    return Json(new { success = false });
                }

                var b = await marketplaceService.RemoveUploadedImage(imageId);

                return Json(new { success = true }); ;
            }
            catch (Exception e)
            {

            }
            return Json(new { success = false });
        }

        [HttpGet]
        [Route("Marketplace/Classifieds/CategorySell/{id:int}/")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> SellLotPage(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var queryUser = await companyService.GetUserByCompanyId(CompanyId);
                ViewBag.Admin = await companyService.GetCompanyById(Int32.Parse(CompanyId));

                var sellLotItem = await marketplaceService.GetSellItemMarketPostWithHistory(id, queryUser.Id);
                ViewBag.QueryUser = queryUser;

                if (sellLotItem != null)
                {
                    await marketplaceService.AddViewToMarketPost(id, queryUser.Id);

                    return View(sellLotItem);
                }

                return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/Classifieds/CategorySell");
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Marketplace/Admin/Save")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> SaveDesidion(PostApprovalHistory model)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                
                await marketplaceService.SendDesidionToPost(model);

                return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/AllPages");
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpGet]
        [Route("Marketplace/Sell/Admin/{id:int}")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> CheckSell(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }

                var sellLotItem = await marketplaceService.GetSellItemMarketPostPendingWithHistory(id);
                var historyChecks = await marketplaceService.GetHistoryChecksMarketPost(id);
                return View((sellLotItem, historyChecks));
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Marketplace/buy/Admin/{id:int}/")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierAdminCompany)]
        public async Task<IActionResult> CheckBuy(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);

                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                var buyLotItem = await marketplaceService.GetBuyItemMarketPostPendingWithHistory(id);
                var historyChecks = await marketplaceService.GetHistoryChecksMarketPost(id);
                return View((buyLotItem, historyChecks));
            }
            catch (Exception e)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Marketplace/Classifieds/CategoryBuy/CreateBuyLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public IActionResult CreateBuyLot()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
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
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> CreateBuyLot(CreateBuyLotViewModel model, string uploadedFiles, string localDate)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);

                    

                    var itemId = await marketplaceService.CreateBuyLot(model, uploadedFiles, CompanyId, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/Classifieds/CategoryBuy/{itemId}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return View("Marketplace/Classifieds/CategoryBuy/CreateBuyLot", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Marketplace/Classifieds/CategorySell/CreateSellLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> CreateSellLot(CreateSellLotViewModel model, string uploadedFiles, string localDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
                    
                    var itemId = await marketplaceService.CreateSellLot(model, uploadedFiles, CompanyId, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/Classifieds/CategorySell/{itemId}");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View(model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Marketplace/Classifieds/CategoryBuy/UpdateBuyLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> UpdateBuyLot(BuyItemMarketPostViewModel model, string uploadedFiles, string localDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    var itemId = await marketplaceService.UpdateBuyLot(model, CompanyId, uploadedFiles, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/Classifieds/CategoryBuy/{itemId}");
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
        
        [HttpPost]
        [Route("Marketplace/Classifieds/CategorySell/UpdateSellLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> UpdateSellLot(SellItemMarketPostViewModel model, string uploadedFiles, string localDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    var itemId = await marketplaceService.UpdateSellLot(model, CompanyId, uploadedFiles, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Marketplace/Classifieds/CategorySell/{itemId}");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return View("Marketplace/Classifieds/CategorySell/CreateSellLot", model);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        
        [HttpPost]
        [Route("Marketplace/RemoveMarketPost")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> RemoveMarketPost(int postId, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var user = await companyService.GetUserByCompanyId(CompanyId);

                if (!marketplaceService.IsHavePermissionToEditMarketPost(postId, user.Id))
                {
                    return Forbid();
                }

                await marketplaceService.UpdateMarketPostById(postId, localDate, DaoModels.DAO.Enum.ConditionPost.Deleted);

                return Json(new { 
                    success = true
                });
            }
            catch (Exception)
            {

            }

            return Json(new
            {
                success = false
            });
        }
        
        [HttpPost]
        [Route("Marketplace/CloseMarketPost")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public async Task<IActionResult> CloseMarketPost(int postId, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var user = await companyService.GetUserByCompanyId(CompanyId);

                if (!marketplaceService.IsHavePermissionToEditMarketPost(postId, user.Id))
                {
                    return Forbid();
                }

                await marketplaceService.UpdateMarketPostById(postId, localDate, DaoModels.DAO.Enum.ConditionPost.Sold);

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception)
            {

            }

            return Json(new
            {
                success = false
            });
        }

        [HttpGet]
        [Route("Marketpace/Image")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetImage(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }

            string fileExtension = Path.GetExtension(imagePath);

            if (!IsAllowedExtension(fileExtension))
            {
                return BadRequest("Недопустимый формат изображения.");
            }

            var imageFileStream = System.IO.File.OpenRead(imagePath);

            return File(imageFileStream, DocAndFileConstants.ContentTypeImage);
        }

        [HttpGet]
        [Route("Marketplace/Classifieds/CategorySell/CreateSellLot")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompanyOrAdmin)]
        public IActionResult CreateSellLot()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId, NavConstants.NormalCompany);
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

        private bool IsAllowedExtension(string extension)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", };

            return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
