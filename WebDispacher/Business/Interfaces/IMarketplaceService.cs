using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebDispacher.ViewModels.Marketplace;

namespace WebDispacher.Business.Interfaces
{
    public interface IMarketplaceService
    {
        Task<List<ItemMarketPostShortViewModel>> GetBuyItemsMarketPosts(BuyMarketPostsFiltersViewModel filters, User user);
        Task<List<ItemMarketPostShortViewModel>> GetSellItemsMarketPosts(SellMarketPostsFiltersViewModel filters, User user);
        Task<int> CreateBuyLot(CreateBuyLotViewModel model, List<IFormFile> files, string companyId, string localDate);
        Task<int> CreateSellLot(CreateSellLotViewModel model, List<IFormFile> files, string companyId, string localDate);
        Task<BuyItemMarketPostViewModel> GetBuyItemMarketPost(int id, string currentUserId);
        Task<SellItemMarketPostViewModel> GetSellItemMarketPost(int id, string currentUserId);
        Task<int> GetCountPublicBuyMarketPosts();
        bool IsHavePermissionToEditMarketPost(int postId, string userId);
        Task<bool> RemoveUploadedImage(int id);
        Task<int> UpdateBuyLot(BuyItemMarketPostViewModel model, string companyId, List<IFormFile> files, string localDate);
        Task<int> UpdateSellLot(SellItemMarketPostViewModel model, string companyId, List<IFormFile> files, string localDate);
        Task UpdateMarketPostById(int postId, string localDate, ConditionPost conditionPost);
        Task<List<ItemMarketPostShortViewModel>> GetUserItemsMarketPosts(UserMarketPostsFiltersViewModel filters, User user);
        Task<bool> AddViewToMarketPost(int postId, string userId);
        Task<(int, DateTime)> GetCountViewAndDateLastUpdateByUserId(string userId);
        Task<(int, DateTime)> GetCountViewAndDateLastUpdateBuy();
        Task<(int, DateTime)> GetCountViewAndDateLastUpdateSell();
        Task<(int, DateTime)> GetCountViewAndDateLastUpdateAll();
        Task<BuyItemMarketPostWithHistoryViewModel> GetBuyItemMarketPostWithHistory(int id, string currentUserId);
        HistoryMarketPostActionGroup GetHistoryForDate(DateTime date, int itemId);
        Task<SellItemMarketPostWithHistoryViewModel> GetSellItemMarketPostWithHistory(int id, string currentUserId);
    }
}
