using ApiMobaileServise.Attribute;
using ApiMobaileServise.Models;
using ApiMobaileServise.Servise;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace ApiMobaileServise.Controllers
{
    [Route("api.Vision")]
    public class VisionGoogleController : Controller
    {
        ManagerMobileApi managerMobileApi = new ManagerMobileApi();

        [HttpPost]
        [Route("plate")]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "image")]
        public string CheckInspecktionDriver(string token, string image, string idDriver, string type)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                //bool isToken = managerMobileApi.CheckToken(token);
                bool isToken = true;
                if (isToken)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", managerMobileApi.GetPlateNumber(image, idDriver, type)));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
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