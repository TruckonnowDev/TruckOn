using MDispatch.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace MDispatch.Service.OrderGet
{
    public class OrderGetService : IOrderGetService
    {
        public int ActiveOreder(string token, ref string description, ref List<Shipping> shippings)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/ActiveOreder", Method.POST);
                client.Timeout = 100000;

                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
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
                return GetData(UnCompress(content), ref description, ref shippings);
            }
        }

        public int DelyveryOreder(string token, ref string description, ref List<Shipping> shippings)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/DelyveryOreder", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
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
                return GetData(UnCompress(content), ref description, ref shippings);
            }
        }

        public int ArchiveOreder(string token, ref string description, ref List<Shipping> shippings)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/OrderArchiveGet", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
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
                return GetData(UnCompress(content), ref description, ref shippings);
            }
        }

        public int GetVehiclwInformation(string token, int idVech, ref string description, ref VehiclwInformation vehiclwInformation)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/GetVechicleInffo", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("idVech", idVech);
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
                return GetData(content, ref description, ref vehiclwInformation);
            }
        }

        public int Save(string token, string id, string idOrder, string name, string contactName, string address,
            string city, string state, string zip, string phone, string email, string typeSave, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/SaveOrder", Method.POST);
                client.Timeout = 10000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("id", id);
                request.AddParameter("idOrder", idOrder);
                request.AddParameter("name", name);
                request.AddParameter("contactName", contactName);
                request.AddParameter("address", address);
                request.AddParameter("city", city);
                request.AddParameter("state", state);
                request.AddParameter("zip", zip);
                request.AddParameter("phone", phone);
                request.AddParameter("email", email);
                request.AddParameter("typeSave", typeSave);
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

        public int Save(string token, string id, string typeSave, string payment, string paymentTeams, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/SaveOrder1", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("id", id);
                request.AddParameter("payment", payment);
                request.AddParameter("paymentTeams", paymentTeams);
                request.AddParameter("typeSave", typeSave);
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

        private int GetData(string respJsonStr, ref string description, ref List<Shipping> shippings)
        {
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

        private int GetData(string respJsonStr, ref string description, ref VehiclwInformation vehiclwInformation)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                vehiclwInformation = JsonConvert.DeserializeObject<VehiclwInformation>(responseAppS.
                        SelectToken("ResponseStr").ToString());
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
                    .Value<string>("description");
                return 2;
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
    }
}
