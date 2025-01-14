﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMobaileServise.Models
{
    public class ResponseAppS
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public object ResponseStr { get; set; }
        public object ResponseStr1 { get; set; }
        public object ResponseStr2 { get; set; }
        public object ResponseStr3 { get; set; }

        public ResponseAppS(string status, string description, object responseStr, object responseStr1 = null, object responseStr2 = null, object responseStr3 = null)
        {
            Status = status;
            Description = description;
            ResponseStr = responseStr;
            ResponseStr1 = responseStr1;
            ResponseStr2 = responseStr2;
            ResponseStr3 = responseStr3;
        }
    }
}