using Newtonsoft.Json.Linq;
using RestSharp;
using System;

namespace MDispatch.Service.Auth
{
    public class Auth : IAuth
    {
        public int Avthorization(string login, string password, ref string description, ref string token)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Avtorization", Method.POST);
                client.Timeout = 20000;
                request.AddParameter("email", login);
                request.AddParameter("password", password);
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
                return GetData(content, ref description, ref token); ;
            }
        }

        public int ClearAvt(string token)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Avtorization/Clear", Method.POST);
                client.Timeout = 10000;
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
                return 3;
            }
        }

        public int RequestPasswordChanges(string login, string password, ref string description)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/RequestPassword", Method.POST);
                client.Timeout = 20000;
                request.AddParameter("email", login);
                request.AddParameter("fullName", password);
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
                return GetData(content, ref description); ;
            }
        }

        private int GetData(string respJsonStr, ref string description)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                return 3;
            }
            else
            {
                description = responseAppS
                    .Value<string>("Description");
                return 2;
            }
        }

        private int GetData(string respJsonStr, ref string description, ref string token)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                token = responseAppS
                    .Value<string>("ResponseStr");
                return 3;
            }
            else
            {
                description = responseAppS
                    .Value<string>("Description");
                return 2;
            }
        }
    }
}
