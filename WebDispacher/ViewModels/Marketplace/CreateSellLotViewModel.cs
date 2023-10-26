using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using System;

namespace WebDispacher.ViewModels.Marketplace
{
    public class CreateSellLotViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ConditionItem ConditionItem { get; set; }
        public double Price { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ZipCode { get; set; }
        public bool ShowView { get; set; }
        public bool ShowComment { get; set; }
    }
}
