namespace WebDispacher.ViewModels.History
{
    public class IPInfoViewModel
    {
        public string CountryName { get; set; }
        public string MostSpecificSubdivisionName { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public int? PostalCodeСonfidence { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? LocationAccuracyRadius { get; set; }
    }
}