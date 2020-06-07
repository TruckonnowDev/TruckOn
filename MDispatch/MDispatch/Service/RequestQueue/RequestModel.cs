﻿using System.Collections.Generic;

namespace MDispatch.Service.RequestQueue
{
    public class RequestModel
    {
        public string NameRequvest { get; set; }
        public string Token { get; set; }
        public List<dynamic> ParamsRequvest { get; set; }
        public bool IsWork { get; set; } = false;
    }
}