using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using System;

namespace WebDispacher.ViewModels.Marketplace
{
    public class BuyItemMarketPostShortViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ConditionPost ConditionPost { get; set; }
        public bool ShowView { get; set; }
        public bool ShowComment { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
    }
}
