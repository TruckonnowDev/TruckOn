using DaoModels.DAO.Enum;

namespace DaoModels.DAO.Models
{
    public class Subscribe_ST
    {
        public int Id { get; set; }
        public int IdCompany { get; set; }
        public string IdCustomerST { get; set; }
        public string IdSubscribeST { get; set; }
        public string IdItemSubscribeST { get; set; }
        public string Status { get; set; }
        public ActiveType ActiveType { get; set; }
    }
}