using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class MarketPost
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ConditionPost ConditionPost { get;set;}
        public bool ShowView { get; set; }
        public bool ShowComment { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public DateTime DateTimeCreate { get; set; }

    }
}
