using System.ComponentModel.DataAnnotations;

namespace DaoModels.DAO.Models
{
    public class AddressInformation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
