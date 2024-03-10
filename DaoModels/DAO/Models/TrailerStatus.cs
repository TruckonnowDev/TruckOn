using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class TrailerStatus : Status
    {
        public TrailerStatusType TrailerStatusType { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public int? StatusThemeId { get; set; }
        public TrailerStatusTheme StatusTheme { get; set; }
        public ICollection<Trailer> Trailers { get; set; }
    }
}