using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using System;

namespace WebDispacher.ViewModels.Marketplace
{
    public class MarketPostViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ConditionPost ConditionPost { get; set; }
        public bool ShowView { get; set; }
        public bool ShowComment { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public DateTime DateTimeCreate { get; set; }
    }
}
