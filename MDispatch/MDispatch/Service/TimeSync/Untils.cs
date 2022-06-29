using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MDispatch.Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using RestSharp;

namespace MDispatch.Service.TimeSync
{
    public class Untils
    {

        private static Timer timer = null;
        private static bool isUpdate { get; set; }

        public static void Start()
        {
            if (!isUpdate)
            {
                timer = new Timer(new TimerCallback(SyncServer), null, 3000, 60000);
                isUpdate = true;
            }
        }

        private static async void SyncServer(object state)
        {
            //await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                DateTime dateTime = new DateTime();
                IRestResponse response = null;
                string content = null;
                try
                {
                    string token = CrossSettings.Current.GetValueOrDefault("Token", "");
                    RestClient client = new RestClient(Config.BaseReqvesteUrl);
                    RestRequest request = new RestRequest("Mobile/Sync", Method.GET);
                    request.AddHeader("Accept", "application/json");
                    client.Timeout = 10000;
                    request.AddParameter("token", token);
                    response = client.Execute(request);
                    content = response.Content;
                    if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        App.isNetwork = false;
                    }
                    else
                    {
                        GetData(content, ref dateTime);
                        string timeZpneCount = NodaTime.DateTimeZoneProviders.Tzdb.GetSystemDefault().MaxOffset.ToString();
                        dateTime = dateTime.AddHours(Convert.ToInt32(timeZpneCount));
                        App.Time = dateTime;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static void Stop()
        {
            if(isUpdate)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                isUpdate = false;
            }
        }

        private static void GetData(string respJsonStr, ref DateTime dateTime)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                string date_Time = responseAppS.
                        SelectToken("ResponseStr").ToString();
                string date = date_Time.Remove(date_Time.IndexOf(" "));
                string time = date_Time.Remove(0, date_Time.IndexOf(" ")+1);
                dateTime = DateTime.Parse($"{GetDFormat(date)} {time}");
            }
        }

        private static string GetDFormat(string data)
        {
            DateTime date;
            if (DateTime.TryParseExact(data, "MM.dd.yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "dd.MM.yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "yyyy.MM.dd", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "MM-dd-yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "dd-MM-yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "yyyy-MM-dd", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "MM/dd/yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "dd/MM/yyyy", null, DateTimeStyles.None, out date))
            {
            }
            else if (DateTime.TryParseExact(data, "yyyy/MM/dd", null, DateTimeStyles.None, out date))
            {
            }
            return date.ToShortDateString();
        }
    }
}