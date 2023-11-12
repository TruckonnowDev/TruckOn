using DaoModels.DAO.Enum;
using System;
using WebDispacher.ViewModels.History;

namespace WebDispacher.ViewModels.Marketplace
{
    public class HistoryMarketPostActionViewModel
    {
        public int Id { get; set; }
        public int MarketPostId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; } = "Not Select";
        public string UserId { get; set; }
        public string CurrentUserName { get; set; } = "Not Select";
        public ActionType ActionType { get; set; }
        public string ChangedField { get; set; }
        public string ContentBefore { get; set; }
        public string ContentAfter { get; set; }
        public DateTime DateTimeAction { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
        public UserAgentInfoViewModel UserAgentInfoViewModel { get; set; }
        public IPInfoViewModel IPInfoViewModel { get; set; }
    }
}