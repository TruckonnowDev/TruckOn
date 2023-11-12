using DaoModels.DAO.Models;
using System;
using System.Collections.Generic;
using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Order
{
    public class EditOrderViewModel
    {
        public int Id { get; set; }

        [History]
        public string OrderId { get; set; }

        [History]
        public string Contact { get; set; }

        public int? PhoneNumberId { get; set; }
        public EditPhoneNumberViewModel PhoneNumber { get; set; }

        public int? FaxNumberId { get; set; }

        public EditPhoneNumberViewModel FaxNumber { get; set; }

        [History]
        public string McNumber { get; set; }

        [History]
        public string Instructions { get; set; }

        [History]
        public string PaymentMethod { get; set; }

        [History]
        public decimal Price { get; set; }

        [History]
        public decimal BrokerFee { get; set; }
        public int CompanyId { get; set; }
        public DaoModels.DAO.Models.Company Company { get; set; }
        public int CurrentStatusId { get; set; }
        public CurrentStatus CurrentStatus { get; set; }
        public int? PickedUpId { get; set; }

        public EditAddressInformationViewModel PickedUp { get; set; }

        [History]
        public DateTime DateTimePickedUp { get; set; }

        public int? DeliveryId { get; set; }

        public EditAddressInformationViewModel Delivery { get; set; }

        [History]
        public DateTime DateTimeDelivery { get; set; }

        public List<VehicleDetails> Vehicles { get; set; }

        public DateTime DateTimeLastUpdate { get; set; }
    }
}
