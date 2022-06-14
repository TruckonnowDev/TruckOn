using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO;
using DaoModels.DAO.Models;
using Microsoft.EntityFrameworkCore;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
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
            
            if (shipping.CurrentStatus == OrderConstants.OrderStatusDeliveredBilled || shipping.CurrentStatus == OrderConstants.OrderStatusDeliveredPaid)
            {
                shipping.CurrentStatus = shipping.CurrentStatus.Replace(OrderConstants.OrderStatusDelivered, OrderConstants.OrderStatusDeleted);
            }
            else
            {
                shipping.CurrentStatus = OrderConstants.OrderStatusDeleted;
            }

            await db.SaveChangesAsync();
        }

        public async Task ArchiveOrder(string id)
        {
            var shipping = await db.Shipping.FirstOrDefaultAsync(s => s.Id == id);

            if (shipping == null) return;
            
            if (shipping.CurrentStatus == OrderConstants.OrderStatusDeliveredBilled
                || shipping.CurrentStatus == OrderConstants.OrderStatusDeliveredPaid)
            {
                shipping.CurrentStatus = shipping.CurrentStatus
                    .Replace(OrderConstants.OrderStatusDelivered, OrderConstants.OrderStatusArchived);
            }
            else
            {
                shipping.CurrentStatus = OrderConstants.OrderStatusArchived;
            }

            await db.SaveChangesAsync();
        }
        
        public async Task<Shipping> AddNewOrder(string urlPage, Dispatcher dispatcher)
        {
            var transportationDispatch = GetTransportationDispatch(dispatcher.Type);
            
            var shipping = await transportationDispatch.GetShipping(urlPage, dispatcher);
            
            if (shipping != null)
            {
                shipping.PriceListed = shipping.PriceListed.Replace("$", "");

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
                case OrderConstants.ActionAssign:
                    historyOrder.TypeAction = OrderConstants.ActionAssign;
                    break;
                case OrderConstants.ActionUnAssign:
                    idDriver = GetDriverIdByIdOrder(idOrder);
                    historyOrder.TypeAction = OrderConstants.ActionUnAssign;
                    break;
                case OrderConstants.ActionSolved:
                    historyOrder.TypeAction = OrderConstants.ActionSolved;
                    break;
                case OrderConstants.ActionArchivedOrder:
                    historyOrder.TypeAction = OrderConstants.ActionArchivedOrder;
                    break;
                case OrderConstants.ActionDeletedOrder:
                    historyOrder.TypeAction = OrderConstants.ActionDeletedOrder;
                    break;
                case OrderConstants.ActionCreate:
                    historyOrder.TypeAction = OrderConstants.ActionCreate;
                    break;
                case OrderConstants.ActionSaveOrder:
                    historyOrder.TypeAction = OrderConstants.ActionSaveOrder;
                    break;
                case OrderConstants.ActionSaveVech:
                    idOrder = GetIdOrderByIdVech(idVech);
                    historyOrder.TypeAction = OrderConstants.ActionSaveVech;
                    break;
                case OrderConstants.ActionRemoveVech:
                    idOrder = GetIdOrderByIdVech(idVech);
                    historyOrder.TypeAction = OrderConstants.ActionRemoveVech;
                    break;
                case OrderConstants.ActionAddVech:
                    historyOrder.TypeAction = OrderConstants.ActionAddVech;
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
            var strAction = string.Empty;
            switch (action)
            {
                //int idUser = _sqlEntityFramworke.GetUserIdByKey(key);
                case OrderConstants.ActionAssign:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var fullNameDriver = GetFullNameDriverById(idDriver);
                    strAction = $"{fullNameUser} assign the driver ordered {fullNameDriver}";
                    break;
                }
                case OrderConstants.ActionUnAssign:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var fullNameDriver = GetFullNameDriverById(idDriver);
                    strAction = $"{fullNameUser} withdrew an order from {fullNameDriver} driver";
                    break;
                }
                case OrderConstants.ActionSolved:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} clicked on the \"Solved\" button";
                    break;
                }
                case OrderConstants.ActionArchivedOrder:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} transferred the order to the archive";
                    break;
                }
                case OrderConstants.ActionDeletedOrder:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} transferred the order to deleted orders";
                    break;
                }
                case OrderConstants.ActionCreate:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} created an order";
                    break;
                }
                case OrderConstants.ActionSaveOrder:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    strAction = $"{fullNameUser} edited the order";
                    break;
                }
                case OrderConstants.ActionSaveVech:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var vehiclwInformation = GetVechById(idVech);
                    strAction = $"{fullNameUser} edited the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Model}";
                    break;
                }
                case OrderConstants.ActionRemoveVech:
                {
                    var fullNameUser = GetFullNameUserByKey(key);
                    var vehiclwInformation = GetVechById(idVech);
                    strAction = $"{fullNameUser} removed the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Make}";
                    break;
                }
                case OrderConstants.ActionAddVech:
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
                CurrentStatus = OrderConstants.OrderStatusNewLoad
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
        
        public async Task<int> GetCountPage(string status, string loadId, string name, string address, string phone,
            string email, string price)
        {
            return await GetCountPageInDb(status, loadId, name, address, phone, email, price);
        }

        public int GetCountPage(int countPage)
        {
            var remainderPage = countPage % 20;
                    
            return  remainderPage > 0 ? countPage + 1 : countPage;
        }
        
        public async Task<List<Shipping>> GetOrders(string status, int page, string loadId ,string name, string address,
            string phone, string email, string price)
        {
            return await GetShippings(status, page, loadId, name, address, phone, email, price);
        }
        
        public ShippingViewModel GetOrder(string id)
        {
            return GetShipping(id);
        }
        
        public async Task<ShippingViewModel> UpdateOrder(ShippingViewModel shipping)
        {
           return await UpdateOrderInDb(shipping);
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
            var inspectionDriver = driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0 
                ? driver.InspectionDrivers.Last() : null;
            
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
            return string.Empty;
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

        private async Task<List<Shipping>> GetShippings(string status, int page, string loadId, string name, string address, string phone, string email, string price)
        {
            var qShippings = db.Shipping.Where(x => x.CurrentStatus == status).AsQueryable();
        
            if (loadId != null)
            {
                qShippings = qShippings.Where(x =>
                    x.idOrder.Contains(loadId));
            }
            
            if (name != null)
            {
                qShippings = qShippings.Where(x =>
                    x.NameD.Contains(name) || x.NameP.Contains(name) || x.ContactNameP.Contains(name) ||
                    x.ContactNameD.Contains(name));
            }
        
            if (address != null)
            {
                qShippings = qShippings.Where(x =>
                    x.AddresP.Contains(address) || x.AddresD.Contains(address) || x.CityP.Contains(address) ||
                    x.CityD.Contains(address));
            }
        
            if (phone != null)
            {
                qShippings = qShippings.Where(x =>
                    x.PhoneP.Contains(phone) || x.PhoneD.Contains(phone) || x.PhoneC.Contains(phone));
            }
        
            if (email != null)
            {
                qShippings = qShippings.Where(x => x.EmailP.Contains(email) || x.EmailD.Contains(email));
            }
        
            if (price != null)
            {
                qShippings = qShippings.Where(x => x.PriceListed.Contains(price));
            }
            
            if (page != 0)
            {
                try
                {
                    qShippings = qShippings.Skip(20 * page - 20);
        
                    qShippings = qShippings.Take(20);
                }
                catch(Exception)
                {
                    qShippings = qShippings.Skip((20 * page) - 20);
                }
            }
            else
            {
                try
                {
                    qShippings = qShippings.Take(20);
                }
                catch (Exception)
                {
                }
            }
        
            var listShippings = await qShippings.ToListAsync();
            
            return listShippings;
        }
        
        private async Task<int> GetCountPageInDb(string status, string loadId, string name, string address, string phone, string email, string price)
        {
            var qShippings = db.Shipping.Where(x => x.CurrentStatus == status).AsQueryable();
        
            if (loadId != null)
            {
                qShippings = qShippings.Where(x =>
                    x.idOrder.Contains(loadId));
            }
            
            if (name != null)
            {
                qShippings = qShippings.Where(x =>
                    x.NameD.Contains(name) || x.NameP.Contains(name) || x.ContactNameP.Contains(name) ||
                    x.ContactNameD.Contains(name));
            }
        
            if (address != null)
            {
                qShippings = qShippings.Where(x =>
                    x.AddresP.Contains(address) || x.AddresD.Contains(address) || x.CityP.Contains(address) ||
                    x.CityD.Contains(address));
            }
        
            if (phone != null)
            {
                qShippings = qShippings.Where(x =>
                    x.PhoneP.Contains(phone) || x.PhoneD.Contains(phone) || x.PhoneC.Contains(phone));
            }
        
            if (email != null)
            {
                qShippings = qShippings.Where(x => x.EmailP.Contains(email) || x.EmailD.Contains(email));
            }
        
            if (price != null)
            {
                qShippings = qShippings.Where(x => x.PriceListed.Contains(price));
            }
        
            var countShippings = qShippings.Count();
            
            var countPage = countShippings / 20;
            
            return countPage;
        }

        private async Task<List<VehiclwInformation>> RemoveDriversInOrder(string idOrder)
        {
            var shipping = db.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.Id == idOrder);
            
            shipping.IdDriver = 0;
            shipping.CurrentStatus = OrderConstants.OrderStatusNewLoad;
            
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
            shipping.CurrentStatus = OrderConstants.OrderStatusAssigned;
            
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
                case  OrderConstants.CentralDispatch: transportationDispatch = new GetDataCentralDispatch(); break;
            }
            
            return transportationDispatch;
        }
        
        private async Task<ShippingViewModel> UpdateOrderInDb(ShippingViewModel shipping)
        {
            if (shipping == null) return null;
            
            var shippingEdit = db.Shipping.FirstOrDefault(s => s.Id == shipping.Id);

            if (shippingEdit != null)
            {

                shippingEdit.idOrder = shipping.IdOrder ?? shippingEdit.Id;

                shippingEdit.InternalLoadID = shipping.InternalLoadID ?? shippingEdit.InternalLoadID;
                switch (shipping.CurrentStatus)
                {
                    case OrderConstants.OrderStatusDelivered:
                        shippingEdit.CurrentStatus = shippingEdit.CurrentStatus
                            .Replace(OrderConstants.OrderStatusDeleted, OrderConstants.OrderStatusDelivered);
                        break;
                    case OrderConstants.OrderStatusArchived:
                        shippingEdit.CurrentStatus = shippingEdit.CurrentStatus
                            .Replace(OrderConstants.OrderStatusArchived, OrderConstants.OrderStatusDelivered);
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
                shippingEdit.TotalPaymentToCarrier =
                    shipping.TotalPaymentToCarrier ?? shippingEdit.TotalPaymentToCarrier;
                shippingEdit.PriceListed = shipping.PriceListed ?? shippingEdit.PriceListed;
                shippingEdit.BrokerFee = shipping.BrokerFee ?? shippingEdit.BrokerFee;
                shippingEdit.ContactC = shipping.ContactC ?? shippingEdit.ContactC;
                shippingEdit.PhoneC = shipping.PhoneC ?? shippingEdit.PhoneC;
                shippingEdit.FaxC = shipping.FaxC ?? shippingEdit.FaxC;
                shippingEdit.IccmcC = shipping.IccmcC ?? shippingEdit.IccmcC;
                
                await db.SaveChangesAsync();
            
            }

            var updatedOrder = db.Shipping.FirstOrDefault(x => x.Id == shipping.Id);

            return mapper.Map<ShippingViewModel>(updatedOrder);
        }
    }
}