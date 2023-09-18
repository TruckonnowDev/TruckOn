using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class CommentMarketPost
    {
        public int Id { get;set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int MarketPostId { get; set; }
        public MarketPost MarketPost { get; set; }
        public DateTime DateTimeAction { get; set; }
        public string Message { get; set; }
    }
}
