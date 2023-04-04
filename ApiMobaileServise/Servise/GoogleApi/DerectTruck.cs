using System.Collections.Generic;
using System.Drawing;
using ApiMobaileServise.Servise;
using DaoModels.DAO.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using Google.Api.Gax.Grpc;

namespace ApiMobaileServise.Servise.GoogleApi
{
    public class DerectTruck : IDetect
    {
        private SqlCommandApiMobile sqlCommandApiMobil = null;

        public void AuchGoole(SqlCommandApiMobile sqlCommandApiMobil)
        {  
            this.sqlCommandApiMobil = sqlCommandApiMobil;
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Config.AuchGoogleCloud);
        }

        public string DetectText(params object[] parames)
        {
            string plate = "";
            try
            {
                byte[] photo = (byte[])parames[1];
                string idDriver = (string)parames[0];
                List<Truck> trucks = sqlCommandApiMobil.GetTrucks();
                var client = ImageAnnotatorClient.Create();
                var image = Google.Cloud.Vision.V1.Image.FromBytes(photo);
                var response = client.DetectText(image);
                var response3 = client.DetectLocalizedObjects(image);

                //foreach (var localizedObject in response3)
                //{
                    string numPlateTmp = "";
                    Truck truck = null;
                    foreach (EntityAnnotation text in response)
                    {
                        if (trucks.FirstOrDefault(t => t.PlateTruk == text.Description) != null)
                        {
                            truck = trucks.FirstOrDefault(t => t.PlateTruk == text.Description);
                            //sqlCommandApiMobil.SetPlateTruck(truck.Id, idDriver);
                            plate = truck.PlateTruk;
                            break;
                        }
                        else if (truck != null && truck.PlateTruk.Contains(text.Description))
                        {
                            numPlateTmp += " " + text.Description;
                        }
                        else if (truck == null && trucks.FirstOrDefault(t => t.PlateTruk.Contains(text.Description)) != null)
                        {
                            numPlateTmp +=  text.Description;
                            truck = trucks.FirstOrDefault(t => t.PlateTruk.Contains(text.Description));
                        }
                        if (numPlateTmp.Length >= 6)
                        {
                            if (truck != null && truck.PlateTruk == numPlateTmp)
                            {
                                //sqlCommandApiMobil.SetPlateTruck(truck.Id, idDriver);
                                plate = truck.PlateTruk;
                                numPlateTmp = "";
                                break;
                            }
                            else if (truck.PlateTruk.Remove(truck.PlateTruk.Length - 3) == numPlateTmp || truck.PlateTruk.Remove(truck.PlateTruk.Length - 2) == numPlateTmp || truck.PlateTruk.Remove(truck.PlateTruk.Length - 1) == numPlateTmp)
                            {
                                //sqlCommandApiMobil.SetPlateTruck(truck.Id, idDriver);
                                plate = truck.PlateTruk;
                                numPlateTmp = "";
                                break;
                            }
                            else if (numPlateTmp.Length > 7)
                            {
                                truck = null;
                                numPlateTmp = "";
                            }
                        }

                    }
                    if (plate == "")
                    {
                        plate = numPlateTmp;
                    }
                //}
            }
            catch (Exception e)
            {
                throw e;
            }

            return plate;
        }
    }
}