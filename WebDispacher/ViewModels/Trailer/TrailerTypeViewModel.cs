using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerTypeViewModel
    {
        public int Id { get; set; }

        [History]
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}