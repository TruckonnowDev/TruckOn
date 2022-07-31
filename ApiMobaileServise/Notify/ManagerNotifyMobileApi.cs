using DaoModels.DAO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ApiMobaileServise.Notify
{
    public class ManagerNotifyMobileApi
    {
        private WebRequest tRequest = null;

        public ManagerNotifyMobileApi()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAvKIh8VA:APA91bHn3S6rP3yBFlmXA1xEVZtS0IM2Mby11shGOkNvhrCOmxNtiu92cLXcWViSwKvkbh5jxMr_XPJW0qoA4V_dE1XsSPXFZE97i9uosj1whRyN9dE3XUu8ieyWikqc7yljUz-TH-d7"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "810173985104"));
            tRequest.ContentType = "application/json";
        }

        private void InitReqvest()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAvKIh8VA:APA91bHn3S6rP3yBFlmXA1xEVZtS0IM2Mby11shGOkNvhrCOmxNtiu92cLXcWViSwKvkbh5jxMr_XPJW0qoA4V_dE1XsSPXFZE97i9uosj1whRyN9dE3XUu8ieyWikqc7yljUz-TH-d7"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "810173985104"));
            tRequest.ContentType = "application/json";
        }

        public void SendNotyfyStatusPickup(string idShip, string tokenShope)
        {
            if (tokenShope != null && tokenShope != "")
            {
                var payload = new
                {
                    to = tokenShope,
                    content_available = true,
                    notification = new
                    {
                        click_action = "No Action",
                        body = "After delivery, write down the time of arrival, then to indicate this in the inspection",
                        title = $"Orede Id: {idShip} on my way",
                        sound = "default",
                        badge = 1,
                    },
                };
                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                        }
                    }
                }
                InitReqvest();
            }
        }

        public void SendNotyfyStatusDelyvery(string idShip, string tokenShope, string body)
        {
            if (tokenShope != null && tokenShope != "")
            {
                var payload = new
                {
                    to = tokenShope,
                    content_available = true,
                    notification = new
                    {
                        click_action = "No Action",
                        body = body,
                        title = $"Orede Id: {idShip} delivered",
                        sound = "default",
                        badge = 1,
                    },
                };
                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                }
                        }
                    }
                }
                InitReqvest();
            }
        }
    }
}
