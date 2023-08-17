using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class PhoneNumber
    {
        public int Id { get;set; }
        public string Iso2 { get;set; }
        public int DialCode { get;set; }
        public string Name { get;set; }
        public string Number { get;set; }
    }
}
