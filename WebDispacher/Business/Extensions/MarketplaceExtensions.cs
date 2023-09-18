using DaoModels.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebDispacher.ViewModels.Marketplace;

namespace WebDispacher.Business.Extensions
{
    public static class MarketplaceExtensions
    {
        public static async Task<List<BuyItemMarketPostShortViewModel>> ProjectToBuyItemMarketPostShort(this IQueryable<BuyItemMarketPost> query)
        {
            return await query.Select(u => new BuyItemMarketPostShortViewModel
            {
                Id = u.Id,
                Title = u.Title,
                Description = u.Description,
                UserId = u.MarketPost.UserId,
                User = u.MarketPost.User,
                ConditionPost = u.MarketPost.ConditionPost,
                ShowView = u.MarketPost.ShowView,
                ShowComment = u.MarketPost.ShowComment,
                DateTimeLastUpdate = u.MarketPost.DateTimeLastUpdate
            }).ToListAsync();
        }
    }
}
