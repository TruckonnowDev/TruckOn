using SQLite;

namespace MDispatch.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string path { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Base64 { get; set; }
    }
}