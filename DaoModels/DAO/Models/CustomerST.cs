using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class CustomerST
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string NameCompanyST { get; set; }
        public string IdCustomerST { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<SubscribeST> Subscribes { get; set; }
    }
}
