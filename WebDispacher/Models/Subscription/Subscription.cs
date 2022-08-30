namespace WebDispacher.Models.Subscription
{
    public class Subscription
    {
        public int Id { get; set; }
        public string IdSubscriptionST { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int PeriodDays { get; set; }
    }
}