using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using System;

namespace WebDispacher.ViewModels.Order
{
    public class HistoryOrderActionWithVehicleViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? VehicleId { get; set; }
        public VehicleDetails VehicleDetails { get; set; }
        public ActionType ActionType { get; set; }
        public string FieldAction { get; set; }
        public string ContentFrom { get; set; }
        public string ContentTo { get; set; }
        public DateTime DateTimeAction { get; set; }
    }
}
