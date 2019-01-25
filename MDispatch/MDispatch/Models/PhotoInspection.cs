﻿using System.Collections.Generic;

namespace MDispatch.Models
{
    public class PhotoInspection
    {
        public int Id { get; set; }
        public int IndexPhoto { get; set; }
        public string CurrentStatusPhoto { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Damage> Damages { get; set; }
    }
}