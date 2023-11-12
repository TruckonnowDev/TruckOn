using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Marketplace
{
    public class SellItemMarketPostViewModel
    {
        public int Id { get; set; }

        [History]
        public string Title { get; set; }

        [History]
        public string Description { get; set; }

        [History]
        public double Price { get; set; }

        [History]
        public ConditionItem ConditionItem { get; set; }

        [History]
        public string Email { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        [History]
        public string ZipCode { get; set; }
        public int MarketPostId { get; set; }
        public MarketPostViewModel MarketPost { get; set; }
        public int PhotoListMPId { get; set; }
        public PhotoListMP PhotoListMP { get; set; }
    }
}
