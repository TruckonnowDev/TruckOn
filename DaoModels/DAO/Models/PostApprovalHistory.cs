using DaoModels.DAO.Enum;
using System;

namespace DaoModels.DAO.Models
{
    public class PostApprovalHistory
    {
        public int Id { get; set; }
        public int MarketPostId { get; set; }
        public MarketPost MarketPost { get; set; }
        public string AdminName { get; set; }
        public string Conclusion { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public DateTime DateTimeAction { get; set; }
    }
}