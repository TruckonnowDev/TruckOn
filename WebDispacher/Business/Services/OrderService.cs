using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO;
using DaoModels.DAO.Models;
using Microsoft.EntityFrameworkCore;
using WebDispacher.Business.Interfaces;
using WebDispacher.Notify;
using WebDispacher.Service;
using WebDispacher.Service.TransportationManager;

namespace WebDispacher.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly StripeApi stripeApi;

        public OrderService(
            IMapper mapper,
            Context db)
        {
            this.mapper = mapper;
            this.db = db;
            stripeApi = new StripeApi();
        }

        public async void DeleteOrder(string id)
        {
            Shipping shipping = await db.Shipping.FirstOrDefaultAsync(s => s.Id == id);

            if (shipping != null)
            {
                if (shipping.CurrentStatus == "Delivered,Billed" || shipping.CurrentStatus == "Delivered,Paid")
                {
                    shipping.CurrentStatus = shipping.CurrentStatus.Replace("Delivered", "Deleted");
                }
                else
                {
                    shipping.CurrentStatus = "Deleted";
                }

                await db.SaveChangesAsync();
            }
        }

        public async void ArchiveOrder(string id)
        {
            Shipping shipping = await db.Shipping.FirstOrDefaultAsync(s => s.Id == id);

            if (shipping != null)
            {
                if (shipping.CurrentStatus == "Delivered,Billed" || shipping.CurrentStatus == "Delivered,Paid")
                {
                    shipping.CurrentStatus = shipping.CurrentStatus.Replace("Delivered", "Archived");
                }
                else
                {
                    shipping.CurrentStatus = "Archived";
                }

                await db.SaveChangesAsync();
            }
        }
        
        public async Task<Shipping> AddNewOrder(string urlPage, Dispatcher dispatcher)
        {
            ITransportationDispatch transportationDispatch = GetTransportationDispatch(dispatcher.Type);
            
            Shipping shipping = await transportationDispatch.GetShipping(urlPage, dispatcher);
            
            if (shipping != null)
            {
                AddOrder(shipping);
            }
            
            return shipping;
        }
        
        public async void SaveVechi(string idVech, string vin, string year, string make, string model, string type, string color, string lotNumber)
        {
            VehiclwInformation vehiclwInformation = new VehiclwInformation();
            
            vehiclwInformation.VIN = vin;
            vehiclwInformation.Year = year;
            vehiclwInformation.Make = make;
            vehiclwInformation.Model = model;
            vehiclwInformation.Type = type;
            vehiclwInformation.Color = color;
            vehiclwInformation.Lot = lotNumber;
            
            SavevechInDb(idVech, vehiclwInformation);
        }
        
        public void AddHistory(string key, string idCompany, string idOrder, string idVech, string idDriver, string action)
        {
            HistoryOrder historyOrder = new HistoryOrder();
            int idUser = GetUserIdByKey(key);
            if(action == "Assign")
            {
                historyOrder.TypeAction = "Assign";
            }
            else if(action == "Unassign")
            {
                idDriver = GetDriverIdByIdOrder(idOrder);
                historyOrder.TypeAction = "Unassign";
            }
            else if (action == "Solved")
            {
                historyOrder.TypeAction = "Solved";
            }
            else if (action == "ArchivedOrder")
            {
                historyOrder.TypeAction = "ArchivedOrder";
            }
            else if (action == "DeletedOrder")
            {
                historyOrder.TypeAction = "DeletedOrder";
            }
            else if (action == "Creat")
            {
                historyOrder.TypeAction = "Creat";
            }
            else if (action == "SavaOrder")
            {
                historyOrder.TypeAction = "SavaOrder";
            }
            else if (action == "SavaVech")
            {
                idOrder = GetIdOrderByIdVech(idVech);
                historyOrder.TypeAction = "SavaVech";
            }
            else if (action == "RemoveVech")
            {
                idOrder = GetIdOrderByIdVech(idVech);
                historyOrder.TypeAction = "RemoveVech";
            }
            else if (action == "AddVech")
            {
                historyOrder.TypeAction = "AddVech";
            }

            historyOrder.IdConmpany = Convert.ToInt32(idCompany);
            historyOrder.IdDriver = Convert.ToInt32(idDriver);
            historyOrder.IdOreder = idOrder;
            historyOrder.IdVech = Convert.ToInt32(idVech);
            historyOrder.IdUser = idUser;
            historyOrder.DateAction = DateTime.Now.ToString();
            
            AddHistoryInDb(historyOrder);
        }
        
        public string GetStrAction(string key, string idCompany, string idOrder, string idVech, string idDriver, string action)
        {
            string strAction = "";
            //int idUser = _sqlEntityFramworke.GetUserIdByKey(key);
            if (action == "Assign")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                string fullNameDriver = GetFullNameDriverById(idDriver);
                strAction = $"{fullNameUser} assign the driver ordered {fullNameDriver}";
            }
            else if (action == "Unassign")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                string fullNameDriver = GetFullNameDriverById(idDriver);
                strAction = $"{fullNameUser} withdrew an order from {fullNameDriver} driver";
            }
            else if (action == "Solved")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} clicked on the \"Solved\" button";
            }
            else if (action == "ArchivedOrder")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} transferred the order to the archive";
            }
            else if (action == "DeletedOrder")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} transferred the order to deleted orders";
            }
            else if (action == "Creat")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} created an order";
            }
            else if (action == "SavaOrder")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} edited the order";
            }
            else if (action == "SavaVech")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                VehiclwInformation vehiclwInformation = GetVechById(idVech);
                strAction = $"{fullNameUser} edited the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Model}";
            }
            else if (action == "RemoveVech")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                VehiclwInformation vehiclwInformation = GetVechById(idVech);
                strAction = $"{fullNameUser} removed the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Make}";
            }
            else if (action == "AddVech")
            {
                string fullNameUser = GetFullNameUserByKey(key);
                strAction = $"{fullNameUser} created a vehicle";
            }
            
            return strAction;
        }
        
        public void RemoveVechi(string idVech)
        {
            db.VehiclwInformation.Remove(db.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == idVech));
            db.SaveChanges();
        }
        
        public async Task<VehiclwInformation> AddVechi(string idOrder)
        {
            Shipping shipping = await db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefaultAsync(s => s.Id.ToString() == idOrder);
            
            VehiclwInformation vehiclwInformation = new VehiclwInformation();
            
            if(shipping.VehiclwInformations == null)
            {
                shipping.VehiclwInformations = new List<VehiclwInformation>();
            }
            
            shipping.VehiclwInformations.Add(vehiclwInformation);
            
            await db.SaveChangesAsync();
            
            return vehiclwInformation;
        }
        
        public async Task<Shipping> CreateShipping()
        {
            Shipping shipping = new Shipping();
            
            shipping.Id = CreateIdShipping().ToString();
            shipping.CurrentStatus = "NewLoad";
            db.Shipping.Add(shipping);
            await db.SaveChangesAsync();
            
            Shipping shipping1 = db.Shipping.FirstOrDefault(s => s.Id == shipping.Id);
            
            return shipping;
        }
        
        public Shipping GetShippingCurrentVehiclwIn(string id)
        {
            return GetShipingCurrentVehiclwInDb(id);
        }
        
        public async void Assign(string idOrder, string idDriver)
        {
            bool isDriverAssign = CheckDriverOnShipping(idOrder);
            string tokenShope = null;
            
            if (isDriverAssign)
            {
                tokenShope = GerShopTokenForShipping(idOrder);
            }
            
            List<VehiclwInformation> vehiclwInformations = await AddDriversInOrder(idOrder, idDriver);
            
            Task.Run(() =>
            {
                ManagerNotifyWeb managerNotify = new ManagerNotifyWeb();
                string tokenShope1 = GerShopTokenForShipping(idOrder);
                
                if (!isDriverAssign)
                {
                    managerNotify.SendNotyfyAssign(idOrder, tokenShope1, vehiclwInformations);
                }
                else
                {
                    managerNotify.SendNotyfyAssign(idOrder, tokenShope1, vehiclwInformations);
                    managerNotify.SendNotyfyUnassign(idOrder, tokenShope, vehiclwInformations);
                }
            });
        }
        
        public async void Unassign(string idOrder)
        {
            ManagerNotifyWeb managerNotify = new ManagerNotifyWeb();
            string tokenShope = GerShopTokenForShipping(idOrder);
            List<VehiclwInformation> vehiclwInformations = await RemoveDriversInOrder(idOrder);
            
            Task.Run(() =>
            {
                managerNotify.SendNotyfyUnassign(idOrder, tokenShope, vehiclwInformations);
            });
        }
        
        public void Solved(string idOrder)
        {
            Shipping shipping = db.Shipping.First(s => s.Id == idOrder);
            shipping.IsProblem = false;
            
            db.SaveChanges();
        }
        
        public async Task<int> GetCountPage(string status, string name, string address, string phone, string email, string price)
        {
            return await GetCountPageInDb(status, name, address, phone, email, price);
        }
        
        public async Task<List<Shipping>> GetOrders(string status, int page, string name, string address, string phone, string email, string price)
        {
            return await GetShippings(status, page, name, address, phone, email, price);
        }
        
        public Shipping GetOrder(string id)
        {
            return GetShipping(id);
        }
        
        public void UpdateOrder(string idOrder, string idLoad, string internalLoadID, string driver, string status, string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP, string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD, string paymentMethod, string price, string paymentTerms, string brokerFee,
            string contactId, string phoneC, string faxC, string iccmcC)
        {
            UpdateOrderInDb(idOrder, idLoad, internalLoadID, driver, status, instructions, nameP, contactP, addressP, cityP, stateP, zipP,
                phoneP, emailP, scheduledPickupDateP, nameD, contactD, addressD, cityD, stateD, zipD, phoneD, emailD, ScheduledPickupDateD, paymentMethod,
                price, paymentTerms, brokerFee, contactId, phoneC, faxC, iccmcC);
        }
        
        public void SavePath(string id, string path)
        {
            SavePathDb(id, path);
        }
        
        public async Task<string> GetDocument(string id)
        {
            return GetDocumentDb(id);
        }
        
        public void RemoveDoc(string idDock)
        {
            RemoveDocDb(idDock);
        }
        
        public bool SendRemindInspection(int idDriver)
        {
            ManagerNotifyWeb managerNotifyWeb = new ManagerNotifyWeb();
            bool isInspactionDriverToDay = CheckInspactionDriverToDay(idDriver);
            
            if (!isInspactionDriverToDay)
            {
                string tokenShiping = GerShopToken(idDriver.ToString());
                managerNotifyWeb.SendSendNotyfyRemindInspection(tokenShiping);
            }
            
            return isInspactionDriverToDay;
        }
        
        public List<HistoryOrder> GetHistoryOrder(string idOrder)
        {
            return GetHistoryOrderByIdOrder(idOrder);
        }
        
        private List<HistoryOrder> GetHistoryOrderByIdOrder(string idOrder)
        {
            return db.HistoryOrders.Where(ho => ho.IdOreder.ToString() == idOrder).ToList();
        }
        
        private bool CheckInspactionDriverToDay(int idDriver)
        {
            Driver driver = db.Drivers.Include(d => d.InspectionDrivers).FirstOrDefault(d => d.Id == idDriver);
            InspectionDriver inspectionDriver = driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            
            if (inspectionDriver == null)
            {
                driver.IsInspectionDriver = false;
                driver.IsInspectionToDayDriver = false;
                db.SaveChanges();
            }
            else if (Convert.ToDateTime(inspectionDriver.Date).Date != DateTime.Now.Date)
            {
                if (DateTime.Now.Hour >= 12)
                {
                    driver.IsInspectionDriver = false;
                    driver.IsInspectionToDayDriver = false;
                }
                else if (DateTime.Now.Hour <= 12 && 6 >= DateTime.Now.Hour)
                {
                    driver.IsInspectionDriver = true;
                    driver.IsInspectionToDayDriver = false;
                }
                db.SaveChanges();
            }
            
            return driver.IsInspectionToDayDriver;
        }
        
        private string GerShopToken(string idDriver)
        {
            Driver driver = db.Drivers.FirstOrDefault<Driver>(d => d.Id == Convert.ToInt32(idDriver));
            return driver.TokenShope;
        }
        
        private void RemoveDocDb(string idDock)
        {
            db.DocumentTruckAndTrailers.Remove(db.DocumentTruckAndTrailers.First(d => d.Id.ToString() == idDock));
            db.SaveChanges();
        }
        
        private string GetDocumentDb(string id)
        {
            //string pathDoc = "";
            //Driver driver = context.Drivers
            //    .Include(d => d.InspectionDrivers)
            //    .FirstOrDefault(d => d.Id.ToString() == id);
            //if (driver.InspectionDrivers != null)
            //{
            //    InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
            //    Truck truck = context.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck);
            //    if (truck != null)
            //    {
            //        pathDoc = truck.PathDoc;
            //    }
            //}
            return "";
        }
        
        private void SavePathDb(string id, string path)
        {
            //Truck truck = context.Trucks.First(t => t.Id.ToString() == id);
            //truck.PathDoc = path;
            //context.SaveChanges();
        }
        
        private Shipping GetShipping(string id)
        {
            return db.Shipping.Include(s => s.VehiclwInformations).FirstOrDefault(s => s.Id == id);
        }
        
        private async Task<List<Shipping>> GetShippings(string status, int page, string name, string address, string phone, string email, string price)
        {
            List<Shipping> shipping = null;
            shipping = await db.Shipping
                .Where(s => s.CurrentStatus == status
                            && (name == null || s.NameD.Contains(name) || s.NameP.Contains(name) || s.ContactNameP.Contains(name) || s.ContactNameD.Contains(name))
                            && (address == null || s.AddresP.Contains(address) || s.AddresD.Contains(address) || s.CityP.Contains(address) || s.CityD.Contains(address) || s.StateP.Contains(address.ToUpper()) || s.StateD.Contains(address.ToUpper()) || s.ZipP.Contains(address) || s.ZipD.Contains(address))
                            && (phone == null || s.PhoneP.Contains(phone) || s.PhoneD.Contains(phone) || s.PhoneC.Contains(phone))
                            && (email == null || s.EmailP.Contains(email) || s.EmailD.Contains(email))
                            && (price == null || s.PriceListed.Contains(price)))
                .ToListAsync();
            
            if (page != 0)
            {
                try
                {
                    shipping = shipping.GetRange((20 * page) - 20, 20);
                }
                catch(Exception)
                {
                    shipping = shipping.GetRange((20 * page) - 20, shipping.Count % 20);
                }
            }
            else
            {
                try
                {
                    shipping = shipping.GetRange(0, 20);
                }
                catch (Exception)
                {
                    shipping = shipping.GetRange(0, shipping.Count % 20);
                }
            }
            return shipping;
        }
        
        private async Task<int> GetCountPageInDb(string status, string name, string address, string phone, string email, string price)
        {
            int countPage = 0;
            List<Shipping> shipping = await db.Shipping
                .Where(s => s.CurrentStatus == status
                            && (name == null || s.NameD.Contains(name) || s.NameP.Contains(name) || s.ContactNameP.Contains(name) || s.ContactNameD.Contains(name))
                            && (address == null || s.AddresP.Contains(address) || s.AddresD.Contains(address) || s.CityP.Contains(address) || s.CityD.Contains(address) || s.StateP.Contains(address.ToUpper()) || s.StateD.Contains(address.ToUpper()) || s.ZipP.Contains(address) || s.ZipD.Contains(address))
                            && (phone == null || s.PhoneP.Contains(phone) || s.PhoneD.Contains(phone) || s.PhoneC.Contains(phone))
                            && (email == null || s.EmailP.Contains(email) || s.EmailD.Contains(email))
                            && (price == null || s.PriceListed.Contains(price)))
                .ToListAsync();
            
            countPage = shipping.Count / 20;
            int remainderPage = shipping.Count % 20;
            countPage = remainderPage > 0 ? countPage + 1 : countPage;
            
            return countPage;
        }
        
        private async Task<List<VehiclwInformation>> RemoveDriversInOrder(string idOrder)
        {
            Shipping shipping = db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.Id == idOrder);
            
            shipping.IdDriver = 0;
            shipping.CurrentStatus = "NewLoad";
            
            await db.SaveChangesAsync();
            
            return shipping.VehiclwInformations;
        }
        
        private string GerShopTokenForShipping(string idOrder)
        {
            Shipping shipping = db.Shipping
                .FirstOrDefault(d => d.Id == idOrder);
            Driver driver = db.Drivers.First(d => d.Id == shipping.IdDriver);
            
            return driver.TokenShope;
        }
        
        private async Task<List<VehiclwInformation>> AddDriversInOrder(string idOrder, string idDriver)
        {
            Shipping shipping = db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.Id == idOrder);
            Driver driver = db.Drivers.FirstOrDefault(d => d.Id == Convert.ToInt32(idDriver));
            
            shipping.IdDriver = driver.Id;
            shipping.CurrentStatus = "Assigned";
            
            await db.SaveChangesAsync();
            
            return shipping.VehiclwInformations;
        }
        
        private bool CheckDriverOnShipping(string idShipping)
        {
            bool isDriverAssign = false;
            Shipping shipping = db.Shipping
                .FirstOrDefault(s => s.Id == idShipping);
            if(db.Drivers.FirstOrDefault(d => d.Id == shipping.IdDriver) != null)
            {
                isDriverAssign = true;
            }
            return isDriverAssign;
        }
        
        private Shipping GetShipingCurrentVehiclwInDb(string id)
        {
            VehiclwInformation vehiclwInformation = db.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == id);
            Shipping shipping = db.Shipping.Where(s => s.VehiclwInformations.FirstOrDefault(v => v == vehiclwInformation) != null)
                .Include(s => s.VehiclwInformations)
                .Include("VehiclwInformations.Ask")
                .Include("VehiclwInformations.Ask.Any_personal_or_additional_items_with_or_in_vehicle")
                .Include("VehiclwInformations.Ask1")
                .Include("VehiclwInformations.Ask1.App_will_force_driver_to_take_pictures_of_each_strap")
                .Include("VehiclwInformations.Ask1.Photo_after_loading_in_the_truck")
                .Include("VehiclwInformations.AskDelyvery")
                .Include("VehiclwInformations.AskDelyvery.Please_take_a_picture_Id_of_the_person_taking_the_delivery")
                .Include("VehiclwInformations.PhotoInspections.Photos")
                .Include(v => v.AskFromUser)
                .Include(v => v.AskFromUser.App_will_ask_for_signature_of_the_client_signature)
                .Include(v => v.AskFromUser.PhotoPay)
                .Include(v => v.AskFromUser.VideoRecord)
                .Include(v => v.AskFromUser.App_will_ask_for_signature_of_the_client_signature)
                .Include(v => v.askForUserDelyveryM)
                .Include(v => v.askForUserDelyveryM.App_will_ask_for_signature_of_the_client_signature)
                .Include(v => v.askForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo)
                .Include(v => v.askForUserDelyveryM.PhotoPay)
                .Include(v => v.askForUserDelyveryM.VideoRecord)
                .Include("VehiclwInformations.Scan")
                .Include(v => v.Ask2)
                .FirstOrDefault();
            return db.Shipping.FirstOrDefault(s => s.VehiclwInformations.FirstOrDefault(v => v == vehiclwInformation) != null);
        }
        
        private int CreateIdShipping()
        {
            int id = 1;
            
            while(db.Shipping.FirstOrDefault(s => s.Id == id.ToString()) != null) { id = new Random().Next(0, 100000000); }
            
            return id;
        }
        
        private string GetFullNameUserByKey(string key)
        {
            return db.User.First(u => u.KeyAuthorized == key).Login;
        }
        
        private VehiclwInformation GetVechById(string idVech)
        {
            return db.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == idVech);
        }
        
        private string GetFullNameDriverById(string idDriver)
        {
            return db.Drivers.First(d => d.Id.ToString() == idDriver).FullName;
        }
        
        private string GetIdOrderByIdVech(string idVech)
        {
            Shipping shipping = db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.VehiclwInformations.FirstOrDefault(v => v.Id.ToString() == idVech) != null);
            
            return shipping.Id;
        }
        
        private void AddHistoryInDb(HistoryOrder historyOrder)
        {
            db.HistoryOrders.Add(historyOrder);
            db.SaveChanges();
        }
        
        private int GetUserIdByKey(string key)
        {
            return db.User.First(u => u.KeyAuthorized == key).Id;
        }
        
        private string GetDriverIdByIdOrder(string idOrder)
        {
            Shipping shipping = db.Shipping
                .First(s => s.Id.ToString() == idOrder);
            
            return shipping.IdDriver.ToString();
        }
        
        private void SavevechInDb(string idVech, VehiclwInformation vehiclwInformation)
        {
            VehiclwInformation vehiclwInformationDb = db.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == idVech);
            
            vehiclwInformationDb.VIN = vehiclwInformation.VIN;
            vehiclwInformationDb.Year = vehiclwInformation.Year;
            vehiclwInformationDb.Make = vehiclwInformation.Make;
            vehiclwInformationDb.Model = vehiclwInformation.Model;
            vehiclwInformationDb.Type = vehiclwInformation.Type;
            vehiclwInformationDb.Color = vehiclwInformation.Color;
            vehiclwInformationDb.Lot = vehiclwInformation.Lot;
            
            db.SaveChanges();
        }
        
        private async void AddOrder(Shipping shipping)
        {
            bool isCheckOrder = CheckUrlOrder(shipping);
            if (CheckOrder(shipping) && !isCheckOrder)
            {
                shipping.Id += new Random().Next(0, 1000);
            }
            try
            {
                if (!isCheckOrder)
                {
                    await db.Shipping.AddAsync(shipping);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
            }
        }
        
        private bool CheckUrlOrder(Shipping shipping)
        {
            return db.Shipping.FirstOrDefault(s => s.UrlReqvest == shipping.UrlReqvest) != null;
        }
        
        private bool CheckOrder(Shipping shipping)
        {
            return db.Shipping.FirstOrDefault(s => s.Id == shipping.Id) != null;
        }
        
        private ITransportationDispatch GetTransportationDispatch(string typeDispatch)
        {
            ITransportationDispatch transportationDispatch = null;
            
            switch (typeDispatch)
            {
                case "Central Dispatch": transportationDispatch = new GetDataCentralDispatch(); break;
            }
            
            return transportationDispatch;
        }
        
        private async void UpdateOrderInDb(string idOrder, string idLoad, string internalLoadID, string driver, string status, string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP, string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD, string paymentMethod, string price, string paymentTerms, string brokerFee,
            string contactId, string phoneC, string faxC, string iccmcC)
        {
            Shipping shipping = db.Shipping.FirstOrDefault(s => s.Id == idOrder);
            shipping.idOrder = idLoad != null ? idLoad : shipping.Id;
            shipping.InternalLoadID = internalLoadID != null ? internalLoadID : shipping.InternalLoadID;
            //shipping.Driverr = internalLoadID != null ? internalLoadID : shipping.InternalLoadID;
            if(status == "Delivered")
            {
                shipping.CurrentStatus = shipping.CurrentStatus.Replace("Deleted", "Delivered");
            }
            else if(status == "Archived")
            {
                shipping.CurrentStatus = shipping.CurrentStatus.Replace("Archived", "Delivered");
            }
            else
            {
                shipping.CurrentStatus = status != null ? status : shipping.CurrentStatus;
            }
            shipping.Titl1DI = instructions != null ? instructions : shipping.Titl1DI;
            shipping.NameP = nameP != null ? nameP : shipping.NameD;
            shipping.ContactNameP = contactP != null ? contactP : shipping.ContactNameP;
            shipping.AddresP = addressP != null ? addressP : shipping.AddresP;
            shipping.CityP = cityP != null ? cityP : shipping.CityP;
            shipping.StateP = stateP != null ? stateP : shipping.StateP;
            shipping.ZipP = zipP != null ? zipP : shipping.ZipP;
            shipping.PhoneP = phoneP != null ? phoneP : shipping.PhoneP;
            shipping.EmailP = emailP != null ? emailP : shipping.EmailP;
            shipping.PickupExactly = scheduledPickupDateP != null ? scheduledPickupDateP : shipping.PickupExactly;
            shipping.NameD = nameD != null ? nameD : shipping.NameD;
            shipping.ContactNameD = contactD != null ? contactD : shipping.ContactNameD;
            shipping.AddresD = addressD != null ? addressD : shipping.AddresD;
            shipping.CityD = cityD != null ? cityD : shipping.CityD;
            shipping.StateD = stateD != null ? stateD : shipping.StateD;
            shipping.ZipD = zipD != null ? zipD : shipping.ZipD;
            shipping.PhoneD = phoneD != null ? phoneD : shipping.PhoneD;
            shipping.EmailD = emailD != null ? emailD : shipping.EmailD;
            shipping.DeliveryEstimated = ScheduledPickupDateD != null ? ScheduledPickupDateD : shipping.DeliveryEstimated;
            shipping.TotalPaymentToCarrier = paymentMethod != null ? paymentMethod : shipping.TotalPaymentToCarrier;
            shipping.PriceListed = price != null ? price : shipping.PriceListed;
            shipping.BrokerFee = brokerFee != null ? brokerFee : shipping.BrokerFee;
            shipping.ContactC = contactId != null ? contactId : shipping.ContactC;
            shipping.PhoneC = phoneC != null ? phoneC : shipping.PhoneC;
            shipping.FaxC = faxC != null ? faxC : shipping.FaxC;
            shipping.IccmcC = iccmcC != null ? iccmcC : shipping.IccmcC;
            
            await db.SaveChangesAsync();
        }
    }
}