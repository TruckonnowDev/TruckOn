namespace WebDispacher.Models
{
    public class OrderModel
    {
        public string idOrder { get; set; }
        public string CurrentStatus { get; set; }
        public string NameP { get; set; }
        public string ContactNameP { get; set; }
        public string Address { get; set; }
        public string CityP { get; set; }
        public string StateP { get; set; }
        public string ZipP { get; set; }
        public string PhoneP { get; set; }
        public string EmailP { get; set; }
        public string PickupExactly { get; set; }
        public string Instructions { get; set; }
        public string NameD { get; set; }
        public string ContactNameD { get; set; }
        public string AddressD { get; set; }
        public string CityD { get; set; }
        public string StateD { get; set; }
        public string ZipD { get; set; }
        public string PhoneD { get; set; }
        public string EmailD { get; set; }
        public string TotalPaymentToCarrier { get; set; }
        public string PriceListed { get; set; }
        public string BrokerFee { get; set; }
    }
}