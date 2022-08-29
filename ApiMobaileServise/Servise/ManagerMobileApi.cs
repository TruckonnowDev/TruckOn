using ApiMobaileServise.EmailSmtp;
using ApiMobaileServise.Models;
using ApiMobaileServise.Notify;
using ApiMobaileServise.Servise.AddDamage;
using ApiMobaileServise.Servise.GoogleApi;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMobaileServise.Servise
{
    public class ManagerMobileApi
    {
        SqlCommandApiMobile sqlCommandApiMobile = null;
          
        public ManagerMobileApi()
        {
            sqlCommandApiMobile = new SqlCommandApiMobile();
            CheckAndCreatedFolder();
        }

        public async Task SendBol(string idShip, string email)
        {
            Shipping shipping = sqlCommandApiMobile.SendBolInDb(idShip);
            string patern = new PaternSourse().GetPaternBol(shipping);
            await new AuthMessageSender().Execute(email, "Truckonnow - BOL", patern, shipping.VehiclwInformations, shipping);
        }

        public string GetPlateNumber(string image, string idDriver, string type)
        {
            string plate = "";
            IDetect detect = null;
            if (type == "truck")
            {
                detect = new DerectTruck();
            }
            else if (type == "trailer")
            {
                detect = new DetectTrailers();
            }
            if (detect != null)
            {
                detect.AuchGoole(sqlCommandApiMobile);
                plate = detect.DetectText(idDriver, Convert.FromBase64String(image));
            }
            return plate;
        }

        internal Shipping GetStatusInspectionPickedUp(string idShipping)
        {
            return sqlCommandApiMobile.GetStatusInspectionPickedUpDb(idShipping);
        }

        internal Shipping GetStatusInspectionDelivery(string idShipping)
        {
            return sqlCommandApiMobile.GetStatusInspectionDeliveryDb(idShipping);
        }

        internal Shipping GetStatusInspectionEnd(string idShipping)
        {
            return sqlCommandApiMobile.GetStatusInspectionEndDb(idShipping);
        }

        internal List<Truck> GetTruck()
        {
            return sqlCommandApiMobile.GetTrucks();
        }

        internal List<Trailer> GetTrailer()
        {
            return sqlCommandApiMobile.GetTrailers();
        }

        public async Task SendCoupon(string email)
        {
            string patern = new PaternSourse().GetPaternCopon();
            await new AuthMessageSender().Execute(email, "Truckonnow - Coupon", patern);
        }

        public async void SetInspectionDriver(string idDriver, string inspectionDriverStr)
        {
            InspectionDriver inspectionDriver = JsonConvert.DeserializeObject<InspectionDriver>(inspectionDriverStr);
            await sqlCommandApiMobile.SetInspectionDriverInDb(idDriver, inspectionDriver);
        }

        public async Task SaveInspactionDriver(string idDriver, string photoJson, int indexPhoto, string typeTransportVehicle)
        {
            photoJson = photoJson.Insert(photoJson.IndexOf(idDriver) + 2, $"{DateTime.Now.ToShortDateString()}/{typeTransportVehicle}/");
            PhotoDriver photo = JsonConvert.DeserializeObject<PhotoDriver>(photoJson);
            await sqlCommandApiMobile.SaveInspectionDriverInDb(idDriver, photo, indexPhoto, typeTransportVehicle);
            //await Task.Run(() =>
            //{
            //    IDetect detect = null;
            //    if(indexPhoto == 1 || indexPhoto == 2 || indexPhoto == 26 || indexPhoto == 13)
            //    {
            //        detect = new DerectTruck();
            //    }
            //    else if(indexPhoto == 34 || indexPhoto == 35 || indexPhoto == 38)
            //    {
            //        detect = new DetectTrailers();
            //    }
            //    if(detect != null)
            //    {
            //        detect.AuchGoole(sqlCommandApiMobile);
            //        detect.DetectText(idDriver, photo.path);
            //    }
            //});
        }

        internal bool CheckFullNameAndPasswrod(string email, string fullName)
        {
            bool isFullNamePassword = sqlCommandApiMobile.CheckFullNameAndPasswrodDB(email, fullName);
            if (isFullNamePassword)
            {
                string token = CreateToken(email, fullName);
                int idDriver = sqlCommandApiMobile.AddRecoveryPassword(email, fullName, token);
                string patern = new PaternSourse().GetPaternRecoveryPassword($"{Config.UrlAdmin}/Recovery/Password?idDriver={idDriver}&token={token}");
                Task.Run(async () => await new AuthMessageSender().Execute(email, "Password recovery", patern));
            }
            return isFullNamePassword;
        }

        public async void UpdateInspectionDriver(string idDriver)
        {
            await sqlCommandApiMobile.UpdateInspectionDriver(idDriver);
        }

        public async Task<bool> ChechToDayInspaction(string token)
        {
            int countImageTruck = 0;
            int countImageTrailer = 0;
            string plateTrailer = sqlCommandApiMobile.GetPlateTrailerByTokenDriver(token);
            if(plateTrailer != null)
            {
                countImageTrailer = GetPaternTrailerInspectionDriver(plateTrailer).CountPhoto;
            }
            string plateTruck = sqlCommandApiMobile.GetPlateTruckByTokenDriver(token);
            if (plateTruck != null)
            {
                countImageTruck = GetPaternTruckInspectionDriver(plateTruck).CountPhoto;
            }
            return await sqlCommandApiMobile.ChechToDayInspactionInDb(token, countImageTruck, countImageTrailer);
        }

        public int GetIndexPhoto(string token)
        {
            return sqlCommandApiMobile.GetIndexDb(token);
        }

        public void SaveGPSLocationData(string token, string longitude, string latitude)
        {
            Geolocations geolocations = new Geolocations();
            geolocations.Latitude = latitude;
            geolocations.Longitude = longitude;
            sqlCommandApiMobile.SaveGPSLocationData(token, geolocations);
        }

        public void SaveTokenStore(string token, string tokenStore)
        {
            sqlCommandApiMobile.SaveTokenStoreinDb(token, tokenStore);
        }   

        public async Task SaveDamageForUser(string idVech, string idShiping, string damageForUserJson)
        {
            List<DamageForUser> damageForUsers = JsonConvert.DeserializeObject<List<DamageForUser>>(damageForUserJson);
            Task.Run(async() =>
            {
                VehiclwInformation vehiclwInformation = await sqlCommandApiMobile.GetVehiclwInformationAndSaveDamageForUser(idVech, idShiping, damageForUsers);
                ITypeScan typeScan = GetTypeScan(vehiclwInformation.Ask.TypeVehicle);
                await typeScan.SetDamage(damageForUsers, vehiclwInformation.Ask.TypeVehicle, vehiclwInformation.Scan.path);
            });
        }

        internal ITransportVehicle GetPaternTrailerInspectionDriverByTokenDriver(string token)
        {
            ITransportVehicle transportVehicle = null;
            string plateTrailer = sqlCommandApiMobile.GetPlateTrailerByTokenDriver(token);
            if(plateTrailer != null)
            {
                transportVehicle = GetPaternTrailerInspectionDriver(plateTrailer);
            }
            else
            {
                string plateTruck = sqlCommandApiMobile.GetPlateTruckByTokenDriver(token);
                if(plateTruck != null)
                {
                    transportVehicle = GetPaternTruckInspectionDriver(plateTruck);
                }
            }
            return transportVehicle;
        }

        internal ITransportVehicle GetPaternTrailerInspectionDriver(string plateTrailer)
        {
            ITransportVehicle transportVehicle = null;
            Trailer trailer = sqlCommandApiMobile.GetTrailers().FirstOrDefault(t => t.Plate == plateTrailer);
            ProfileSetting profileSetting = sqlCommandApiMobile.GetProfileSetingsByIdTr(trailer.Id, TypeTransportVehikle.Trailer);
            if (profileSetting != null)
            {
                transportVehicle = GetTransportVehicle(profileSetting.TransportVehicle);
                transportVehicle.IsNextInspection = false;
            }
            else
            {
                transportVehicle = HelperTransport.GetTransportVehicle(trailer.Type);
            }
            return transportVehicle;
        }

        internal ITransportVehicle GetPaternTruckInspectionDriver(string plateTruck)
        {
            ITransportVehicle transportVehicle = null;
            Truck truck = sqlCommandApiMobile.GetTrucks().FirstOrDefault(t => t.PlateTruk == plateTruck);
            ProfileSetting profileSetting = sqlCommandApiMobile.GetProfileSetingsByIdTr(truck.Id, TypeTransportVehikle.Truck);
            if (profileSetting != null)
            {
                transportVehicle = GetTransportVehicle(profileSetting.TransportVehicle);
                transportVehicle.IsNextInspection = true;
            }
            else
            {
                transportVehicle = HelperTransport.GetTransportVehicle(truck.Type);
            }
            return transportVehicle;
        }

        private ITransportVehicle GetTransportVehicle(TransportVehicle transportVehicle)
        {
            ITransportVehicle transportVehicleRes = null;
            transportVehicle.Layouts = transportVehicle.Layouts.OrderBy(l => l.OrdinalIndex).ToList();
            transportVehicleRes = HelperTransport.GetTransportVehicle("Deffalt");
            transportVehicleRes.CountPhoto = transportVehicle.CountPhoto;
            transportVehicleRes.Type = transportVehicle.Type;
            transportVehicleRes.TypeTransportVehicle = transportVehicle.TypeTransportVehicle;
            transportVehicleRes.Layouts = new List<string>();
            transportVehicleRes.NamePatern = new List<string>();
            for (int i = 0; i < transportVehicle.Layouts.Count; i++)
            {
                if (transportVehicle.Layouts[i].IsUsed)
                {
                    transportVehicleRes.Layouts.Add(transportVehicle.Layouts[i].Index.ToString());
                    transportVehicleRes.NamePatern.Add(transportVehicle.Layouts[i].Name);
                }
            }
            return transportVehicleRes;
        }

        internal VehiclwInformation GetVehiclwPhoto(string idVehiclw)
        {
            return sqlCommandApiMobile.GetVehiclwPhotoDb(idVehiclw);
        }

        public async Task SaveSigPhoto(string idShip, string sig)
        {
            Photo photoSig = JsonConvert.DeserializeObject<Photo>(sig);
            sqlCommandApiMobile.SaveSigPikedUpInDb(idShip, photoSig);
        }

        public VehiclwInformation GetVehiclwInformation(int idVech)
        {
            return sqlCommandApiMobile.GetVehiclwInformationInDb(idVech);
        }

        public Shipping GetShipping(string idShip)
        {
            return sqlCommandApiMobile.GetShippingInDb(idShip);
        }

        public Shipping GetShippingPhoto(string idShip)
        {
            return sqlCommandApiMobile.GetShippingPhotInDb(idShip);
        }

        public async Task SavePhotoInspection(string idVe, string photoInspectionJson)
        {
            PhotoInspection photoInspection = JsonConvert.DeserializeObject<PhotoInspection>(photoInspectionJson);
            VehiclwInformation vehiclwInformation = await sqlCommandApiMobile.SavePhotoInspectionInDb(idVe, photoInspection);
            Task.Run(async () =>
            {
                ITypeScan typeScan = GetTypeScan(vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                await typeScan.SetDamage(photoInspection, vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""), vehiclwInformation.Scan.path);
            });
        }

        private ITypeScan GetTypeScan(string type)
        {
            ITypeScan typeScan = null;
            switch(type)
            {
                case "Coupe":
                    {
                        typeScan = new CoupeCar();
                        break;
                    }
                case "Suv":
                    {
                        typeScan = new SuvCar();
                        break;
                    }
                case "PickUp":
                    {
                        typeScan = new PickedUpCar();
                        break;
                    }
                case "Sedan":
                    {
                        typeScan = new SedanCar();
                        break;
                    }
                case "Sportbike":
                    {
                        typeScan = new SportMotorcycle();
                        break;
                    }
                case "Touringmotorcycle":
                    {
                        typeScan = new TouringMotorcycle();
                        break;
                    }
                case "Cruisemotorcycle":
                    {
                        typeScan = new MotorcycleСruising();
                        break;
                    }
                case "Tricycle":
                    {
                        typeScan = new TricycleMotorcycle();
                        break;
                    }
            }
            return typeScan;
        }

        internal string CheckTralerAndTruck(string token)
        {
            return sqlCommandApiMobile.CheckTralerAndTruckDb(token);
        }

        internal bool SetTralerAndTruck(string token, string plateTrailer, string plateTruck, string nowCheck)
        {
            return sqlCommandApiMobile.SetTralerAndTruck(token, plateTrailer, plateTruck, nowCheck);
        }

        internal string GetDocument(string id)
        {
            return sqlCommandApiMobile.GetDocumentDB(id);
        }

        public async Task SaveAsk(string id, int type, string jsonStrAsk)
        {
            if(type == 1)
            {
                Ask ask = JsonConvert.DeserializeObject<Ask>(jsonStrAsk);
                sqlCommandApiMobile.SaveAskInDb(id, ask);
            }
            else if(type == 2)
            {
                Ask1 ask1 = JsonConvert.DeserializeObject<Ask1>(jsonStrAsk);
                sqlCommandApiMobile.SaveAsk1InDb(id, ask1);
            }
            else if (type == 3)
            {
                AskFromUser askFromUser = JsonConvert.DeserializeObject<AskFromUser>(jsonStrAsk);
                sqlCommandApiMobile.SaveAskFromUserInDb(id, askFromUser);
            }
            else if(type == 4)
            {
                AskDelyvery askDelyvery = JsonConvert.DeserializeObject<AskDelyvery>(jsonStrAsk);
                sqlCommandApiMobile.SaveAskDelyveryInDb(id, askDelyvery);
            }
            else if (type == 5)
            {
                 AskForUserDelyveryM askForUserDelyveryM = JsonConvert.DeserializeObject<AskForUserDelyveryM>(jsonStrAsk);
                 sqlCommandApiMobile.SaveAskForUserDelyveryInDb(id, askForUserDelyveryM);
            }
            else if (type == 6)
            {
                Ask2 ask2 = JsonConvert.DeserializeObject<Ask2>(jsonStrAsk);
                sqlCommandApiMobile.SaveAsk2InDb(id, ask2);
            }
            else if (type == 7)
            {
                List<Photo> photo = JsonConvert.DeserializeObject<List<Photo>>(jsonStrAsk);
                sqlCommandApiMobile.SavePhotoinTruckInDb(id, photo);
            }
            else if (type == 8)
            {
                List<Photo> photo = JsonConvert.DeserializeObject<List<Photo>>(jsonStrAsk);
                sqlCommandApiMobile.SavePhotoStrapInDb(id, photo);
            }
        }

        internal string GetLastInspaction(string idDriver)
        {
            return sqlCommandApiMobile.GetLastInspaction(idDriver);
        }

        public async Task SaveFeedBack(string jsonStrAsk)
        {
            Feedback feedback = JsonConvert.DeserializeObject<Feedback>(jsonStrAsk);
            sqlCommandApiMobile.SaveFeedBackInDb(feedback);
        }

        public async Task ReCurentStatus(string idShip, string status)
        {
            sqlCommandApiMobile.ReCurentStatus(idShip, status);
            await Task.Run(() =>
            {
                try
                {
                    string tokenShope = sqlCommandApiMobile.GerShopTokenForShipping(idShip);
                    ManagerNotifyMobileApi managerNotifyMobileApi = new ManagerNotifyMobileApi();
                    if (status == "Picked up")
                    {
                        managerNotifyMobileApi.SendNotyfyStatusPickup(idShip, tokenShope);
                    }
                    else if (status == "Delivered,Paid")
                    {
                        managerNotifyMobileApi.SendNotyfyStatusDelyvery(idShip, tokenShope, "Cars passed inspection, the order is paid");
                    }
                    else if (status == "Delivered,Billed")
                    {
                        managerNotifyMobileApi.SendNotyfyStatusDelyvery(idShip, tokenShope, "Cars passed inspection, waiting for payment (Billing)");
                    }
                }
                catch(Exception e)
                {
                    File.WriteAllText("ReCurentStatus1.txt", e.Message);
                }
            });
        }

        private void CheckAndCreatedFolder()
        {
            if(!Directory.Exists("PhotoCars"))
            {
                Directory.CreateDirectory("PhotoCars");
            }
        }

        public string Avtorization(string email, string password)
        {
            string token = "";
            if (sqlCommandApiMobile.CheckEmailAndPsw(email, password))
            {
                token = CreateToken(email, password);
                token += ","+ sqlCommandApiMobile.SaveToken(email, password, token);
            }
            return token;
        }
        
        public void SavepOrder(string id, string idOrder, string name, string contactName, string address, 
            string city, string state, string zip, string phone, string email, string typeSave)
        {
            if (typeSave == "PikedUp")
            {
                sqlCommandApiMobile.SavePikedUpInDb(id, idOrder, name, contactName, address, city, state, zip, phone, email);
            }
            else if(typeSave == "Delivery")
            {
                sqlCommandApiMobile.SaveDeliveryInDb(id, idOrder, name, contactName, address, city, state, zip, phone, email);
            }
        }

        public void SavepOrder(string id, string typeSave, string payment, string paymentTeams)
        {
            if (typeSave == "Payment")
            {
                sqlCommandApiMobile.SavePaymentsInDb(id, payment, paymentTeams);
            }
        }

        public bool CheckToken(string token)
        {
            return sqlCommandApiMobile.CheckToken(token);
        }

        public string GetInspectionDriver(string token)
        {
            return sqlCommandApiMobile.GetInspectionDriverIndb(token);
        }

        internal void SetInstraction(string idShiping)
        {
            sqlCommandApiMobile.SetInstractionShipingById(idShiping);
        }

        public bool GetOrdersForToken(string token, ref List<Shipping> shippings)
        {
            bool isToken = sqlCommandApiMobile.CheckToken(token);
            if (isToken)
            {
                shippings = sqlCommandApiMobile.GetOrdersForToken(token);
            }
            return isToken;
        }

        public bool GetOrdersDelyveryForToken(string token, ref List<Shipping> shippings)
        {
            bool isToken = sqlCommandApiMobile.CheckToken(token);
            if (isToken)
            {
                shippings = sqlCommandApiMobile.GetOrdersDelyveryForToken(token, 0);
            }
            return isToken;
        }

        public bool GetOrdersArchiveForToken(string token, ref List<Shipping> shippings)
        {
            bool isToken = sqlCommandApiMobile.CheckToken(token);
            if (isToken)
            {
                shippings = sqlCommandApiMobile.GetOrdersArchiveForToken(token, 0);
            }
            return isToken;
        }

        public async Task SavePay(string idShiping, int type, string photo)
        {
            Photo photo1 = JsonConvert.DeserializeObject<Photo>(photo);
            sqlCommandApiMobile.SavePayInDb(idShiping, type, photo1);
        }

        public async Task SaveRecount(string idShiping, int type, string video)
        {
            Video video1 = JsonConvert.DeserializeObject<Video>(video);
            await sqlCommandApiMobile.SaveRecontInDb(idShiping, type, video1);
        }

        public async Task SavePayMethot(string idShiping, string payMethod, string countPay)
        {
            sqlCommandApiMobile.SavePayMethotInDb(idShiping, payMethod, countPay);
        }

        public void ClearToken(string token)
        {
            sqlCommandApiMobile.ClearTokenDb(token);
        }

        private string CreateToken(string email, string password)
        {
            string token = "";
            for(int i = 0; i < email.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + email[i]; 
            }
            for (int i = 0; i < password.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + password[i];
            }
            return token;
        }

        public void SetProplem(string idShiping, string type)
        {
            sqlCommandApiMobile.SetProblemDb(idShiping);
        }

        public bool CheckProplem(string idShiping, string type)
        {
            return sqlCommandApiMobile.CheckProplemDb(idShiping);
        }

        #region Task
        public List<Tasks> CheckTask(string token)
        {
            return sqlCommandApiMobile.CheckTask(token);
        }

        public async Task<string> StartTask(string nameMethod, string optionalParameter, string token)
        {
            return await sqlCommandApiMobile.StartTaskDb(nameMethod, optionalParameter, token);
        }

        public async Task<string> LoadTask(string idTask, string byteBase64)
        {
            byte[] buffer = Convert.FromBase64String(byteBase64);
            return await sqlCommandApiMobile.LoadTaskDb(idTask, buffer);
        }

        public async Task EndTask(string idTask, string nameMethod)
        {
            string[] objSave = await sqlCommandApiMobile.EndTaskDb(idTask);
            if (objSave != null)
            {
                await GoToEndTask(objSave[0], nameMethod, objSave[1]);
            }
        }

        private async Task GoToEndTask(string objSave, string nameMethod, string optionalParameter)
        {
            string[] parameter = null;
            try
            {
                if (nameMethod == "SavePhoto")
                {
                    parameter = optionalParameter.Split(',');
                    await SavePhotoInspection(parameter[0], objSave);
                }
                else if (nameMethod == "SaveInspactionDriver")
                {
                    parameter = optionalParameter.Split(',');
                    //await SaveInspactionDriver(parameter[0], objSave, Convert.ToInt32(parameter[1]));
                }
                else if (nameMethod == "SaveRecount")
                {
                    parameter = optionalParameter.Split(',');
                    await SaveRecount(parameter[0], Convert.ToInt32(parameter[1]), objSave);
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}