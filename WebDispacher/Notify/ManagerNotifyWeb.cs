using DaoModels.DAO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WebDispacher.Notify
{
    public class ManagerNotifyWeb
    {
        private WebRequest tRequest = null;

        public ManagerNotifyWeb()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAwfAhwuA:APA91bGEjY_-E4saZ4kv2GuiJzHFAzrON0i96akHnuHls0rOqCNEgwlo3KcvLCvTHcUDYeaJxCedVvs3OAx0h9dVn9SRxyUjJpD-qXqAT49XWwAHlhhIJeMMXW9T5koFiIo0a8Sw8qei"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "832957432544"));
            tRequest.ContentType = "application/json";
        }

        private void InitReqvest()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAwfAhwuA:APA91bGEjY_-E4saZ4kv2GuiJzHFAzrON0i96akHnuHls0rOqCNEgwlo3KcvLCvTHcUDYeaJxCedVvs3OAx0h9dVn9SRxyUjJpD-qXqAT49XWwAHlhhIJeMMXW9T5koFiIo0a8Sw8qei"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "832957432544"));
            tRequest.ContentType = "application/json";
        }

        public void SendNotyfyAssign(int orderId, string tokenShope, List<VehicleDetails> vehiclwInformations)
        {
            string body = null;
            if (tokenShope != null && tokenShope != "")
            {
                foreach (var vech in vehiclwInformations)
                {
                    body += $"{vech.VehicleModel.Year} {vech.VehicleModel.VehicleBrand.Name} {vech.VehicleModel.Name}\n";
                }
                var payload = new
                {
                    to = tokenShope,
                    content_available = true,
                    notification = new
                    {
                        click_action = "Oreder",
                        body = $"{body}",
                        title = $"New Load Order Id: {orderId} Assign",
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

        public void SendNotyfyUnassign(int orderId, string tokenShope, List<VehicleDetails> vehiclwInformations)
        {
            string body = null;
            if (tokenShope != null && tokenShope != "")
            {
                foreach (var vech in vehiclwInformations)
                {
                    body += $"{vech.VehicleModel.Year} {vech.VehicleModel.VehicleBrand.Name} {vech.VehicleModel.Name}\n";
                }
                var payload = new
                {
                    to = tokenShope,
                    content_available = true,
                    notification = new
                    {
                        click_action = "Oreder",
                        body = $"{body}",
                        title = $"Unassign Load Order Id: {orderId}",
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

        public void SendSendNotyfyRemindInspection(string tokenShope)
        {
            string body = null;
            if (tokenShope != null && tokenShope != "")
            {
                body = "Please go through the truck and trailer inspection";
                var payload = new
                {
                    to = tokenShope,
                    content_available = true,
                    notification = new
                    {
                        click_action = "Oreder",
                        body = $"{body}",
                        title = "Inspection truck and trailer",
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