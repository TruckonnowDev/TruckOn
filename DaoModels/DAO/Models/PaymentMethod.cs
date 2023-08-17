namespace DaoModels.DAO.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string PaymentMethodSTId { get; set; }
        public string CustomerAttachPaymentMethodId { get; set; }
        public bool IsDefault { get; set; }
    }
}