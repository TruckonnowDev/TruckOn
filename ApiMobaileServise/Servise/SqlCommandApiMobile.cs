using ApiMobaileServise.Models;
using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionDriver = DaoModels.DAO.Models.InspectionDriver;

namespace ApiMobaileServise.Servise
{
    public class SqlCommandApiMobile
    {
        private Context context = null;

        public SqlCommandApiMobile()
        {
            context = new Context();
            if (Context._cache == null) { Context._cache = new MemoryCache(new MemoryCacheOptions()); }
        }

        internal List<PasswordRecovery> GetPasswordRecovery()
        {
            return context.PasswordRecoveries.ToList();
        }

        public List<Shipping> GetShipingPayd()
        {
            return context.Shipping.Where(s => s.CurrentStatus == "Delivered,Paid").ToList();
        }

        internal void RemovePasswordRecoveriesRange(List<PasswordRecovery> passwordRecoveries)
        {
            context.PasswordRecoveries.RemoveRange(passwordRecoveries);
            context.SaveChanges();
        }

        internal List<Driver> GetDriverInDb()
        {
            return context.Drivers.Where(d => !d.IsFired).ToList();
        }

        internal List<Trailer> GetTrailers()
        {
            return context.Trailers.ToList();
        }

        internal List<Truck> GetTrucks()
        {
            return context.Trucks.ToList();
        }

        public Shipping SendBolInDb(string idShip)
        {
            Shipping shipping = context.Shipping.Where(s => s.Id == idShip)
                .Include("VehiclwInformations.Scan")
                .Include(s => s.askForUserDelyveryM)
                .Include(s => s.AskFromUser)
                .FirstOrDefault();
            return shipping;
        }

        internal Shipping GetStatusInspectionPickedUpDb(string idShipping)
        {
            Shipping shipping = context.Shipping
                .Where(s => s.Id == idShipping)
                .Include(s => s.VehiclwInformations)
                .Include("VehiclwInformations.Ask")
                .Include("VehiclwInformations.Ask1")
                .Include("VehiclwInformations.Ask1.App_will_force_driver_to_take_pictures_of_each_strap")
                .Include("VehiclwInformations.PhotoInspections.Damages")
                .Include(s => s.AskFromUser.App_will_ask_for_signature_of_the_client_signature)
                .Include(s => s.AskFromUser.PhotoPay)
                .Include(s => s.AskFromUser.VideoRecord)
                .FirstOrDefault();
            return shipping;
        }

        internal Shipping GetStatusInspectionDeliveryDb(string idShipping)
        {
            Shipping shipping = context.Shipping
                .Where(s => s.Id == idShipping)
                .Include(s => s.VehiclwInformations)
                .Include("VehiclwInformations.Ask")
                .Include("VehiclwInformations.AskDelyvery")
                .Include("VehiclwInformations.PhotoInspections.Damages")
                .Include(s => s.askForUserDelyveryM.App_will_ask_for_signature_of_the_client_signature)
                .Include(s => s.askForUserDelyveryM.PhotoPay)
                .Include(s => s.askForUserDelyveryM.VideoRecord)
                .Include(s => s.Ask2)
                .FirstOrDefault();
            return shipping;
        }

        internal Shipping GetStatusInspectionEndDb(string idShipping)
        {
            Shipping shipping = context.Shipping
                  .Where(s => s.Id == idShipping)
                  .Include(s => s.VehiclwInformations)
                  .FirstOrDefault();
            return shipping;
        }

        public async Task RefreshInspectionDriverInDb(int idDriver)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Id == idDriver);
            if (driver != null)
            {
                driver.IsInspectionDriver = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task RefreshInspectionToDayDriverInDb(int idDriver)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Id == idDriver);
            if (driver != null)
            {
                driver.IsInspectionToDayDriver = false;
                await context.SaveChangesAsync();
            }
        }

        internal void SetPlateTruck(int id, string idDriver)
        {
            Driver driver = context.Drivers.Include(d => d.InspectionDrivers)
                       .FirstOrDefault(d => d.Id == Convert.ToInt32(idDriver));
            InspectionDriver inspectionDrivers = driver.InspectionDrivers.FirstOrDefault(i => DateTime.Parse(i.Date).Date == DateTime.Now.Date);
            if (inspectionDrivers != null)
            {
                Truck truck = context.Trucks.First(t => t.Id == id);
                inspectionDrivers.IdITruck = truck.Id;
                context.SaveChanges();
            }
        }

        internal void SetPlateTrailer(int id, string idDriver)
        {
            Driver driver = context.Drivers.Include(d => d.InspectionDrivers)
                       .FirstOrDefault(d => d.Id == Convert.ToInt32(idDriver));
            InspectionDriver inspectionDrivers = driver.InspectionDrivers.FirstOrDefault(i => DateTime.Parse(i.Date).Date == DateTime.Now.Date);
            if (inspectionDrivers != null)
            {
                Trailer trailer = context.Trailers.First(t => t.Id == id);
                inspectionDrivers.IdITruck = trailer.Id;
                context.SaveChanges();
            }
        }

        public async Task SetInspectionDriverInDb(string idDriver, InspectionDriver inspectionDriver)
        {
            Driver driver = await context.Drivers.Where(d => d.Id == Convert.ToUInt32(idDriver))
                .Include(d => d.InspectionDrivers)
                .FirstOrDefaultAsync();
            if(driver.InspectionDrivers == null)
            {
                driver.InspectionDrivers = new List<InspectionDriver>();
            }
            driver.InspectionDrivers.Add(inspectionDriver);
            await context.SaveChangesAsync();
        }

        internal int AddRecoveryPassword(string email, string fullName, string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.EmailAddress == email && d.FullName == fullName);
            PasswordRecovery passwordRecovery1 = context.PasswordRecoveries.FirstOrDefault(p => p.IdDriver == driver.Id);
            if (passwordRecovery1 == null)
            {
                PasswordRecovery passwordRecovery = new PasswordRecovery()
                {
                    Date = DateTime.Now.ToString(),
                    IdDriver = driver.Id,
                    Token = token
                };
                context.PasswordRecoveries.Add(passwordRecovery);
            }
            else
            {
                passwordRecovery1.Token = token;
                passwordRecovery1.Date = DateTime.Now.ToString();
            }
            context.SaveChanges();
            return driver.Id;
        }

        internal string GetPlateTrailerByTokenDriver(string token)
        {
            string plateTrailer = null;
            Driver driver = context.Drivers
                .Include(d => d.InspectionDrivers)
                .First(d => d.Token == token);
            InspectionDriver inspectionDriver = driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            if(inspectionDriver != null && inspectionDriver.IdITrailer != 0 && Convert.ToDateTime(inspectionDriver.Date).Date == DateTime.Now.Date)
            {
                plateTrailer = context.Trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer) != null ? context.Trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer).Plate : null;
            }
            return plateTrailer;
        }

        internal ProfileSetting GetProfileSetingsByIdTr(int idTr, TypeTransportVehikle typeTransportVehikle)
        {
            return context.ProfileSettings
                .Where(p => p.IsUsed && p.TypeTransportVehikle == typeTransportVehikle.ToString() && p.IdTr == idTr)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();
        }

        internal string GetPlateTruckByTokenDriver(string token)
        {
            string plateTrailer = null;
            Driver driver = context.Drivers
                .Include(d => d.InspectionDrivers)
                .First(d => d.Token == token);
            InspectionDriver inspectionDriver = driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            if (inspectionDriver != null && inspectionDriver.IdITruck != 0 && Convert.ToDateTime(inspectionDriver.Date).Date == DateTime.Now.Date)
            {
                plateTrailer = context.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck) != null ? context.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck).PlateTruk : null;
            }
            return plateTrailer;
        }

        internal bool CheckFullNameAndPasswrodDB(string email, string fullName)
        {
            bool isCheckFullNameAdnPassword = false;
            Driver driver = context.Drivers.FirstOrDefault(d => d.EmailAddress == email && d.FullName == fullName);
            if (driver != null)
            {
                isCheckFullNameAdnPassword = true;
            }
            return isCheckFullNameAdnPassword;
        }

        public async Task SaveInspectionDriverInDb(string idDriver, PhotoDriver photo, int IndexPhoto, string typeTransportVehicle)
        {
            try
            {
                Driver driver = context.Drivers.Include(d => d.InspectionDrivers)
                    .FirstOrDefault(d => d.Id == Convert.ToInt32(idDriver));
                if (driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
                {
                    InspectionDriver inspectionDrivers = driver.InspectionDrivers.FirstOrDefault(i => DateTime.Parse(i.Date).Date == DateTime.Now.Date);
                    if (inspectionDrivers == null)
                    {
                        inspectionDrivers = new InspectionDriver();
                        inspectionDrivers.Date = DateTime.Now.ToString();
                        inspectionDrivers.PhotosTruck = new List<PhotoDriver>();
                        driver.InspectionDrivers.Add(inspectionDrivers);
                    }
                    await context.SaveChangesAsync();
                    photo.IdInspaction = inspectionDrivers.Id; 
                    photo.IndexPhoto = IndexPhoto;
                    inspectionDrivers.PhotosTruck = context.PhotoDrivers.Where(p => p.IdInspaction == inspectionDrivers.Id).ToList();
                    if ((typeTransportVehicle == "Truck" && inspectionDrivers.CountPhotoTruck == IndexPhoto - 1)
                        || (typeTransportVehicle == "Trailer" && inspectionDrivers.CountPhotoTrailer == IndexPhoto - 1))
                    {
                        if (typeTransportVehicle == "Truck")
                        {
                            inspectionDrivers.CountPhotoTruck++;
                        }
                        else if (typeTransportVehicle == "Trailer")
                        {
                            inspectionDrivers.CountPhotoTrailer++;
                        }
                        inspectionDrivers.PhotosTruck.Add(photo);
                    }
                    await context.SaveChangesAsync();
                }
                else
                {
                    InspectionDriver inspectionDrivers = new InspectionDriver();
                    driver.InspectionDrivers = new List<InspectionDriver>();
                    inspectionDrivers.Date = DateTime.Now.ToString();
                    inspectionDrivers.PhotosTruck = new List<PhotoDriver>();

                    if (typeTransportVehicle == "Truck")
                    {
                        inspectionDrivers.CountPhotoTruck++;
                    }
                    else if (typeTransportVehicle == "Trailer")
                    {
                        inspectionDrivers.CountPhotoTrailer++;
                    }
                    inspectionDrivers.PhotosTruck.Add(photo);
                    driver.InspectionDrivers.Add(inspectionDrivers);
                    await context.SaveChangesAsync();
                    photo.IdInspaction = inspectionDrivers.Id;
                    await context.SaveChangesAsync();
                }
            }
            catch(Exception e)
            {

            }
        }

        internal VehiclwInformation GetVehiclwPhotoDb(string idVehiclw)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation
                .Where(v => v.Id.ToString() == idVehiclw)
                .Include(v => v.Scan)
                .Include(v => v.PhotoInspections)
                .FirstOrDefault();
            return vehiclwInformation;
        }

        internal string CheckTralerAndTruckDb(string token)
        {
            string plates = null;
            Driver driver =  context.Drivers.Where(d => d.Token == token)
                   .Include(d => d.InspectionDrivers)
                   .FirstOrDefault(); 
            InspectionDriver inspectionDriver = driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            if (inspectionDriver != null)
            {
                List<Truck> trucks = context.Trucks.ToList();
                List<Trailer> trailers = context.Trailers.ToList();
                DateTime dateTime = Convert.ToDateTime(inspectionDriver.Date);
                if (dateTime.Date == DateTime.Now.Date)
                {
                    Truck truck = trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck);
                    Trailer trailer = trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer);
                    string plateTruck = truck != null ? truck.PlateTruk : "";
                    string plateTrailer = trailer != null ? trailer.Plate : "";
                    plates = $"{plateTruck},{plateTrailer}";
                }
                else
                {
                    plates = ",";
                }
            }
            else
            {
                plates = ",";
            }
            return plates;
        }

        internal string GetDocumentDB(string id)
        {
            //string pathDoc = "";
            //Driver driver = context.Drivers
            //    .Include(d => d.InspectionDrivers)
            //    .FirstOrDefault(d => d.Id.ToString() == id);
            //if(driver.InspectionDrivers != null)
            //{
            //    InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
            //    Truck truck = context.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck);
            //    if(truck != null)
            //    {
            //        pathDoc = truck.PathDoc;
            //    }
            //}
            return "";
        }

        public async Task UpdateInspectionDriver(string idDriver)
        {
            Driver driver = await context.Drivers.Where(d => d.Id == Convert.ToUInt32(idDriver))
                   .Include(d => d.InspectionDrivers)
                   .FirstOrDefaultAsync();
            driver.LastDateInspaction = DateTime.Now.ToString();
            InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
            inspectionDriver.Date = DateTime.Now.ToString();
            driver.IsInspectionDriver = true;
            driver.IsInspectionToDayDriver = true;
            context.SaveChanges();
        }

        public async Task<bool> ChechToDayInspactionInDb(string token, int countImageTruck, int countImageTrailer)
        {
            bool isInspaction = false;
            Driver driver = await context.Drivers.Where(d => d.Token == token)
                 .Include(d => d.InspectionDrivers)
                 .FirstOrDefaultAsync();
            if (driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
            {
                InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
                DateTime dateTime = Convert.ToDateTime(inspectionDriver.Date);
                Truck truck = context.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck);
                Trailer trailer = context.Trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck);
                if (dateTime.Date != DateTime.Now.Date || (inspectionDriver.CountPhotoTruck + inspectionDriver.CountPhotoTrailer <= countImageTruck + countImageTrailer))
                {
                    isInspaction = false;
                }
                else
                {
                    driver.IsInspectionToDayDriver = true;
                    driver.IsInspectionDriver = true;
                    isInspaction = true;
                }
            }
            else
            {
                isInspaction = false;
            }
            context.SaveChanges();
            return isInspaction;
        }

        internal void SavePhotoStrapInDb(string id, List<Photo> photos)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation
                .Where(v => v.Id.ToString() == id)
                .Include(v => v.Ask1)
                .FirstOrDefault();
            if (vehiclwInformation != null)
            {
                vehiclwInformation.Ask1.App_will_force_driver_to_take_pictures_of_each_strap = photos;
                context.SaveChanges();
            }
        }

        internal void SavePhotoinTruckInDb(string id, List<Photo> photos)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation
                .Where(v => v.Id.ToString() == id)
                .Include(v => v.Ask1)
                .FirstOrDefault();
            if(vehiclwInformation != null)
            {
                vehiclwInformation.Ask1.Photo_after_loading_in_the_truck = photos;
                context.SaveChanges();
            }
        }

        internal string GetLastInspaction(string idDriver)
        {
            string lastInspectionDriver = null;
            Driver driver = context.Drivers.Where(d => d.Id.ToString() == idDriver)
                 .Include(d => d.InspectionDrivers)
                 .FirstOrDefault();
            if(driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
            {
                InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
                Truck truck = context.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck);
                Trailer trailer = context.Trailers.FirstOrDefault(t => t.Id == inspectionDriver.IdITrailer);
                lastInspectionDriver = inspectionDriver.Date.Remove(inspectionDriver.Date.IndexOf(" ") + 1);
                if(truck != null)
                {
                    lastInspectionDriver += $",{truck.PlateTruk}";
                }
                if (trailer != null)
                {
                    lastInspectionDriver += $",{trailer.Plate}";
                }

            }
            return lastInspectionDriver;
        }

        internal void SetInstractionShipingById(string idShiping)
        {
            Shipping shipping = context.Shipping.FirstOrDefault(s => s.Id.ToString() == idShiping);
            if(shipping != null)
            {
                shipping.IsInstructinRead = true;
                context.SaveChanges();
            }
        }

        internal bool SetTralerAndTruck(string token, string plateTrailer, string plateTruck, string nowCheck)
        {
            Truck truck = context.Trucks.FirstOrDefault(t => t.PlateTruk == plateTruck);
            Trailer trailer = context.Trailers.FirstOrDefault(t => t.Plate == plateTrailer);
            if (truck == null && trailer == null)
                return false;
            if(nowCheck == "Truck")
            {
                if (truck == null)
                    return false;
            }
            else if(nowCheck == "Trailer")
            {
                if (trailer == null)
                    return false;
            }
            Driver driver = context.Drivers
                .Include(d => d.InspectionDrivers)
                .First(d => d.Token == token);
            InspectionDriver inspectionDriver = driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            if (inspectionDriver != null && Convert.ToDateTime(inspectionDriver.Date).Date == DateTime.Now.Date)
            {
                if (truck != null)
                {
                    inspectionDriver.IdITruck = truck.Id;
                }
                if (trailer != null)
                {
                    inspectionDriver.IdITrailer = trailer.Id;
                }
            }
            else
            {
                if (truck != null)
                {
                    driver.InspectionDrivers.Add(new InspectionDriver()
                    {
                        IdITruck = truck.Id,
                        Date = DateTime.Now.ToString(),
                        CountPhotoTruck = 0,
                        CountPhotoTrailer = 0
                    });
                }
                if (trailer != null)
                {
                    driver.InspectionDrivers.Add(new InspectionDriver()
                    {
                        IdITrailer = trailer.Id,
                        Date = DateTime.Now.ToString(),
                        CountPhotoTruck = 0,
                        CountPhotoTrailer = 0
                    });
                }
            }
            context.SaveChanges();
            return true;
        }

        public int GetIndexDb(string token)
        {
            int indexPhoto = 0;
            Driver driver = context.Drivers.Where(d => d.Token == token)
                 .Include(d => d.InspectionDrivers)
                 .FirstOrDefault();
            if (driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
            {
                InspectionDriver inspectionDrivers = driver.InspectionDrivers.FirstOrDefault(i => DateTime.Parse(i.Date).Date == DateTime.Now.Date);
                if (inspectionDrivers == null)
                {
                    indexPhoto = 1;
                }
                else
                {
                    if(inspectionDrivers.IdITrailer != 0)
                    {
                        indexPhoto = inspectionDrivers.CountPhotoTrailer + 1;
                    }
                    else
                    {
                        indexPhoto = inspectionDrivers.CountPhotoTruck + 1;
                    }
                }
            }
            else
            {
                indexPhoto = 1;
            }
            return indexPhoto;
        }

        public async void SaveGPSLocationData(string token, Geolocations geolocations)
        {
            Driver driver = context.Drivers.Where(d => d.Token == token)
                .Include(d => d.geolocations)
                .FirstOrDefault();
            driver.geolocations = geolocations;
            await context.SaveChangesAsync();
        }

        internal void SaveAsk2InDb(string idShiping, Ask2 ask2)
        {
            Shipping shipping = context.Shipping.FirstOrDefault(s => s.Id == idShiping);
            shipping.Ask2 = ask2;
            context.SaveChanges();
        }

        public async void SaveTokenStoreinDb(string token, string tokenStore)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            driver.TokenShope = tokenStore;
            await context.SaveChangesAsync();
        }

        public async Task<VehiclwInformation> GetVehiclwInformationAndSaveDamageForUser(string idVech, string idShiping, List<DamageForUser> damageForUsers)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation.Where(v => v.Id.ToString() == idVech)
                 .Include(v => v.Ask)
                 .Include("PhotoInspections.Photos")
                 .Include("DamageForUsers")
                 .FirstOrDefault();
            Shipping shipping = context.Shipping
                 .Include("DamageForUsers")
                 .FirstOrDefault(v => v.Id == idShiping);
            if (shipping.DamageForUsers == null)
            {
                shipping.DamageForUsers = new List<DamageForUser>();
            }
            shipping.DamageForUsers.AddRange(damageForUsers);
            await context.SaveChangesAsync();
            return vehiclwInformation;
        }

        public async Task<VehiclwInformation> SavePhotoInspectionInDb(string idVe, PhotoInspection photoInspection)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation.Where(v => v.Id.ToString() == idVe)
                .Include(v => v.Ask)
                .Include(v => v.Scan)
                .Include("PhotoInspections.Photos")
                .Include("PhotoInspections.Damages")
                .FirstOrDefault();
            if (vehiclwInformation.PhotoInspections == null)
            {
                vehiclwInformation.PhotoInspections = new List<PhotoInspection>();
            }
            if (photoInspection.IndexPhoto == 1 && photoInspection.CurrentStatusPhoto == "PikedUp")
            {
                Photo photo = new Photo();
                photo.path = $"../Photo/{vehiclwInformation.Id}/scan.jpg";
                photo.Base64 = Convert.ToBase64String(File.ReadAllBytes($"../Scans/scan{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}.jpg"));
                vehiclwInformation.Scan = photo;
            }
            vehiclwInformation.PhotoInspections.Add(photoInspection);
            await context.SaveChangesAsync();
            return vehiclwInformation;
        }

        public Shipping GetShippingInDb(string idShip)
        {
            Shipping shipping = context.Shipping.Where(s => s.Id.ToString() == idShip)
                .Include("VehiclwInformations.PhotoInspections.Damages")
                .Include("VehiclwInformations.Scan")
                .FirstOrDefault();

            shipping.UrlReqvest = "";
            return shipping;
        }

        public Shipping GetShippingPhotInDb(string idShip)
        {
            Shipping shipping = context.Shipping.Where(s => s.Id.ToString() == idShip)
                .Include("VehiclwInformations.Scan")
                .Include("VehiclwInformations.PhotoInspections.Photos")
                .FirstOrDefault();

            shipping.UrlReqvest = "";
            return shipping;
        }

        public  void SaveAskInDb(string idve, Ask ask)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation.FirstOrDefault(v => v.Id == Convert.ToInt32(idve));
            vehiclwInformation.Ask = ask;
            context.SaveChanges();
        }

        public async void SavePayMethotInDb(string idShiping, string payMethod, string countPay)
        {
            Shipping shipping = context.Shipping.Where(v => v.Id == idShiping)
                .Include(v => v.AskFromUser)
                .FirstOrDefault();
            if (shipping.AskFromUser == null)
            {
                shipping.AskFromUser = new AskFromUser();
            }
            shipping.AskFromUser.What_form_of_payment_are_you_using_to_pay_for_transportation = payMethod;
            shipping.AskFromUser.CountPay = countPay;
            await context.SaveChangesAsync();
        }

        public bool CheckProplemDb(string idShiping)
        {
            Shipping shipping = context.Shipping.First(s => s.Id == idShiping);
            return shipping.IsProblem;
        }

        public void SetProblemDb(string idShiping)
        {
            Shipping shipping = context.Shipping.First(s => s.Id == idShiping);
            shipping.IsProblem = true;
            context.SaveChanges();
        }

        public async void SavePayInDb(string idShiping, int type, Photo photo)
        {
            Shipping shipping = context.Shipping.Where(v => v.Id == idShiping)
                .Include(v => v.AskFromUser)
                .Include(v => v.askForUserDelyveryM)
                .FirstOrDefault();
            if (type == 1 && shipping.AskFromUser != null)
            {
                shipping.AskFromUser.PhotoPay = photo;
            }
            else if (type == 2 && shipping != null && shipping.askForUserDelyveryM != null)
            {
                shipping.askForUserDelyveryM.PhotoPay = photo;
            }
            await context.SaveChangesAsync();
        }

        public async Task SaveRecontInDb(string idShiping, int type, Video video)
        {
            Shipping shipping = context.Shipping.Where(v => v.Id == idShiping)
                .Include(v => v.AskFromUser)
                .Include(v => v.askForUserDelyveryM)
                .Include("AskFromUser.PhotoPay")
                .Include("AskFromUser.VideoRecord")
                .FirstOrDefault();
            if (type == 1 && shipping.AskFromUser != null)
            {
                shipping.AskFromUser.VideoRecord = video;
            }
            else if (type == 2 && shipping != null && shipping.askForUserDelyveryM != null)
            {
                shipping.askForUserDelyveryM.VideoRecord = video;
            }
            await context.SaveChangesAsync();
        }

        public List<Tasks> CheckTask(string token)
        {
            Driver driver = context.Drivers
                .First(d => d.Token == token);
            return context.TaskLoads
                .Where(t => t.IdDriver == driver.Id.ToString())
                .Select(t => new Tasks()
                {
                    IdTask = t.Id.ToString(),
                    Method =t.NameMethod
                })
                .ToList();
        }

        public async Task<string> LoadTaskDb(string idTask, byte[] buffer)
        {
            TaskLoad taskLoad = await context.TaskLoads
                .FirstOrDefaultAsync(l => l.Id.ToString() == idTask);
            if(taskLoad == null)
            {
                return "No";
            }
            string str = Encoding.Default.GetString(taskLoad.Array);
            string str1 = Encoding.Default.GetString(buffer);
            taskLoad.Array = Encoding.Default.GetBytes(str + str1);
            await context.SaveChangesAsync();
            return taskLoad.Id.ToString();
        }

        public async Task<string[]> EndTaskDb(string idTask)
        {
            LogTask logTask = await context.LogTasks
                .Include(l => l.TaskLoads)
                .FirstOrDefaultAsync();
            if(logTask == null)
            {
                return null;
            }
            TaskLoad taskLoad = logTask.TaskLoads.FirstOrDefault(l => l.Id.ToString() == idTask);
            if (taskLoad == null)
            {
                return null;
            }
            string str = Encoding.Default.GetString(taskLoad.Array);
            context.TaskLoads.Remove(taskLoad);
            await context.SaveChangesAsync();
            return new string[] { str, taskLoad.OptionalParameter};
        }

        public async Task<string> StartTaskDb(string nameMethod, string optionalParameter, string token)
        {
            Driver driver = context.Drivers.First(d => d.Token == token);
            LogTask logTask = context.LogTasks
                .Include(l => l.TaskLoads)
                .FirstOrDefault();
            if(logTask == null)
            {
                context.LogTasks.Add(new LogTask()
                {
                    TaskLoads = new List<TaskLoad>()
                });
                context.SaveChanges();
                logTask = context.LogTasks
                 .Include(l => l.TaskLoads)
                 .FirstOrDefault();
            }
            TaskLoad taskLoad = new TaskLoad()
            {
                Array = new byte[0],
                NameMethod = nameMethod,
                OptionalParameter = optionalParameter,
                IdDriver = driver.Id.ToString()
            };
            logTask.TaskLoads.Add(taskLoad);
            context.SaveChanges();
            return taskLoad.Id.ToString();
        }

        public async Task<Feedback> GetFeedbackFromShippingId(string shippingId)
        {
            return await context.Feedbacks.FirstOrDefaultAsync(x => x.ShippingId == shippingId);
        }

        public async Task SaveFeedBackInDb(Feedback feedback, string shippingId)
        {
            var shipping = await context.Shipping.FirstOrDefaultAsync(x => x.Id == shippingId);
            if (shipping != null)
            {
                feedback.ShippingId = shipping.Id;
                feedback.DriverId = shipping.IdDriver;
                context.Feedbacks.Add(feedback);
                await context.SaveChangesAsync();
            }
        }

        public void ReCurentStatus(string idShip, string status)
        {
            Shipping shipping = context.Shipping.FirstOrDefault(s => s.Id == idShip);
            shipping.CurrentStatus = status;
            try
            {
                if (status == "Delivered,Billed" && shipping.TotalPaymentToCarrier.Contains(" days"))
                {
                    shipping.DataPaid = DateTime.Now.AddDays(Convert.ToInt32(shipping.TotalPaymentToCarrier.Replace(" days", ""))).ToString();
                    shipping.DataCancelOrder = DateTime.Now.ToString();
                }
                else if (status == "Delivered,Paid")
                {
                    shipping.DataFullArcive = DateTime.Now.AddDays(21).ToString();
                    shipping.DataCancelOrder = DateTime.Now.ToString();
                }
                else if (shipping.CurrentStatus == "Archived")
                {
                    shipping.CurrentStatus = shipping.CurrentStatus.Replace("Delivered", "Archived");
                }
                context.SaveChanges();
            }
            catch(Exception e)
            {
                File.WriteAllText("ReCurentStatus.txt", shipping.TotalPaymentToCarrier.ToString());
            }
        }

        public string GerShopTokenForShipping(string idOrder)
        {
            Shipping shipping = context.Shipping.Where(d => d.Id == idOrder)
                .FirstOrDefault();
            Driver driver = context.Drivers.First(d => d.Id == shipping.IdDriver);
            return driver.TokenShope;
        }

        public async void SaveSigPikedUpInDb(string idShip, Photo sig)
        {
            Shipping shipping = context.Shipping.Where(v => v.Id == idShip)
                .Include(v => v.AskFromUser)
                .Include(v => v.AskFromUser.App_will_ask_for_signature_of_the_client_signature)
                .FirstOrDefault();
            if (shipping.AskFromUser == null)
            {
                shipping.AskFromUser = new AskFromUser();
            }
            shipping.AskFromUser.App_will_ask_for_signature_of_the_client_signature = sig;
            await context.SaveChangesAsync();
        }

        public void SaveAsk1InDb(string idve, Ask1 ask1)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation.FirstOrDefault(v => v.Id == Convert.ToInt32(idve));
            vehiclwInformation.Ask1 = ask1;
            context.SaveChangesAsync();
        }

        public void SaveAskFromUserInDb(string idShip, AskFromUser askFromUser)
        {
            Shipping shipping = context.Shipping.Where(s => s.Id == idShip).FirstOrDefault();
            shipping.AskFromUser = askFromUser;
            context.SaveChanges();
        }

        public void SaveAskDelyveryInDb(string idve, AskDelyvery askDelyvery)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation.FirstOrDefault(v => v.Id == Convert.ToInt32(idve));
            vehiclwInformation.AskDelyvery = askDelyvery;
            context.SaveChanges();
        }

        public void SaveAskForUserDelyveryInDb(string idShiping, AskForUserDelyveryM askForUserDelyveryM)
        {

            Shipping shipping = context.Shipping.Where(v => v.Id == idShiping).FirstOrDefault();
            shipping.askForUserDelyveryM = askForUserDelyveryM;
            context.SaveChanges();
        }

        public bool CheckEmailAndPsw(string email, string password)
        {
            return context.Drivers.FirstOrDefault(d => d.EmailAddress == email && d.Password == password && !d.IsFired) != null ? true : false;
        }

        public void SavePikedUpInDb(string id, string idOrder, string name, string contactName, string address, string city, string state, string zip, string phone, string email)
        {
            Shipping shipping = context.Shipping.FirstOrDefault(s => s.Id == id);
            if(shipping != null)
            {
                shipping.idOrder = idOrder;
                shipping.NameP = name;
                shipping.ContactNameP = contactName;
                shipping.AddresP = address;
                shipping.CityP = city;
                shipping.StateP = state;
                shipping.ZipP = zip;
                shipping.PhoneP = phone;
                shipping.EmailP = email;
                context.SaveChanges();
            }
        }

        public async void SaveDeliveryInDb(string id, string idOrder, string name, string contactName, string address, string city, string state, string zip, string phone, string email)
        {
            Shipping shipping = context.Shipping.FirstOrDefault(s => s.Id == id);
            if (shipping != null)
            {
                shipping.idOrder = idOrder;
                shipping.NameD = name;
                shipping.ContactNameD = contactName;
                shipping.AddresD = address;
                shipping.CityD = city;
                shipping.StateD = state;
                shipping.ZipD = zip;
                shipping.PhoneD = phone;
                shipping.EmailD = email;
                await context.SaveChangesAsync();
            }
        }

        public async void SavePaymentsInDb(string id, string payment, string paymentTeams)
        {
            Shipping shipping = context.Shipping.FirstOrDefault(s => s.Id == id);
            if (shipping != null)
            {
                shipping.PriceListed = payment;
                shipping.TotalPaymentToCarrier = paymentTeams;
                await context.SaveChangesAsync();
            }
        }

        public string SaveToken(string email, string password, string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => !d.IsFired && d.EmailAddress == email && d.Password == password);
            driver.Token = token;
            context.SaveChanges();
            return driver.Id.ToString();
        }

        public bool CheckToken(string token)
        {
            return context.Drivers.FirstOrDefault(d => d.Token == token) != null ? true : false;
        }

        public async void ClearTokenDb(string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            driver.Token = null;
            driver.TokenShope = null;
            await context.SaveChangesAsync();
        }

        public string GetInspectionDriverIndb(string token)
        {
            string statusAndTimeInInspection = null;
            Driver driver = context.Drivers.Include(d => d.InspectionDrivers).FirstOrDefault(d => d.Token == token);
            InspectionDriver inspectionDriver = driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            if(inspectionDriver == null)
            {
                driver.IsInspectionDriver = false;
                driver.IsInspectionToDayDriver = false;
            }
            else if(Convert.ToDateTime(inspectionDriver.Date).Date != DateTime.Now.Date)
            {
                if (DateTime.Now.Hour >= 12)
                {
                    driver.IsInspectionDriver = false;
                    driver.IsInspectionToDayDriver = false;
                }
                else if (DateTime.Now.Hour <= 12 && 6 <= DateTime.Now.Hour)
                {
                    driver.IsInspectionDriver = true;
                    driver.IsInspectionToDayDriver = false;
                }
            }
            context.SaveChanges();
            if(driver.IsInspectionDriver)
            {
                if(driver.IsInspectionToDayDriver)
                {
                    statusAndTimeInInspection = "true,true,0";
                }
                else
                {
                    string TimOfInsection = "";
                    if ((12 - DateTime.Now.Hour) < 0 || DateTime.Now.Hour < 5)
                    {
                        TimOfInsection = "0";
                    }
                    else
                    {
                        TimOfInsection = (12 - DateTime.Now.Hour).ToString();
                    }
                    statusAndTimeInInspection = "true,false,"+ TimOfInsection;
                }
            }
            else
            {
                if (driver.IsInspectionToDayDriver)
                {
                    statusAndTimeInInspection = "false,true,0";
                }
                else
                {
                    statusAndTimeInInspection = "false,false,0";
                }
            }
            statusAndTimeInInspection += $",{driver.Id}";
            return statusAndTimeInInspection;
        }

        public VehiclwInformation GetVehiclwInformationInDb(int idVech)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation.Where(v => v.Id == idVech)
                .Include("Ask.Any_personal_or_additional_items_with_or_in_vehicle")
                .Include("Ask.Any_paperwork_or_documentation")
                .Include("Ask1.Any_additional_parts_been_given_to_you")
                .Include("Ask1.Any_additional_documentation_been_given_after_loading")
                .Include("Ask1.App_will_force_driver_to_take_pictures_of_each_strap")
                .Include("Ask1.Photo_after_loading_in_the_truck")
                .Include("PhotoInspections.Photos")
                .Include("AskFromUser.PhotoPay")
                .Include(v => v.AskDelyvery)
                .Include("askForUserDelyveryM.PhotoPay")
                .Include("askForUserDelyveryM.App_will_ask_for_signature_of_the_client_signature")
                .Include(v => v.Scan)
                .FirstOrDefault();
            return vehiclwInformation;
        }

        public List<Shipping> GetOrdersForToken(string token)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);

            List<Shipping> shippings = context.Shipping.Where(s => driver != null && s.IdDriver == driver.Id)
                .Include("VehiclwInformations.Ask")
                //.Include("VehiclwInformations.Ask1")
                //.Include("VehiclwInformations.Ask1.App_will_force_driver_to_take_pictures_of_each_strap")
                //.Include("VehiclwInformations.PhotoInspections.Damages")
                //.Include(s => s.AskFromUser.App_will_ask_for_signature_of_the_client_signature)
                //.Include(s => s.AskFromUser.PhotoPay)
                //.Include(s => s.AskFromUser.VideoRecord)
                .Include("VehiclwInformations.AskDelyvery")
                //.Include(s => s.askForUserDelyveryM.App_will_ask_for_signature_of_the_client_signature)
                //.Include(s => s.askForUserDelyveryM.PhotoPay)
                //.Include(s => s.askForUserDelyveryM.VideoRecord)
                //.Include(s => s.Ask2)
                .Where(s => s.CurrentStatus == "Picked up" || s.CurrentStatus == "Assigned")
                .ToList();
            if (shippings == null)
            {
                return new List<Shipping>();
            }
            shippings.ForEach((item) => item.UrlReqvest = "");
            //int countFor5 = Shipping1.Count / 5;
            //int ost = Shipping1.Count % 5;
            //int countGet = ost == 0 ? (5 * type) + 5 : (5 * type) + ost;
            return shippings; //.GetRange(5 * type, countGet);
        }

        public List<Shipping> GetOrdersDelyveryForToken(string token, int type)
        {
            List<Shipping> Shipping1 = new List<Shipping>();
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            List<Shipping> shippings = context.Shipping.Where(s => driver != null && s.IdDriver == driver.Id)
                 .Include("VehiclwInformations")
                 .ToList();
            if (shippings == null)
            {
                return new List<Shipping>();
            }
            Shipping1.AddRange(shippings.FindAll(s => (s.CurrentStatus == "Delivered,Paid" || s.CurrentStatus == "Delivered,Billed" || s.CurrentStatus == "Archived,Billed" || s.CurrentStatus == "Archived,Paid") 
            && s.DataCancelOrder != null && DateTime.Parse(s.DataCancelOrder).AddDays(7) > DateTime.Now));
            Shipping1.ForEach((item) => item.UrlReqvest = "");
            //int countFor5 = Shipping1.Count / 5;
            //int ost = Shipping1.Count % 5;
            //int countGet = ost == 0 ? (5 * type) + 5 : (5 * type) + ost;
            return Shipping1; //.GetRange(5 * type, countGet);
        }

        public List<Shipping> GetOrdersArchiveForToken(string token, int type)
        {
            List<Shipping> Shipping1 = new List<Shipping>();
            Driver driver = context.Drivers.FirstOrDefault(d => d.Token == token);
            List<Shipping> shippings = context.Shipping.Where(s => driver != null && s.IdDriver == driver.Id)
                 .Include("VehiclwInformations")
                 .ToList();
            if (shippings == null)
            {
                return new List<Shipping>();
            }
            Shipping1.AddRange(shippings.FindAll(s => (s.CurrentStatus == "Delivered,Paid" || s.CurrentStatus == "Delivered,Billed" || s.CurrentStatus == "Archived,Billed" || s.CurrentStatus == "Archived,Paid") 
            && s.DataCancelOrder != null && DateTime.Parse(s.DataCancelOrder).AddDays(7) < DateTime.Now && DateTime.Parse(s.DataCancelOrder).AddDays(21) > DateTime.Now));
            Shipping1.ForEach((item) => item.UrlReqvest = "");
            //int countFor5 = Shipping1.Count / 5;
            //int ost = Shipping1.Count % 5;
            //int countGet = ost == 0 ? (5 * type) + 5 : (5 * type) + ost;
            return Shipping1; //.GetRange(5 * type, countGet);
        }
    }
}