using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class TruckStatus : Status
    {
        public TruckStatusType TruckStatusType { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public int? StatusThemeId { get; set; }
        public TruckStatusTheme StatusTheme { get; set; }
        public ICollection<Truck> Trucks { get; set; }
        public ICollection<TruckStatusWidget> Widgets { get; set; }
    }
}