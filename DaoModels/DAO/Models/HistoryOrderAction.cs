using DaoModels.DAO.Enum;
using System;

namespace DaoModels.DAO.Models
{
    public class HistoryOrderAction
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? VehicleId { get; set; }
        public ActionType ActionType { get; set; }
        public string FieldAction { get; set; }
        public string ContentFrom { get; set; }
        public string ContentTo { get; set; }
        public DateTime DateTimeAction { get; set; }
    }
}
