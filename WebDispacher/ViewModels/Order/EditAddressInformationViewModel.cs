using DaoModels.DAO.Models;
using System.ComponentModel.DataAnnotations;
using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Order
{
    public class EditAddressInformationViewModel
    {
        public int Id { get; set; }

        [History]
        public string Name { get; set; }

        [History]
        public string ContactName { get; set; }

        [History]
        public string Address { get; set; }

        [History]
        public string City { get; set; }

        [MinLength(2)]
        [MaxLength(2)]
        [History]
        public string State { get; set; }

        [MinLength(5)]
        [MaxLength(5)]
        [History]
        public string ZipCode { get; set; }

        public int? PhoneNumberId { get; set; }
        public EditPhoneNumberViewModel PhoneNumber { get; set; }

        [History]
        public string PhoneCountryCode { get; set; }

        [History]
        public string Email { get; set; }
    }
}
