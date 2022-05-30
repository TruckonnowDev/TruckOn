using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace MDispatch.Service.GoogleApi
{
    public class GoogleApiService : IGoogleApiService
    {
        public async Task<int> DetectPlate(string token, string image, string idDriver, string type, string plate)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("api.Vision/plate", Method.POST);
                client.Timeout = 150000;
                var compressedImage = Compress(image);
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
                request.AddParameter("image", compressedImage);
                request.AddParameter("idDriver", idDriver);
                request.AddParameter("type", type);
                response = await client.ExecuteAsync(request);
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
