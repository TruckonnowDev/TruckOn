using System.Collections.Generic;
using System.Threading.Tasks;
using WebDispacher.ViewModels.Marketplace;

namespace WebDispacher.Business.Interfaces
{
    public interface IMarketplaceService
    {
        Task<List<BuyItemMarketPostShortViewModel>> GetPublicBuyItemsMarketPosts(int page);
        Task CreateBuyLot(CreateBuyLotViewModel model, string companyId, string localDate);
    }
}
