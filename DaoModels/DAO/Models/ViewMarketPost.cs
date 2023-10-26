using System;

namespace DaoModels.DAO.Models
{
    public class ViewMarketPost
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int MarketPostId { get; set; }
        public MarketPost MarketPost { get; set; }
        public DateTime DateTimeAction { get; set; }
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
    }
}
