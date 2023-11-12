using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class HistoryTrailerAction
    {
        public int Id { get; set; }
        public int TrailerId { get; set; }
        public string AuthorId { get; set; }
        public string UserId { get; set; }
        public ActionType ActionType { get; set; }
        public string ChangedField { get; set; }
        public string ContentBefore { get; set; }
        public string ContentAfter { get; set; }
        public DateTime DateTimeAction { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }
    }
}