using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace MDispatch.Service
{
    public class GoogleApi
    {

        internal int DetectPlate(string token, string image, string idDriver, string type, ref string plate)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                var res1 = Compress(image);
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("api.Vision/plate", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("image", Compress(image));
                request.AddParameter("idDriver", idDriver);
                request.AddParameter("type", type);
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
                return GetData(content, ref plate);
            }
        }

        private int GetData(string respJsonStr, ref string plate)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                plate = responseAppS
                    .Value<string>("ResponseStr");
                return 3;
            }
            else if (status == "NotAuthorized")
            {
                return 1;
            }
            else
            {
                return 2;
            }
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