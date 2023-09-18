using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class CheckoutMP
    {
        public int Id { get; set; }
        public int MarketPostId { get; set; }
        public MarketPost MarketPost { get; set; }
        public DateTime DateTimeAction { get; set; }
        public double Price { get; set; }
        public string BuyerId { get; set; }
        public User Buyer { get; set; }
        public string SellerId { get; set; }
        public User Seller { get; set; }
    }
}
