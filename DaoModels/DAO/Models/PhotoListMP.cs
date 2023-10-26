using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class PhotoListMP
    {
        public int Id { get; set; }
        public List<PhotoMP> Photos { get; set; }
    }
}
