using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string Contact { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public int? FaxNumberId { get; set; }
        public PhoneNumber FaxNumber { get; set; }
        public string McNumber { get; set; }
        public string Instructions { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Price { get; set; }
        public decimal BrokerFee { get; set; }
        public string UrlRequest { get; set; }
        public int? DriverId { get; set; }
        public Driver Driver { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int CurrentStatusId { get; set; }
        public CurrentStatus CurrentStatus { get; set; }
        public int? PickedUpId { get;set; }
        public AddressInformation PickedUp { get; set; }
        public DateTime DateTimePickedUp { get; set; }
        public int? DeliveryId { get; set; }
        public AddressInformation Delivery { get; set; }
        public DateTime DateTimeDelivery { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public DateTime DateTimePaid { get; set; }
        public DateTime DateTimeCancelOrder { get; set; }
        public ICollection<DamageForUser> DamagesForUser { get; set; }
        public ICollection<VehicleDetails> Vehicles { get; set; }
        public ICollection<HistoryOrderAction> HistoryOrderActions { get; set; }

    }
}
