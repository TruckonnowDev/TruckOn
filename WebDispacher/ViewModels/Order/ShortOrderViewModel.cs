using DaoModels.DAO.Models;
using System;
using System.Collections.Generic;

namespace WebDispacher.ViewModels.Order
{
    public class ShortOrderViewModel
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int? DriverId { get; set; }
        public DaoModels.DAO.Models.Driver Driver { get; set; }
        public string Contact { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Price { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public AddressInformation PickedUp { get; set; }
        public List<OrderStatusWidget> Widgets { get; set; }
        public DateTime DateTimePickedUp { get; set; }
        public AddressInformation Delivery { get; set; }
        public DateTime DateTimeDelivery { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public DateTime? DateTimePaid { get; set; }
        public DateTime? DateTimeCancelOrder { get; set; }
    }
}
