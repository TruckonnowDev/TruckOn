using System;
using System.IO;

namespace ApiMobaileServise.Servise
{
    public static class Config
    {
        public static string Url { get; set; } = "http://212.224.113.5:8099"; 
       // public static string Url { get; set; } = "http://dev.api.truckonnow.com"; 


        public static string UrlAdmin { get; set; } = "http://truckonnow.com";
       // public static string UrlAdmin { get; set; } = "http://dev.truckonnow.com";
        public static string AuchGoogleCloud 
        {
            get
            {
                string path = Path.GetFullPath("../AuchConfig/json.txt");
                return path;
            }
        }
    }
}
