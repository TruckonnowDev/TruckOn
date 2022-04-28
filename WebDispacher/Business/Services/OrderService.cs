﻿using System;
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
using WebDispacher.ViewModels;

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

        public async Task DeleteOrder(string id)
        {
            var shipping = await db.Shipping.FirstOrDefaultAsync(s => s.Id == id);
            if (shipping == null) return;
            
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

        public async Task ArchiveOrder(string id)
        {
            var shipping = await db.Shipping.FirstOrDefaultAsync(s => s.Id == id);
            if (shipping == null) return;
            
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
        
        public async Task<Shipping> AddNewOrder(string urlPage, Dispatcher dispatcher)
        {
            var transportationDispatch = GetTransportationDispatch(dispatcher.Type);
            
            var shipping = await transportationDispatch.GetShipping(urlPage, dispatcher);
            
            if (shipping != null)
            {
                await AddOrder(shipping);
            }
            
            return shipping;
        }
        
        public async Task SaveVechi(string idVech, string vin, string year, string make, string model, string type, string color, string lotNumber)
        {
            var vehiclwInformation = new VehiclwInformation
            {
                VIN = vin,
                Year = year,
                Make = make,
                Model = model,
                Type = type,
                Color = color,
                Lot = lotNumber
            };


            await SavevechInDb(idVech, vehiclwInformation);
        }
        
        public void AddHistory(string key, string idCompany, string idOrder, string idVech, string idDriver, string action)
        {
            var historyOrder = new HistoryOrder();
            var idUser = GetUserIdByKey(key);
            
            switch (action)
            {
                case "Assign":
                    historyOrder.TypeAction = "Assign";
                    break;
                case "Unassign":
                    idDriver = GetDriverIdByIdOrder(idOrder);
                    historyOrder.TypeAction = "Unassign";
                    break;
                case "Solved":
                    historyOrder.TypeAction = "Solved";
                    break;
                case "ArchivedOrder":
                    historyOrder.TypeAction = "ArchivedOrder";
                    break;
                case "DeletedOrder":
                    historyOrder.TypeAction = "DeletedOrder";
                    break;
                case "Creat":
                    historyOrder.TypeAction = "Creat";
                    break;
                case "SavaOrder":
                    historyOrder.TypeAction = "SavaOrder";
                    break;
                case "SavaVech":
                    idOrder = GetIdOrderByIdVech(idVech);
                    historyOrder.TypeAction = "SavaVech";
                    break;
                case "RemoveVech":
                    idOrder = GetIdOrderByIdVech(idVech);
                    historyOrder.TypeAction = "RemoveVech";
                    break;
                case "AddVech":
                    historyOrder.TypeAction = "AddVech";
                    break;
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
            var strAction = "";
            switch (action)
            {
                //int idUser = _sqlEntityFramworke.GetUserIdByKey(key);
                case "Assign":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var fullNameDriver = GetFullNameDriverById(idDriver);
                    strAction = $"{fullNameUser} assign the driver ordered {fullNameDriver}";
                    break;
                }
                case "Unassign":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var fullNameDriver = GetFullNameDriverById(idDriver);
                    strAction = $"{fullNameUser} withdrew an order from {fullNameDriver} driver";
                    break;
                }
                case "Solved":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} clicked on the \"Solved\" button";
                    break;
                }
                case "ArchivedOrder":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} transferred the order to the archive";
                    break;
                }
                case "DeletedOrder":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} transferred the order to deleted orders";
                    break;
                }
                case "Creat":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} created an order";
                    break;
                }
                case "SavaOrder":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} edited the order";
                    break;
                }
                case "SavaVech":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var vehiclwInformation = GetVechById(idVech);
                    strAction = $"{fullNameUser} edited the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Model}";
                    break;
                }
                case "RemoveVech":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var vehiclwInformation = GetVechById(idVech);
                    strAction = $"{fullNameUser} removed the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Make}";
                    break;
                }
                case "AddVech":
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} created a vehicle";
                    break;
                }
            }
            
            return strAction;
        }
        
        public void RemoveVechi(string idVech)
        {
            var vehicle = db.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == idVech);
            if (vehicle == null) return;
            
            db.VehiclwInformation.Remove(vehicle);
            
            db.SaveChanges();
        }
        
        public async Task<VehiclwInformation> AddVechi(string idOrder)
        {
            var shipping = await db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefaultAsync(s => s.Id.ToString() == idOrder);
            
            var vehicleInformation = new VehiclwInformation();
            
            if(shipping.VehiclwInformations == null)
            {
                shipping.VehiclwInformations = new List<VehiclwInformation>();
            }
            
            shipping.VehiclwInformations.Add(vehicleInformation);
            
            await db.SaveChangesAsync();
            
            return vehicleInformation;
        }
        
        public async Task<Shipping> CreateShipping()
        {
            var shipping = new Shipping
            {
                Id = CreateIdShipping().ToString(),
                CurrentStatus = "NewLoad"
            };

            db.Shipping.Add(shipping);
            await db.SaveChangesAsync();
            
            var shipping1 = db.Shipping.FirstOrDefault(s => s.Id == shipping.Id);
            
            return shipping;
        }
        
        public Shipping GetShippingCurrentVehiclwIn(string id)
        {
            return GetShipingCurrentVehiclwInDb(id);
        }
        
        public async Task Assign(string idOrder, string idDriver)
        {
            var isDriverAssign = CheckDriverOnShipping(idOrder);
            string tokenShope = null;
            
            if (isDriverAssign)
            {
                tokenShope = GerShopTokenForShipping(idOrder);
            }
            
            var vehiclwInformations = await AddDriversInOrder(idOrder, idDriver);
            
            await Task.Run(() =>
            {
                var managerNotify = new ManagerNotifyWeb();
                var tokenShope1 = GerShopTokenForShipping(idOrder);
                
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
        
        public async Task Unassign(string idOrder)
        {
            var managerNotify = new ManagerNotifyWeb();
            var tokenShope = GerShopTokenForShipping(idOrder);
            var vehiclwInformations = await RemoveDriversInOrder(idOrder);
            
            await Task.Run(() =>
            {
                managerNotify.SendNotyfyUnassign(idOrder, tokenShope, vehiclwInformations);
            });
        }
        
        public void Solved(string idOrder)
        {
            var shipping = db.Shipping.FirstOrDefault(s => s.Id == idOrder);
            if (shipping == null) return;
            
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
        
        public ShippingViewModel GetOrder(string id)
        {
            return GetShipping(id);
        }
        
        public void UpdateOrder(ShippingViewModel shipping)
        {
            UpdateOrderInDb(shipping);
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
            var managerNotifyWeb = new ManagerNotifyWeb();
            var isInspactionDriverToDay = CheckInspactionDriverToDay(idDriver);
            
            if (!isInspactionDriverToDay)
            {
                var tokenShiping = GerShopToken(idDriver.ToString());
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
            var driver = db.Drivers.Include(d => d.InspectionDrivers).FirstOrDefault(d => d.Id == idDriver);
            var inspectionDriver = driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            
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
            var driver = db.Drivers.FirstOrDefault<Driver>(d => d.Id == Convert.ToInt32(idDriver));
            
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
        
        private ShippingViewModel GetShipping(string id)
        {
            var shipping = db.Shipping.Include(s => s.VehiclwInformations).FirstOrDefault(s => s.Id == id);

            return mapper.Map<ShippingViewModel>(shipping);
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
            var countPage = 0;
            var shipping = await db.Shipping
                .Where(s => s.CurrentStatus == status
                            && (name == null || s.NameD.Contains(name) || s.NameP.Contains(name) || s.ContactNameP.Contains(name) || s.ContactNameD.Contains(name))
                            && (address == null || s.AddresP.Contains(address) || s.AddresD.Contains(address) || s.CityP.Contains(address) || s.CityD.Contains(address) || s.StateP.Contains(address.ToUpper()) || s.StateD.Contains(address.ToUpper()) || s.ZipP.Contains(address) || s.ZipD.Contains(address))
                            && (phone == null || s.PhoneP.Contains(phone) || s.PhoneD.Contains(phone) || s.PhoneC.Contains(phone))
                            && (email == null || s.EmailP.Contains(email) || s.EmailD.Contains(email))
                            && (price == null || s.PriceListed.Contains(price)))
                .ToListAsync();
            
            countPage = shipping.Count / 20;
            var remainderPage = shipping.Count % 20;
            countPage = remainderPage > 0 ? countPage + 1 : countPage;
            
            return countPage;
        }
        
        private async Task<List<VehiclwInformation>> RemoveDriversInOrder(string idOrder)
        {
            var shipping = db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.Id == idOrder);
            
            shipping.IdDriver = 0;
            shipping.CurrentStatus = "NewLoad";
            
            await db.SaveChangesAsync();
            
            return shipping.VehiclwInformations;
        }
        
        private string GerShopTokenForShipping(string idOrder)
        {
            var shipping = db.Shipping
                .FirstOrDefault(d => d.Id == idOrder);
            var driver = db.Drivers.First(d => d.Id == shipping.IdDriver);
            
            return driver.TokenShope;
        }
        
        private async Task<List<VehiclwInformation>> AddDriversInOrder(string idOrder, string idDriver)
        {
            var shipping = db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.Id == idOrder);
            var driver = db.Drivers.FirstOrDefault(d => d.Id == Convert.ToInt32(idDriver));
            
            shipping.IdDriver = driver.Id;
            shipping.CurrentStatus = "Assigned";
            
            await db.SaveChangesAsync();
            
            return shipping.VehiclwInformations;
        }
        
        private bool CheckDriverOnShipping(string idShipping)
        {
            var isDriverAssign = false;
            var shipping = db.Shipping
                .FirstOrDefault(s => s.Id == idShipping);
            
            if(db.Drivers.FirstOrDefault(d => d.Id == shipping.IdDriver) != null)
            {
                isDriverAssign = true;
            }
            
            return isDriverAssign;
        }
        
        private Shipping GetShipingCurrentVehiclwInDb(string id)
        {
            var vehiclwInformation = db.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == id);
            var shipping = db.Shipping.Where(s => s.VehiclwInformations.FirstOrDefault(v => v == vehiclwInformation) != null)
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
            var id = 1;
            
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
            var shipping = db.Shipping
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
            var shipping = db.Shipping
                .First(s => s.Id.ToString() == idOrder);
            
            return shipping.IdDriver.ToString();
        }
        
        private async Task SavevechInDb(string idVech, VehiclwInformation vehiclwInformation)
        {
            var vehiclwInformationDb = await db.VehiclwInformation.FirstOrDefaultAsync(v => v.Id.ToString() == idVech);
            
            vehiclwInformationDb.VIN = vehiclwInformation.VIN;
            vehiclwInformationDb.Year = vehiclwInformation.Year;
            vehiclwInformationDb.Make = vehiclwInformation.Make;
            vehiclwInformationDb.Model = vehiclwInformation.Model;
            vehiclwInformationDb.Type = vehiclwInformation.Type;
            vehiclwInformationDb.Color = vehiclwInformation.Color;
            vehiclwInformationDb.Lot = vehiclwInformation.Lot;
            
            await db.SaveChangesAsync();
        }
        
        private async Task AddOrder(Shipping shipping)
        {
            var isCheckOrder = CheckUrlOrder(shipping);
            if (CheckOrder(shipping) && !isCheckOrder)
            {
                shipping.Id += new Random().Next(0, 1000);
            }
            try
            {
                if (isCheckOrder) return;
                await db.Shipping.AddAsync(shipping);
                await db.SaveChangesAsync();
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
        
        private async Task UpdateOrderInDb(ShippingViewModel shipping)
        {
            var shippingEdit = db.Shipping.FirstOrDefault(s => s.Id == shipping.Id);
            if (shipping == null) return;

            if (shippingEdit == null) return; 
            
                shippingEdit.idOrder = shipping.IdOrder ?? shippingEdit.Id;

                shippingEdit.InternalLoadID = shipping.InternalLoadID ?? shippingEdit.InternalLoadID;
                switch (shipping.CurrentStatus)
                {
                    case "Delivered":
                        shippingEdit.CurrentStatus = shippingEdit.CurrentStatus.Replace("Deleted", "Delivered");
                        break;
                    case "Archived":
                        shippingEdit.CurrentStatus = shippingEdit.CurrentStatus.Replace("Archived", "Delivered");
                        break;
                    default:
                        shippingEdit.CurrentStatus = shipping.CurrentStatus ?? shippingEdit.CurrentStatus;
                        break;
                }

                shippingEdit.Titl1DI = shipping.Titl1DI ?? shippingEdit.Titl1DI;
                shippingEdit.NameP = shipping.NameP ?? shippingEdit.NameD;
                shippingEdit.ContactNameP = shipping.ContactNameP ?? shippingEdit.ContactNameP;
                shippingEdit.AddresP = shipping.AddresP ?? shippingEdit.AddresP;
                shippingEdit.CityP = shipping.CityP ?? shippingEdit.CityP;
                shippingEdit.StateP = shipping.StateP ?? shippingEdit.StateP;
                shippingEdit.ZipP = shipping.ZipP ?? shippingEdit.ZipP;
                shippingEdit.PhoneP = shipping.PhoneP ?? shippingEdit.PhoneP;
                shippingEdit.EmailP = shipping.EmailP ?? shippingEdit.EmailP;
                shippingEdit.PickupExactly = shipping.PickupExactly ?? shippingEdit.PickupExactly;
                shippingEdit.NameD = shipping.NameD ?? shippingEdit.NameD;
                shippingEdit.ContactNameD = shipping.ContactNameD ?? shippingEdit.ContactNameD;
                shippingEdit.AddresD = shipping.AddresD ?? shippingEdit.AddresD;
                shippingEdit.CityD = shipping.CityD ?? shippingEdit.CityD;
                shippingEdit.StateD = shipping.StateD ?? shippingEdit.StateD;
                shippingEdit.ZipD = shipping.ZipD ?? shippingEdit.ZipD;
                shippingEdit.PhoneD = shipping.PhoneD ?? shippingEdit.PhoneD;
                shippingEdit.EmailD = shipping.EmailD ?? shippingEdit.EmailD;
                shippingEdit.DeliveryEstimated = shipping.DeliveryEstimated ?? shippingEdit.DeliveryEstimated;
                shippingEdit.TotalPaymentToCarrier = shipping.TotalPaymentToCarrier ?? shippingEdit.TotalPaymentToCarrier;
                shippingEdit.PriceListed = shipping.PriceListed ?? shippingEdit.PriceListed;
                shippingEdit.BrokerFee = shipping.BrokerFee ?? shippingEdit.BrokerFee;
                shippingEdit.ContactC = shipping.ContactC ?? shippingEdit.ContactC;
                shippingEdit.PhoneC = shipping.PhoneC ?? shippingEdit.PhoneC;
                shippingEdit.FaxC = shipping.FaxC ?? shippingEdit.FaxC;
                shippingEdit.IccmcC = shipping.IccmcC ?? shippingEdit.IccmcC;

                await db.SaveChangesAsync();
        }
    }
}