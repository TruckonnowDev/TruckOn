using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class BuyItemMarketPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string ZipCode { get; set; }
        public int MarketPostId { get; set; }
        public MarketPost MarketPost { get; set; }
        public int PhotoListMPId { get; set; }
        public PhotoListMP PhotoListMP { get; set; }
    }
}
