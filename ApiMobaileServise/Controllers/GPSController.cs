﻿using System;
using System.Threading.Tasks;
using ApiMobaileServise.Models;
using ApiMobaileServise.Servise;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiMobaileServise.Controllers
{
    [Route("Mobile")]
    public class GPSController : Controller
    {
        private ManagerMobileApi managerMobileApi = new ManagerMobileApi();

        [HttpPost]
        [Route("GPS/Save")]
        public string GPSSave(string token, string longitude, string latitude)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("failed", "1", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {

                    Task.Run(() =>
                    {
                        managerMobileApi.SaveGPSLocationData(token, longitude, latitude);
                    });
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "2", null));
                }
            }
            catch (Exception)
            {
                respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Technical work on the service", null));
            }
            return respons;
        }
    }
}