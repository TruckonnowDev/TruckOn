using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class VideoType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public ICollection<Video> Videos { get; set; }
    }
}
