using DaoModels.DAO.Interface;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string HowLong { get; set; }
        public string Vin { get; set; }
        public string Owner { get; set; }
        public string Color { get; set; }
        public string Plate { get; set; }
        public string Exp { get; set; }
        public string AnnualIns { get; set; }
        public string Type { get; set; }
    }
}