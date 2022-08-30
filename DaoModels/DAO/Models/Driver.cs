using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string EmailOrLogin { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string TrailerCapacity { get; set; }
        public string DriversLicenseNumber { get; set; }
        public string IssuingState_Province { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string TokenShope { get; set; }
        public bool IsInspectionDriver { get; set; }
        public bool IsInspectionToDayDriver { get; set; }
        public Geolocations geolocations { get; set; }
        public virtual List<InspectionDriver> InspectionDrivers { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public bool IsFired { get; set; }
        public string LastDateInspaction { get; set; }
        public string DateRegistration { get; set; }
    }
}