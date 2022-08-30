using MDispatch.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace MDispatch.Service
{
    public class Inspection
    {
        public int ReCurentStatus(string token, string id, ref string description, string status)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/ReCurentStatus", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShip", id);
                request.AddParameter("status", status);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SendBolEmaile(string token, string idship, ref string description, string email)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Email/BOL", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShip", idship);
                request.AddParameter("email", email);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SendCouponEmaile(string token, ref string description, string email)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Email/Copon", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("email", email);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveSigPikedUp(string token, Photo photoSig, string idShip, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string sigPhoto = JsonConvert.SerializeObject(photoSig);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/SaveSigPikedUp", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShip", idShip);
                request.AddParameter("jsonSigPhoto", Compress(sigPhoto));
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int GetShipping(string token, string id, ref string description, ref Shipping shipping)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Shipping", Method.POST);
                client.Timeout = 600000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShip", id);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(UnCompress(content), ref description, ref shipping);
            }
        }

        internal int InspectionStatus(string token, string idShipping, string statusInspection, ref string description, ref Shipping shipping)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest($"Mobile/Status/Inspection/{statusInspection}", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShipping", idShipping);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(UnCompress(content), ref description, ref shipping);
            }
        }

        public int GetShippingPhoto(string token, string id, ref string description, ref Shipping shipping)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/ShippingPhoto", Method.POST);
                client.Timeout = 600000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShip", id);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(UnCompress(content), ref description, ref shipping);
            }
        }

        public int SaveAsk(string token, string id, Ask ask, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonAsk = JsonConvert.SerializeObject(ask);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStrAsk", Compress(strJsonAsk));
                request.AddParameter("type", 1);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception e)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveAsk(string token, string id, Ask2 ask, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonAsk = JsonConvert.SerializeObject(ask);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStrAsk", Compress(strJsonAsk));
                request.AddParameter("type", 6);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception e)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveInTruck(string token, string idVe, List<Photo> photo, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonPhoto = JsonConvert.SerializeObject(photo);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVe", idVe);
                request.AddParameter("jsonStrAsk", Compress(strJsonPhoto));
                request.AddParameter("type", 7);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception e)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveStrap(string token, string id, List<Photo> photo, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonPhoto = JsonConvert.SerializeObject(photo);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStrAsk", Compress(strJsonPhoto));
                request.AddParameter("type", 8);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception e)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveAsk(string token, Feedback feedback, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonAsk = JsonConvert.SerializeObject(feedback);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/FeedBack", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("jsonStrAsk", Compress(strJsonAsk));
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public Feedback GetFeedback(string token, string shippingId)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Get/FeedBack", Method.GET);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("shippingId", shippingId);
                response = client.Execute(request);
                content = response.Content;
                HTTPResponse httpResponse = JsonConvert.DeserializeObject<HTTPResponse>(content);
                return new Feedback();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public int SaveAsk(string token, string id, Ask1 ask1, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonAsk = JsonConvert.SerializeObject(ask1);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 100000;
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStrAsk", Compress(strJsonAsk));
                request.AddParameter("type", 2);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception e)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveAsk(string token, string id, AskFromUser askForUser, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonAsk = JsonConvert.SerializeObject(askForUser);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStrAsk", Compress(strJsonAsk));
                request.AddParameter("type", 3);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveAsk(string token, string id, AskDelyvery askDelyvery, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonAsk = JsonConvert.SerializeObject(askDelyvery);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 60000;
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStrAsk", Compress(strJsonAsk));
                request.AddParameter("type", 4);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        internal int SetInstaraction(string token, string idShiping, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Instraction", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 60000;
                request.AddParameter("token", token);
                request.AddParameter("idShiping", idShiping);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int CheckProblem(string token, string idShiping, ref bool isProplem)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Check/Problem", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 20000;
                request.AddParameter("token", token);
                request.AddParameter("idShiping", idShiping);
                request.AddParameter("type", "");
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                string description = null;
                return GetData(content, ref isProplem, ref description);
            }
        }

        public int SetProblem(string token, string idShiping)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Problem", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 60000;
                request.AddParameter("token", token);
                request.AddParameter("idShiping", idShiping);
                request.AddParameter("type", "");
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                string description = null;
                return GetData(content, ref description);
            }
        }

        public int SaveAsk(string token, string id, AskForUserDelyveryM askForUserDelyveryM, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string strJsonAsk = JsonConvert.SerializeObject(askForUserDelyveryM);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Ansver", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 60000;
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStrAsk", Compress(strJsonAsk));
                request.AddParameter("type", 5);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        [Obsolete]
        public int SavePhoto(string token, string id, PhotoInspection photoInspection, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                if (photoInspection.Damages != null)
                {
                    photoInspection.Damages.ForEach((dm) =>
                    {
                        dm.Image = null;
                        dm.ImageSource = null;
                    });
                }
                string strPhotoInspection = JsonConvert.SerializeObject(photoInspection);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Photo", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVe", id);
                request.AddParameter("jsonStr", Compress(strPhotoInspection));
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception e)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        protected byte[] OptimizeNResize(Bitmap original_image, double width, double heidth)
        {
            MemoryStream memoryStream = new MemoryStream();
            Bitmap final_image = null;
            Graphics graphic = null;
            int reqW = (int)width;
            int reqH = (int)heidth;
            final_image = new Bitmap(reqW, reqH);
            graphic = Graphics.FromImage(final_image);
            graphic.FillRectangle(new SolidBrush(System.Drawing.Color.Transparent),
                new System.Drawing.Rectangle(0, 0, reqW, reqH));
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic; 
            graphic.DrawImage(original_image, 0, 0, reqW, reqH);
            final_image.Save(memoryStream, ImageFormat.Jpeg);
            if (graphic != null) graphic.Dispose();
            if (original_image != null) original_image.Dispose();
            if (final_image != null) final_image.Dispose();
            return memoryStream.ToArray();
        }

        public int SaveDamageForuser(string token, string idVech, string idShiping, List<DamageForUser> damageForUsers, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                if (damageForUsers != null)
                {
                    damageForUsers.ForEach((dm) =>
                    {
                        dm.Image = null;
                        dm.ImageSource = null;
                    });
                }
                string strDamageForUsers = JsonConvert.SerializeObject(damageForUsers);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Damages/User", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVech", idVech);
                request.AddParameter("idShiping", idShiping);
                request.AddParameter("damageForUserJson", Compress(strDamageForUsers));
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SavePhotPay(string token, string idShiping, int type, Photo photo, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string photojson = JsonConvert.SerializeObject(photo);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Pay", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShiping", idShiping);
                request.AddParameter("type", type);
                request.AddParameter("Photo", Compress(photojson));
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveVideoRecount(string token, string idShiping, int type, Video video, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string videojson = JsonConvert.SerializeObject(video);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/Recount", Method.POST);
                client.Timeout = 600000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShiping", idShiping);
                request.AddParameter("type", type);
                request.AddParameter("video", Compress(videojson));
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        public int SaveMethodPay(string token, string idShiping, string payMethod, string countPay, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Save/PickedUp/PayMethod", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idShiping", idShiping);
                request.AddParameter("payMethod", payMethod);
                request.AddParameter("countPay", countPay);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description);
            }
        }

        private int GetData(string respJsonStr, ref string description)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            description = responseAppS.Value<string>("Description");
            if (status == "success")
            {
                return 3;
            }
            else if (status == "NotAuthorized")
            {
                return 1;
            }
            else
            {
                description = responseAppS
                    .Value<string>("description");
                return 2;
            }
        }

        private int GetData(string respJsonStr, ref bool isProplem, ref string description)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            description = responseAppS.Value<string>("Description");
            if (status == "success")
            {
                isProplem = JsonConvert.DeserializeObject<bool>(responseAppS.
                          SelectToken("ResponseStr").ToString().ToLower());
                return 3;
            }
            else if (status == "NotAuthorized")
            {
                return 1;
            }
            else
            {
                description = responseAppS
                    .Value<string>("description");
                return 2;
            }
        }

        private int GetData(string respJsonStr, ref string description, ref Shipping shipping)
        {
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            description = responseAppS.Value<string>("Description");
            if (status == "success")
            {
                shipping = JsonConvert.DeserializeObject<Shipping>(responseAppS.
                        SelectToken("ResponseStr").ToString());
                return 3;
            }
            else
            {
                description = responseAppS
                    .Value<string>("description");
                return 2;
            }
        }

        private int GetData(string respJsonStr, ref string description, ref List<Shipping> shippings)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                shippings = JsonConvert.DeserializeObject<List<Shipping>>(responseAppS.
                        SelectToken("ResponseStr").ToString());
                description = responseAppS
                    .Value<string>("Description");
                return 3;
            }
            else if (status == "NotAuthorized")
            {
                description = responseAppS
                    .Value<string>("Description");
                return 1;
            }
            else
            {
                description = responseAppS
                    .Value<string>("Description");
                return 2;
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void EncodeJPEG(List<Photo> photos)
        {
            if (photos != null)
            {
                photos.ForEach((dm) =>
                {
                    Bitmap bmp1 = new Bitmap(new MemoryStream(Convert.FromBase64String(dm.Base64)));
                    ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 0L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    MemoryStream memoryStream = new MemoryStream();
                    bmp1.Save(memoryStream, jgpEncoder, myEncoderParameters);
                    dm.Base64 = Convert.ToBase64String(memoryStream.ToArray());
                });
            }
        }

        private string UnCompress(string dataStr)
        {
            dataStr = dataStr.Replace("\"", "");
            byte[] data = Convert.FromBase64String(dataStr);
            string res = null;
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);

                res = Encoding.UTF8.GetString(resultStream.ToArray());
            }
            return res;
        }

        private string Compress(string dataStr)
        {
            string res = null;
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gz = new GZipStream(ms, CompressionLevel.Optimal, true))
                {
                    gz.Write(data, 0, data.Length);
                }
                res = Convert.ToBase64String(ms.ToArray());
            }
            return res;
        }
    }
}