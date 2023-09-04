using DaoModels.DAO.Enum;

namespace DaoModels.DAO.Models
{
    public class SubscribeST
    {
        public int Id { get; set; }
        public int CustomerSTId { get; set; }
        public CustomerST CustomerST { get; set; }
        public string SubscribeSTId { get; set; }
        public string ItemSubscribeSTId { get; set; }
        public SubscribeStatus Status { get; set; }
        public ActiveType ActiveType { get; set; }
    }
}
