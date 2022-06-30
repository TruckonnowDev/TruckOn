using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ApiMobaileServise.Attribute;
using ApiMobaileServise.BackgraundService.Queue;
using ApiMobaileServise.Models;
using ApiMobaileServise.Servise;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiMobaileServise.Controllers
{
    [Route("Mobile")]
    public class InspectionController : Controller
    {
        ManagerMobileApi managerMobileApi = new ManagerMobileApi();

        [HttpPost]
        [Route("Status/Inspection/Assigned")]
        [CompressGzip(IsCompresRespons = true)]
        public string GetStatusInspectionPikedUp(string token, string idShipping)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    Shipping shipping = managerMobileApi.GetStatusInspectionPickedUp(idShipping);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", shipping));
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

        [HttpPost]
        [Route("Status/Inspection/Picked up")]
        [CompressGzip(IsCompresRespons = true)]
        public string GetStatusInspectionDelyvery(string token, string idShipping)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    Shipping shipping = managerMobileApi.GetStatusInspectionDelivery(idShipping);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", shipping));
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

        [HttpPost]
        [Route("Status/Inspection/Delivered")]
        [CompressGzip(IsCompresRespons = true)]
        public string GetStatusInspectionEnd(string token, string idShipping)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    Shipping shipping = managerMobileApi.GetStatusInspectionEnd(idShipping);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", shipping));
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

        [HttpPost]
        [Route("Save/Ansver")]
        [DisableRequestSizeLimit]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "jsonStrAsk")]
        public async Task<string> SaveAnsver(string token, string idVe, string jsonStrAsk, int type)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    if(type == 1 || type == 2 || type == 3 || type == 4 || type == 5 || type == 7 || type == 8)
                    {
                        QueueWorkerAsk.queues.Add($"SaveAnsver&,&{idVe}&,&{type}&,&{jsonStrAsk}");
                        QueueWorkerAsk.countQueues++;
                    }
                    else
                    {
                        await managerMobileApi.SaveAsk(idVe, type, jsonStrAsk);
                    }
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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


        [HttpPost]
        [Route("SaveSigPikedUp")]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "jsonSigPhoto")]
        public async Task<string> SaveSigPikedUp(string token, string idShip, string jsonSigPhoto)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.SaveSigPhoto(idShip, jsonSigPhoto);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Shipping")]
        [CompressGzip(IsCompresRespons = true)]
        public string GetShipping(string token, string idShip)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    Shipping shipping = managerMobileApi.GetShipping(idShip);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", shipping));
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

        [HttpPost]
        [Route("ShippingPhoto")]
        [CompressGzip(IsCompresRespons = true)]
        public string GetShippingPhoto(string token, string idShip)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    Shipping shipping = managerMobileApi.GetShippingPhoto(idShip);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", shipping));
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

        [HttpPost]
        [Route("VehiclwPhoto")]
        [CompressGzip(IsCompresRespons = true)]
        public string GetVehiclwlPhoto(string token, string idVehiclw)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    VehiclwInformation vehiclwInformation = managerMobileApi.GetVehiclwPhoto(idVehiclw);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", vehiclwInformation));
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

        [HttpPost]
        [Route("ReCurentStatus")]
        public async Task<string> ReCurentStatus(string token, string idShip, string status)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.ReCurentStatus(idShip, status);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
                }
                else
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
                }
            }
            catch (Exception e)
            {
                System.IO.File.WriteAllText("ReCurentStatus.txt", e.Message);
                respons = JsonConvert.SerializeObject(new ResponseAppS("failed", "Technical work on the service", null));
            }
            return respons;
        }

        [HttpPost]
        [Route("Email/BOL")]
        public async Task<string> SendPdfToEmail(string token, string idShip, string email)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.SendBol(idShip, email);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Email/Copon")]
        public async Task<string> SendCoponToEmail(string token, string email)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.SendCoupon(email);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Save/FeedBack")]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "jsonStrAsk")]
        public async Task<string> SaveFeedBack(string token,string jsonStrAsk, string shippingId)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.SaveFeedBack(jsonStrAsk, shippingId);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Save/Photo")]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "jsonStr")]
        public async Task<string> SavePhoto(string token, string idVe, string jsonStr)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                   // managerMobileApi.SavePhotoInspection(idVe, jsonStr);

                    QueueWorkSavePhotoInspection.queues.Add($"SavePhotoInspection&,&{idVe}&,&{jsonStr}");
                    QueueWorkSavePhotoInspection.countQueues++;
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Damages/User")]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "damageForUserJson")]
        public async Task<string> AddDamageForUser(string token, string idVech, string idShiping, string damageForUserJson)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.SaveDamageForUser(idVech, idShiping, damageForUserJson);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Save/Pay")]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "Photo")]
        public async Task<string> SavePay(string token, string idShiping, int type, string Photo)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.SavePay(idShiping, type, Photo);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Save/Recount")]
        [CompressGzip(IsCompresReqvest = true, ParamUnZip = "video")]
        public async Task<string> SaveRecount(string token, string idShiping, int type, string video)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    //await managerMobileApi.SaveRecount(idShiping, type, video);
                    VideoSave.queues.Add($"SaveRecount&,&{idShiping}&,&{type}&,&{video}");
                    VideoSave.countQueues++;
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Save/PickedUp/PayMethod")]
        public async Task<string> SavePickedUpPayMethod(string token, string idShiping, string payMethod, string countPay)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    await managerMobileApi.SavePayMethot(idShiping, payMethod, countPay);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Problem")]
        public string SetProplem(string token, string idShiping, string type)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    managerMobileApi.SetProplem(idShiping, type);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpPost]
        [Route("Check/Problem")]
        public string CheckProplem(string token, string idShiping, string type)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", managerMobileApi.CheckProplem(idShiping, type)));
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

        [HttpPost]
        [Route("Instraction")]
        public string SetInstraction(string token, string idShiping)
        {
            string respons = null;
            if (token == null || token == "")
            {
                return JsonConvert.SerializeObject(new ResponseAppS("NotAuthorized", "Not Authorized", null));
            }
            try
            {
                bool isToken = managerMobileApi.CheckToken(token);
                if (isToken)
                {
                    managerMobileApi.SetInstraction(idShiping);
                    respons = JsonConvert.SerializeObject(new ResponseAppS("success", "", null));
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

        [HttpGet]
        [Route("Image")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetShiping(string name, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(name);
            return File(imageFileStream, $"image/{type}");
        }
    }
}