using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckTypeViewModel
    {
        public int Id { get; set; }

        [History]
        public string Name { get; set; }

        public string Slug { get; set; }
    }
}