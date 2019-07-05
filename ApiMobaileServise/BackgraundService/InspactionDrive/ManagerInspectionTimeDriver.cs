﻿using ApiMobaileServise.Servise;
using DaoModels.DAO.Models;
using FluentScheduler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiMobaileServise.BackgraundService.InspactionDrive
{
    public class ManagerInspectionTimeDriver : IJob
    {
        SqlCommandApiMobile sqlCommandApiMobile = null;
        private WebRequest tRequest = null;

        public void Execute()
        {
            InitReqvest();
            sqlCommandApiMobile = new SqlCommandApiMobile();
            List<Driver> drivers = sqlCommandApiMobile.GetDriverInDb();
            Task.Run(() => RefreshInspectionTimeDriver(drivers));
        }

        private async void RefreshInspectionTimeDriver(List<Driver> drivers)
        {
            foreach (var driver in drivers)
            {
                if (!driver.IsInspectionToDayDriver)
                {
                    await sqlCommandApiMobile.RefreshInspectionDriverInDb(driver.Id);
                    SendNotyfyInspactionDrive(driver.TokenShope, "Truck Inspection", "Immediately go truck inspection or else you will not be able to continue working");
                }
            }
        }

        private void InitReqvest()
        {
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAACa2vxR0:APA91bGTTHJgDmirQgd92-snbn5eixwi-sEPufe8fpl6EojstTcNNMjRnod7nAdUOw0C6InZvWOvom1xlRiWbojN7ObxGTeEPhjBtZ53ac2RLzIVuZc9_AdEkuix-vlul_ylJV7_ctEK"));
            tRequest.Headers.Add(string.Format("Sender: id={0}", "41568683293"));
            tRequest.ContentType = "application/json";
        }

        public void SendNotyfyInspactionDrive(string tokenShope, string title, string body)
        {
            if (tokenShope != null && tokenShope != "")
            {
                var payload = new
                {
                    to = tokenShope,
                    content_available = true,
                    notification = new
                    {
                        click_action = "Driver",
                        body = body,
                        title = title
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
            }
            InitReqvest();
        }
    }
}