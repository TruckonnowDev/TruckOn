using System.Collections;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class PhotoType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}
