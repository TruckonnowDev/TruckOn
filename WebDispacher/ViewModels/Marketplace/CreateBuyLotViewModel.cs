using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using System;

namespace WebDispacher.ViewModels.Marketplace
{
    public class CreateBuyLotViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ConditionPost ConditionPost { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ZipCode { get; set; }
        public bool ShowView { get; set; }
        public bool ShowComment { get; set; }
    }
}
