﻿using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class TrailerGroup
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public ICollection<Trailer> Trailers { get; set; }
    }
}