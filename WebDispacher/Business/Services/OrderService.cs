using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.EntityFrameworkCore;
using WebDispacher.Attributes;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Notify;
/*using WebDispacher.Notify;*/
using WebDispacher.Service;
using WebDispacher.Service.TransportationManager;
using WebDispacher.ViewModels.Order;

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

        public async Task DeleteOrder(int id, string localDate)
        {
            var order = await db.Orders
                .Include(o => o.CurrentStatus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (order == null) return;

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);
            var currentStatusId = order.CurrentStatusId;

            if (order.CurrentStatus.StatusName == OrderConstants.OrderStatusDeliveredBilled || order.CurrentStatus.StatusName == OrderConstants.OrderStatusDeliveredPaid)
            {
                order.CurrentStatusId = GetCurrentStatusIdByName(OrderConstants.OrderStatusDeleted);
            }
            else
            {
                order.CurrentStatusId = GetCurrentStatusIdByName(OrderConstants.OrderStatusDeleted);
            }

            if (currentStatusId != order.CurrentStatusId)
            {
                await db.HistoriesOrdersActions.AddAsync(new HistoryOrderAction
                {
                    OrderId = order.Id,
                    ActionType = ActionType.Delete,
                    DateTimeAction = dateTimeUpdate,
                    FieldAction = "Current Status",
                    ContentFrom = GetCurrentStatusById(currentStatusId).StatusName,
                    ContentTo = GetCurrentStatusById(order.CurrentStatusId).StatusName,
                });
            }

            order.DateTimeLastUpdate = dateTimeUpdate;

            await db.SaveChangesAsync();
        }

        public async Task<List<VehicleDetails>> GetVehicleDetailsByOrderId(int orderId)
        {
            var vehicles = await db.VehiclesDetails
                .Include(o => o.VehicleModel)
                .ThenInclude(o => o.VehicleBrand)
                .ThenInclude(o => o.VehicleType)
                .Include(o => o.VehicleModel)
                .ThenInclude(o => o.VehicleBody)
                .Where(vd => vd.OrderId == orderId)
                .ToListAsync();

            return vehicles;
        }

        public VehicleDetails GetVehicleDetailsById(int id)
        {
            var vehicles = db.VehiclesDetails
                .Include(o => o.VehicleModel)
                .ThenInclude(o => o.VehicleBrand)
                .ThenInclude(o => o.VehicleType)
                .Include(o => o.VehicleModel)
                .ThenInclude(o => o.VehicleBody)
                .FirstOrDefault(vd => vd.Id == id);

            return vehicles;
        }

        public async Task<int> GetOrderIdByVehicleId(int id)
        {
            var vehicle = await db.VehiclesDetails.FirstOrDefaultAsync(vd => vd.Id == id);

            return vehicle.OrderId;
        }

        public async Task ArchiveOrder(int id, string localDate)
        {
            var order = await db.Orders
                .Include(o => o.CurrentStatus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (order == null) return;

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);
            
            var currentStatusId = order.CurrentStatusId;

            if (order.CurrentStatus.StatusName == OrderConstants.OrderStatusDeliveredBilled
                || order.CurrentStatus.StatusName == OrderConstants.OrderStatusDeliveredPaid)
            {
                order.CurrentStatusId = GetCurrentStatusIdByName(OrderConstants.OrderStatusArchived);
            }
            else
            {
                order.CurrentStatusId = GetCurrentStatusIdByName(OrderConstants.OrderStatusArchived);
            }

            if(currentStatusId != order.CurrentStatusId)
            {
                await db.HistoriesOrdersActions.AddAsync(new HistoryOrderAction
                {
                    OrderId = order.Id,
                    ActionType = ActionType.Update,
                    DateTimeAction = dateTimeUpdate,
                    FieldAction = "Current Status",
                    ContentFrom = GetCurrentStatusById(currentStatusId).StatusName,
                    ContentTo = GetCurrentStatusById(order.CurrentStatusId).StatusName,
                });
            }

            order.DateTimeLastUpdate = dateTimeUpdate;

            await db.SaveChangesAsync();
        }
        
        public async Task<Order> AddNewOrder(string urlPage, Dispatcher dispatcher)
        {
            var transportationDispatch = GetTransportationDispatch(dispatcher.DispatcherType.Name);
            
            var order = await transportationDispatch.GetShipping(urlPage, dispatcher);
            
            if (order != null)
            {
                await AddOrder(order);
            }
            
            return order;
        }
        
        public async Task SaveVechicle(int idVech, string vin,
            string year, string make, string model, string type,
            string body, string color, string lotNumber, string localDate)
        {
            var vehicleType = await GetVehicleTypeByNameOrCreate(type);
            var vehicleBrand = await GetVehicleBrandByNameOrCreate(make, vehicleType.Id);
            var vehicleBody = await GetVehicleBodyByNameOrCreate(body);
            var vehicleModel = await GetVehicleModelByNameOrCreate(model, vehicleBrand.Id, vehicleBody.Id, Convert.ToInt32(year));

            var vehiclwInformation = new VehicleDetails
            {
                VIN = vin,
                VehicleModelId = vehicleModel.Id,
                VehicleModel = vehicleModel,
                Color = color,
                Lot = lotNumber
            };


            await SaveVechInDb(idVech, vehiclwInformation, localDate);
        }

        public async Task<VehicleBody> GetVehicleBodyByNameOrCreate(string vehicleBody)
        {
            if (string.IsNullOrEmpty(vehicleBody)) return null;

            var vehicleBodydDb = await db.VehiclesBodies.FirstOrDefaultAsync(vb => vb.Name == vehicleBody);

            if (vehicleBodydDb == null) return await CreateVehicleBodyByName(vehicleBody);

            return vehicleBodydDb;
        }

        public async Task<VehicleBody> CreateVehicleBodyByName(string name)
        {
            var vehicleBody = new VehicleBody
            {
                Name = name,
            };

            await db.VehiclesBodies.AddAsync(vehicleBody);

            await db.SaveChangesAsync();

            return vehicleBody;
        }
        
        public async Task<VehicleModel> GetVehicleModelByNameOrCreate(string vehicleModel, int vehicleBrandId, int vehicleBodyId, int year)
        {
            if (string.IsNullOrEmpty(vehicleModel)) return null;

            var vehicleModeldDb = await db.VehiclesModels.FirstOrDefaultAsync(vb => vb.Name == vehicleModel && vb.VehicleBrandId == vehicleBrandId && vb.VehicleBodyId == vehicleBodyId && vb.Year == year);

            if (vehicleModeldDb == null) return await CreateVehicleModelByName(vehicleModel, vehicleBrandId, vehicleBodyId, year);

            return vehicleModeldDb;
        }

        public async Task<VehicleModel> CreateVehicleModelByName(string name, int vehicleBrandId, int vehicleBodyId, int year)
        {
            var vehicleModel = new VehicleModel
            {
                Name = name,
                VehicleBodyId = vehicleBodyId,
                VehicleBrandId = vehicleBrandId,
                Year = year,
            };

            await db.VehiclesModels.AddAsync(vehicleModel);

            await db.SaveChangesAsync();

            return vehicleModel;
        }

        public async Task<VehicleBrand> GetVehicleBrandByNameOrCreate(string vehicleBrand, int vehicleTypeId)
        {
            if (string.IsNullOrEmpty(vehicleBrand)) return null;

            var vehicleBrandDb = await db.VehiclesBrands.FirstOrDefaultAsync(vb => vb.Name == vehicleBrand && vb.VehicleTypeId == vehicleTypeId);

            if (vehicleBrandDb == null) return await CreateVehicleBrandByName(vehicleBrand, vehicleTypeId);

            return vehicleBrandDb;
        }

        public async Task<VehicleBrand> CreateVehicleBrandByName(string name, int vehicleTypeId)
        {
            var vehicleBrand = new VehicleBrand
            {
                Name = name,
                VehicleTypeId = vehicleTypeId
            };

            await db.VehiclesBrands.AddAsync(vehicleBrand);

            await db.SaveChangesAsync();

            return vehicleBrand;
        }

        public async Task<VehicleType> GetVehicleTypeByNameOrCreate(string vehicleType)
        {
            if (string.IsNullOrEmpty(vehicleType)) return null;

            var vehicleTypeDb = await db.VehiclesTypes.FirstOrDefaultAsync(vt => vt.Name == vehicleType);

            if (vehicleTypeDb == null) return await CreateVehicleTypeByName(vehicleType);

            return vehicleTypeDb;
        }

        public async Task<VehicleType> CreateVehicleTypeByName(string name)
        {
            var vehicleType = new VehicleType
            {
                Name = name
            };

            await db.VehiclesTypes.AddAsync(vehicleType);

            await db.SaveChangesAsync();

            return vehicleType;
        }
        
        public async Task AddHistory(string key, string idCompany, string idOrder, string idVech, string idDriver, string action, string localDate)
        {
            /*var historyOrder = new HistoryOrder();
            var idUser = await GetUserIdByKey(key);
            
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
            historyOrder.DateAction = string.IsNullOrEmpty(localDate) ? DateTime.UtcNow.ToString(DateTimeFormats.FullDateTimeInfo) : localDate;
            
            await AddHistoryInDb(historyOrder);*/
        }
        
        public string GetStrAction(string key, string idCompany, string idOrder, string idVech, string idDriver, string action)
        {
            var strAction = string.Empty;
            try
            {
                switch (action)
                {
                    //int idUser = _sqlEntityFramworke.GetUserIdByKey(key);
                    case OrderConstants.ActionAssign:
                        {
                            var fullNameDriver = GetFullNameDriverById(idDriver);
                            strAction = $"Assign the driver ordered {fullNameDriver}";
                            break;
                        }
                    case OrderConstants.ActionUnAssign:
                        {
                            var fullNameDriver = GetFullNameDriverById(idDriver);
                            strAction = $"Withdrew an order from {fullNameDriver} driver";
                            break;
                        }
                    case OrderConstants.ActionSolved:
                        {
                            /*var fullNameUser = GetFullNameUserByKey(key);
                            strAction = $"{fullNameUser} clicked on the \"Solved\" button";*/
                            break;
                        }
                    case OrderConstants.ActionArchivedOrder:
                        {
                            strAction = $"Transferred the order to the archive";
                            break;
                        }
                    case OrderConstants.ActionDeletedOrder:
                        {
                            strAction = $"Transferred the order to deleted orders";
                            break;
                        }
                    case OrderConstants.ActionCreate:
                        {
                            strAction = $"Created an order";
                            break;
                        }
                    case OrderConstants.ActionSaveOrder:
                        {
                            strAction = $"Edited the order";
                            break;
                        }
                    case OrderConstants.ActionSaveVech:
                        {
                            //var vehiclwInformation = GetVechById(idVech);
                            /*var vehiclwInformation = GetVehicleHistoryById(idVech);
                            strAction = $"Edited the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Model}";
                            */break;
                        }
                    case OrderConstants.ActionRemoveVech:
                        {
                            //var vehiclwInformation = GetVechById(idVech);
                            /*var vehiclwInformation = GetVehicleHistoryById(idVech);
                            strAction = $"Removed the vehicle {vehiclwInformation.Year} y. {vehiclwInformation.Make} {vehiclwInformation.Model}";
                            */break;
                        }
                    case OrderConstants.ActionAddVech:
                        {
                            strAction = $"Created a vehicle";
                            break;
                        }
                }
            }
            catch (Exception ex)
            {

            }
                  
            return strAction;
        }
        
        public async Task RemoveVechicle(int vechicleId)
        {
            var vehicle = await db.VehiclesDetails.FirstOrDefaultAsync(v => v.Id == vechicleId);

            if (vehicle == null) return;

            //var carForHistory = mapper.Map<VehicleHistory>(vehicle);

            /*await db.VehicleHistories.AddAsync(carForHistory);*/

            db.VehiclesDetails.Remove(vehicle);
            
            await db.SaveChangesAsync();
        }
        
        public async Task<List<string>> GetVehicleModels(string searchName, string vehicleBrand)
        {
            var vehiclesModels = db.VehiclesModels
                .Include(vb => vb.VehicleBrand)
                .AsQueryable();

            if(!string.IsNullOrEmpty(searchName))
                vehiclesModels = vehiclesModels.Where(vb => vb.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(vehicleBrand))
                vehiclesModels = vehiclesModels.Where(vb => vb.VehicleBrand.Name.Contains(vehicleBrand));

            var vehiclesModelsList = await vehiclesModels
                .Select(brand => brand.Name)
                .Distinct()
                .ToListAsync();

            return vehiclesModelsList;
        }
        
        public async Task<List<string>> GetVehicleBrands(string searchName, string vehicleType)
        {
            var vehiclesBrands = db.VehiclesBrands
                .Include(vb => vb.VehicleType)
                .AsQueryable();

            if(!string.IsNullOrEmpty(searchName))
                vehiclesBrands = vehiclesBrands.Where(vb => vb.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(vehicleType))
                vehiclesBrands = vehiclesBrands.Where(vb => vb.VehicleType.Name.Contains(vehicleType));

            var vehiclesBrandsList = await vehiclesBrands.Select(brand => brand.Name).Distinct().ToListAsync();

            return vehiclesBrandsList;
        }

        public async Task<List<string>> GetVehicleTypes(string searchName)
        {
            var vehiclesTypes = await db.VehiclesTypes
                .Where(vb => vb.Name.Contains(searchName))
                .Select(brand => brand.Name)
                .Distinct()
                .ToListAsync();

            return vehiclesTypes;
        }

        public async Task<VehicleDetails> AddVechicle(int id, string localDate)
        {
            var order = await db.Orders
                .FirstOrDefaultAsync(s => s.Id == id);

            var vehicle = new VehicleDetails
            {
                OrderId = order.Id,
            };

            var dateTimeLastUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            order.DateTimeLastUpdate = dateTimeLastUpdate;
            await db.VehiclesDetails.AddAsync(vehicle);

            await db.SaveChangesAsync();

            var historyOrder = new HistoryOrderAction
            {
                OrderId = order.Id,
                VehicleId = vehicle.Id,
                ActionType = ActionType.Create,
                DateTimeAction = dateTimeLastUpdate,
            };

            await db.HistoriesOrdersActions.AddAsync(historyOrder);
            
            await db.SaveChangesAsync();
            
            return vehicle;
        }
        
        public async Task<Order> CreateOrder(string companyId, string localDate)
        {
            if (!int.TryParse(companyId, out var result)) return null;

            var dateTimeCreate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var pickedUpInfo = new AddressInformation();
            var deliveryInfo = new AddressInformation();

            await db.AddressInformations.AddAsync(pickedUpInfo);
            await db.AddressInformations.AddAsync(deliveryInfo);

            await db.SaveChangesAsync();

            var order = new Order
            {
                CompanyId = result,
                DateTimeCreate = dateTimeCreate,
                CurrentStatusId = 1,
                PickedUpId = pickedUpInfo.Id,
                DeliveryId = deliveryInfo.Id,
                DateTimeDelivery = DateTime.Now,
                DateTimePickedUp = DateTime.Now,
            };

            db.Orders.Add(order);

            var phoneNumberContact = new PhoneNumber();
            var phoneNumberPickedUp = new PhoneNumber();
            var phoneNumberDelivery = new PhoneNumber();
            var phoneNumberFax = new PhoneNumber();

            await db.PhonesNumbers.AddAsync(phoneNumberContact);
            await db.PhonesNumbers.AddAsync(phoneNumberPickedUp);
            await db.PhonesNumbers.AddAsync(phoneNumberDelivery);
            await db.PhonesNumbers.AddAsync(phoneNumberFax);

            await db.SaveChangesAsync();

            pickedUpInfo.PhoneNumberId = phoneNumberPickedUp.Id;
            deliveryInfo.PhoneNumberId = phoneNumberDelivery.Id;
            order.PhoneNumberId = phoneNumberContact.Id;
            order.FaxNumberId = phoneNumberFax.Id;

            var orderEntry = db.Orders.FirstOrDefault(s => s.OrderId == order.OrderId);

            orderEntry.OrderId = order.Id.ToString();

            var orderHistoryCreate = new HistoryOrderAction
            {
                OrderId = order.Id,
                ActionType = ActionType.Create,
                DateTimeAction = dateTimeCreate,
            };

            await db.HistoriesOrdersActions.AddAsync(orderHistoryCreate);

            await db.SaveChangesAsync();

            return orderEntry;
        }
        
        public Order GetOrderWithVehiclwIn(int id)
        {
            return GetOrderWithVehiclwInDb(id);
        }
        
        public async Task Assign(int orderId, string driverId)
        {
            var isDriverAssign = CheckDriverOnShipping(orderId);
            string tokenShope = null;
            
            if (isDriverAssign)
            {
                tokenShope = await GerShopTokenForShipping(orderId);
            }
            
            var vehiclwInformations = await AddDriversInOrder(orderId, driverId);
            
            
            var managerNotify = new ManagerNotifyWeb();
            var tokenShope1 = await GerShopTokenForShipping(orderId);
                
            if (!isDriverAssign)
            {
                managerNotify.SendNotyfyAssign(orderId, tokenShope1, vehiclwInformations);
            }
            else
            {
               managerNotify.SendNotyfyAssign(orderId, tokenShope1, vehiclwInformations);
               managerNotify.SendNotyfyUnassign(orderId, tokenShope, vehiclwInformations);
            }
        }
        
        public async Task Unassign(int orderId)
        {
            var managerNotify = new ManagerNotifyWeb();
            var tokenShope = await GerShopTokenForShipping(orderId);
            var vehiclwInformations = await RemoveDriversInOrder(orderId);
            
            await Task.Run(() =>
            {
                managerNotify.SendNotyfyUnassign(orderId, tokenShope, vehiclwInformations);
            });
        }
        
        public void Solved(string orderId)
        {
            var shipping = db.Orders.FirstOrDefault(s => s.OrderId == orderId);
            if (shipping == null) return;
            
            //shipping.IsProblem = false;
            
            db.SaveChanges();
        }
        
        public async Task<int> GetCountPage(string companyId, string status, string loadId, string name, string address, string phone,
            string email, decimal price)
        {
            if(int.TryParse(companyId, out int result))
            {
                return await GetCountPageInDb(result, status, loadId, name, address, phone, email, price);
            }

            return 0;
        }

        public int GetCountPage(int countPage)
        {
            var remainderPage = countPage % 20;
                    
            return  remainderPage > 0 ? countPage + 1 : countPage;
        }
        
        public async Task<List<Order>> GetOrders(string companyId, string status, int page, string loadId ,string name, string address,
            string phone, string email, decimal price)
        {
            if(int.TryParse(companyId, out int result))
            {
                return await GetOrdersInDb(result, status, page, loadId, name, address, phone, email, price);
            }

            return new List<Order>();
        }
        
        public async Task<Order> GetCompanyOrderById(string companyId, int id)
        {
            if (int.TryParse(companyId, out int result))
            {

                return await GetOrderInDb(result, id);
            }

            return null;
        }
        
        public async Task<EditOrderViewModel> GetEditCompanyOrderById(string companyId, int id)
        {
            if (int.TryParse(companyId, out int result))
            {
                var orders = await GetOrderInDb(result, id);

                return mapper.Map<EditOrderViewModel>(orders);
            }

            return null;
        }
        
        public async Task<EditOrderViewModel> UpdateOrder(EditOrderViewModel model, string localDate)
        {
           return await UpdateOrderInDb(model, localDate);
        }
        
        public void SavePath(string id, string path)
        {
            SavePathDb(id, path);
        }
        
        public async Task<string> GetDocument(string id)
        {
            return await GetDocumentDb(id);
        }

        public async Task<Order> GetOrder(string companyId, int id)
        {
            if (!int.TryParse(companyId, out var result)) return null;

            return await GetOrderInDb(result, id);
        }
        
        public async Task<OrderWithHistoryViewModel> GetOrderWithHistory(string companyId, int id)
        {
            if (!int.TryParse(companyId, out var result)) return null;

            var orderInfo = await GetOrderInDb(result, id);

            var historyOrderActions = db.HistoriesOrdersActions
                .Where(hoa => hoa.OrderId == id)
                .OrderByDescending(hoa => hoa.DateTimeAction)
                .AsEnumerable()
                .ToList();

            var groupedHistory = historyOrderActions
                .GroupBy(hoa => hoa.DateTimeAction.Date)
                .Select(group => new HistoryOrderGroup
                {
                    GroupAction = group.Key,
                    Groups = group.Select(hoa =>
                    {
                        var historyOrderActionViewModel = new HistoryOrderActionWithVehicleViewModel
                        {
                            Id = hoa.Id,
                            OrderId = hoa.OrderId,
                            VehicleId = hoa.VehicleId,
                            ActionType = hoa.ActionType,
                            FieldAction = hoa.FieldAction,
                            ContentFrom = hoa.ContentFrom,
                            ContentTo = hoa.ContentTo,
                            DateTimeAction = hoa.DateTimeAction
                        };

                        if (hoa.VehicleId.HasValue)
                        {
                            historyOrderActionViewModel.VehicleDetails = GetVehicleDetailsById(hoa.VehicleId.Value);
                        }

                        return historyOrderActionViewModel;
                    }).ToList()
                })
                .ToList();
            

            return new OrderWithHistoryViewModel
            {
                Order = orderInfo,
                Actions = groupedHistory
            };
        }

        public string CreateFiltersString(string loadId, string name, string address, string phone, string email,
                     string price)
                 {
            var sb = new StringBuilder("?");
        
            if (!string.IsNullOrEmpty(loadId))
            {
                sb.Append($"loadId={loadId}&");
            }

            if (!string.IsNullOrEmpty(name))
            {
                sb.Append($"name={name}&");
            }

            if (!string.IsNullOrEmpty(address))
            {
                sb.Append($"address={address}&");
            }

            if (!string.IsNullOrEmpty(phone))
            {
                sb.Append($"phone={phone}&");
            }

            if (!string.IsNullOrEmpty(email))
            {
                sb.Append($"email={email}&");
            }

            if (!string.IsNullOrEmpty(price))
            {
                sb.Append($"price={price}");
            }

            if (sb.Length == 1 || sb[sb.Length - 1] == '&') sb.Length -= 1;

            return sb.ToString();
        }
        
        public bool SendRemindInspection(int driverId)
        {
            var managerNotifyWeb = new ManagerNotifyWeb();
            var isInspactionDriverToDay = CheckInspactionDriverToDay(driverId);
            
            if (!isInspactionDriverToDay)
            {
                var tokenShiping = GerShopToken(driverId);

                managerNotifyWeb.SendSendNotyfyRemindInspection(tokenShiping);
            }
            
            return isInspactionDriverToDay;
        }
        
        public CurrentStatus GetCurrentStatusByName(string name)
        {
            var currentStatus = db.CurrentStatuses.FirstOrDefault(cs => cs.StatusName == name);

            return currentStatus;
        }
        
        public CurrentStatus GetCurrentStatusById(int id)
        {
            var currentStatus = db.CurrentStatuses.FirstOrDefault(cs => cs.Id == id);

            return currentStatus;
        }
        
        public int GetCurrentStatusIdByName(string name)
        {
            var currentStatusId = db.CurrentStatuses.FirstOrDefault(cs => cs.StatusName == name).Id;

            return currentStatusId;
        }

        public List<HistoryOrderAction> GetHistoryOrder(int orderId)
        {
            return GetHistoryOrderByIdOrder(orderId);
        }
        
        private List<HistoryOrderAction> GetHistoryOrderByIdOrder(int orderId)
        {
            return db.HistoriesOrdersActions.Where(ho => ho.OrderId == orderId).ToList();
        }
        
        private bool CheckInspactionDriverToDay(int driverId)
        {
            var driver = db.Drivers
                .Include(d => d.Inspections)
                .FirstOrDefault(d => d.Id == driverId);

            var inspectionDriver = driver.Inspections != null && driver.Inspections.Count != 0 
                ? driver.Inspections.Last() : null;

            if (inspectionDriver == null)
            {
                return false;
            }

            var inspectionDate = inspectionDriver.DateTimeInspection;
            
            if (inspectionDate != DateTime.Now.Date)
            {
                if (DateTime.Now.Hour >= 12)
                {
                    //driver.IsInspectionDriver = false;
                    //driver.IsInspectionToDayDriver = false;
                }
                else if (DateTime.Now.Hour <= 12 && 6 >= DateTime.Now.Hour)
                {
                    //driver.IsInspectionDriver = true;
                    //driver.IsInspectionToDayDriver = false;
                }
                db.SaveChanges();
            }

            return false;
        }
        
        private string GerShopToken(int driverId)
        {
            var driver = db.Drivers
                .Include(d => d.DriverControl)
                .FirstOrDefault(d => d.Id == driverId);
            
            return driver?.DriverControl.TokenShope;
        }
        
        private async Task<string> GetDocumentDb(string id)
        {
            string pathDoc = "";

            var driver = await db.Drivers
                .Include(d => d.Inspections)
                .FirstOrDefaultAsync(d => d.Id.ToString() == id);

            if (driver.Inspections != null)
            {
                var inspectionDriver = driver.Inspections.Last();
                var truck = db.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.TruckId);
                if (truck != null)
                {
                    pathDoc = truck.Documents?.First().DocPath;
                }
            }
            return string.Empty;
        }
        
        private void SavePathDb(string id, string path)
        {
            var truck = db.Trucks.First(t => t.Id.ToString() == id);
            //truck.path = path;
            db.SaveChanges();
        }

        private async Task<Order> GetOrderInDb(int companyId, int id)
        {
            var order = await db.Orders
                .Include(o => o.PhoneNumber)
                .Include(o => o.Vehicles)
                .ThenInclude(o => o.VehicleModel)
                .ThenInclude(o => o.VehicleBrand)
                .Include(o => o.Vehicles)
                .ThenInclude(o => o.VehicleModel)
                .ThenInclude(o => o.VehicleBody)
                .Include(o => o.PickedUp)
                .ThenInclude(o => o.PhoneNumber)
                .Include(o => o.Delivery)
                .ThenInclude(o => o.PhoneNumber)
                .Include(o => o.FaxNumber)
                .Include(o => o.CurrentStatus)
                .FirstOrDefaultAsync(o => o.Id == id && o.CompanyId == companyId);

            return order;
        }

        private async Task<List<Order>> GetOrdersInDb(int companyId, string status, int page, string loadId, string name, string address, string phone, string email, decimal price)
        {
            using (var context = new Context())
            {
                var qOrders = context.Orders
                    .Include(o => o.PhoneNumber)
                    .Include(o => o.Delivery)
                    .ThenInclude(o => o.PhoneNumber)
                    .Include(o => o.PickedUp)
                    .ThenInclude(o => o.PhoneNumber)
                    .Include(o => o.CurrentStatus)
                    .Include(o => o.Driver)
                    .Where(o => o.CurrentStatus.StatusName == status && o.CompanyId == companyId)
                    .OrderByDescending(o => o.DateTimeLastUpdate)
                    .AsQueryable();

                if (loadId != null)
                {
                    qOrders = qOrders.Where(x =>
                        x.OrderId == loadId);
                }

                if (name != null)
                {
                    qOrders = qOrders.Where(x =>
                        x.Contact.Contains(name) || x.PickedUp.Name.Contains(name) || x.PickedUp.ContactName.Contains(name) ||
                        x.Delivery.Name.Contains(name) || x.Delivery.ContactName.Contains(name));
                }

                if (address != null)
                {
                    qOrders = qOrders.Where(x =>
                        x.PickedUp.Address.Contains(address) || x.Delivery.Address.Contains(address) || x.PickedUp.City.Contains(address) ||
                        x.Delivery.City.Contains(address));
                }

                if (phone != null)
                {
                    /* qOrders = qOrders.Where(x =>
                         x.PickedUp.PhoneNumber.Contains(phone) || x.Delivery.PhoneNumber.Contains(phone) || x.PhoneNumber.Contains(phone));*/
                }

                if (email != null)
                {
                    qOrders = qOrders.Where(x => x.PickedUp.Email.Contains(email) || x.Delivery.Email.Contains(email));
                }

                if (price != 0)
                {
                    qOrders = qOrders.Where(x => x.Price == price);
                }

                if (page == UserConstants.AllPagesNumber) return await qOrders.ToListAsync();

                try
                {
                    qOrders = qOrders.Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount);

                    qOrders = qOrders.Take(UserConstants.NormalPageCount);
                }
                catch (Exception)
                {
                    qOrders = qOrders.Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount);
                }

                var listOrders = await qOrders.ToListAsync();

                return listOrders;
            }
        }
        
        private async Task<int> GetCountPageInDb(int companyId, string status, string loadId, string name, string address, string phone, string email, decimal price)
        {
            var qOrders = db.Orders.Where(x => x.CurrentStatus.StatusName == status && x.CompanyId == companyId).AsQueryable();
        
            if (loadId != null)
            {
                qOrders = qOrders.Where(x =>
                    x.OrderId.Contains(loadId));
            }

            if (name != null)
            {
                qOrders = qOrders.Where(x =>
                    x.Contact.Contains(name) || x.PickedUp.Name.Contains(name) || x.PickedUp.ContactName.Contains(name) ||
                    x.Delivery.Name.Contains(name) || x.Delivery.ContactName.Contains(name));
            }

            if (address != null)
            {
                qOrders = qOrders.Where(x =>
                    x.PickedUp.Address.Contains(address) || x.Delivery.Address.Contains(address) || x.PickedUp.City.Contains(address) ||
                    x.Delivery.City.Contains(address));
            }

            if (phone != null)
            {
                qOrders = qOrders.Where(x =>
                    x.PickedUp.PhoneNumber.Number.Contains(phone) || x.Delivery.PhoneNumber.Number.Contains(phone) || x.PhoneNumber.Number.Contains(phone));
            }

            if (email != null)
            {
                qOrders = qOrders.Where(x => x.PickedUp.Email.Contains(email) || x.Delivery.Email.Contains(email));
            }

            if (price != 0)
            {
                qOrders = qOrders.Where(x => x.Price == price);
            }


            var countOrders = await qOrders.CountAsync();

            var countPages = GetCountPage(countOrders, UserConstants.NormalPageCount);
            
            return countPages;
        }

        private int GetCountPage(int countElements, int countElementsInOnePage)
        {
            var countPages = (countElements / countElementsInOnePage) % countElementsInOnePage;

            return countPages > 0 ? countPages + 1 : countPages;
        }

        private async Task<List<VehicleDetails>> RemoveDriversInOrder(int orderId)
        {
            var order = db.Orders
                .Include(s => s.Vehicles)
                .FirstOrDefault(s => s.Id == orderId);

            order.DriverId = null;

            order.CurrentStatusId = GetCurrentStatusIdByName(OrderConstants.OrderStatusNewLoad);
            
            await db.SaveChangesAsync();
            
            return order.Vehicles.ToList();
        }
        
        private async Task<string> GerShopTokenForShipping(int orderId)
        {
            var order = await db.Orders
                .Include(o => o.Driver)
                .ThenInclude(d => d.DriverControl)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            
            return order.Driver?.DriverControl.TokenShope;
        }
        
        private async Task<List<VehicleDetails>> AddDriversInOrder(int orderId, string driverId)
        {
            var order = db.Orders
                .Include(o => o.Vehicles)
                .FirstOrDefault(s => s.Id == orderId);

            var driver = db.Drivers.FirstOrDefault(d => d.Id == Convert.ToInt32(driverId));

            order.DriverId = driver.Id;

            order.CurrentStatusId = GetCurrentStatusIdByName(OrderConstants.OrderStatusAssigned);
            
            await db.SaveChangesAsync();
            
            return order.Vehicles.ToList();
        }
        
        private bool CheckDriverOnShipping(int orderId)
        {
            var order = db.Orders
                .FirstOrDefault(s => s.Id == orderId);

            return order.DriverId != null;
        }
        
        private Order GetOrderWithVehiclwInDb(int id)
        {
            var order = db.Orders
                .Include(o => o.Vehicles)
                .FirstOrDefault(o => o.Vehicles.FirstOrDefault(v => v.Id == id) != null);
            
            return order;
        }
        
        private VehicleDetails GetVechById(string idVech)
        {
            return db.VehiclesDetails.FirstOrDefault(v => v.Id.ToString() == idVech);
        }
        
        /*private VehicleHistory GetVehicleHistoryById(string idVech)
        {
            return db.VehicleHistories.FirstOrDefault(v => v.VehicleId.ToString() == idVech);
        }*/
        
        private string GetFullNameDriverById(string driverId)
        {
            var driver = db.Drivers.First(d => d.Id.ToString() == driverId);
            return $"{driver.FirstName} {driver.LastName}";
        }
        
        private int GetIdOrderByIdVech(string vechId)
        {
            var order = db.Orders
                .Include(s => s.Vehicles)
                .FirstOrDefault(s => s.Vehicles.FirstOrDefault(v => v.Id.ToString() == vechId) != null);
            
            return order.Id;
        }
        
        private async Task AddHistoryInDb(HistoryOrderAction historyOrder)
        {
            await db.HistoriesOrdersActions.AddAsync(historyOrder);

            await db.SaveChangesAsync();
        }
        
        private string GetDriverIdByIdOrder(string idOrder)
        {
            var shipping = db.Orders
                .First(s => s.Id.ToString() == idOrder);
            
            return shipping.DriverId.ToString();
        }
        
        private async Task SaveVechInDb(int vechId, VehicleDetails vehicleInformation, string localDate)
        {
            var vehicleInformationDb = await db.VehiclesDetails
                .Include(vd => vd.VehicleModel)
                .ThenInclude(vm => vm.VehicleBody)
                .Include(vd => vd.VehicleModel)
                .ThenInclude(vm => vm.VehicleBrand)
                .ThenInclude(vb => vb.VehicleType)
                .FirstOrDefaultAsync(vd => vd.Id == vechId);

            if (vehicleInformationDb == null) return;

            var historyList = new List<HistoryOrderAction>();

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            vehicleInformationDb.VIN = vehicleInformationDb.VIN == null ? string.Empty : vehicleInformationDb.VIN;

            if (!Equals(vehicleInformationDb.VIN, vehicleInformation.VIN) && !(string.IsNullOrEmpty(vehicleInformationDb.VIN) && vehicleInformation.VIN == null))
            {
                historyList.Add(new HistoryOrderAction
                {
                    OrderId = vehicleInformationDb.OrderId,
                    VehicleId = vehicleInformationDb.Id,
                    FieldAction = nameof(vehicleInformation.VIN),
                    ContentFrom = vehicleInformationDb.VIN ?? string.Empty,
                    ContentTo = vehicleInformation.VIN ?? string.Empty,
                    DateTimeAction = dateTimeUpdate,
                    ActionType = ActionType.Update,
                });

                vehicleInformationDb.VIN = vehicleInformation.VIN ?? string.Empty;
            }
            
            vehicleInformationDb.Color = vehicleInformationDb.Color == null ? string.Empty : vehicleInformationDb.Color;

            if (!Equals(vehicleInformationDb.Color, vehicleInformation.Color) && !(string.IsNullOrEmpty(vehicleInformationDb.Color) && vehicleInformation.Color == null))
            {
                historyList.Add(new HistoryOrderAction
                {
                    OrderId = vehicleInformationDb.OrderId,
                    VehicleId = vehicleInformationDb.Id,
                    FieldAction = nameof(vehicleInformation.Color),
                    ContentFrom = vehicleInformationDb.Color ?? string.Empty,
                    ContentTo = vehicleInformation.Color ?? string.Empty,
                    DateTimeAction = dateTimeUpdate,
                    ActionType = ActionType.Update,
                });

                vehicleInformationDb.Color = vehicleInformation.Color ?? string.Empty;
            }
            
            vehicleInformationDb.Lot = vehicleInformationDb.Lot == null ? string.Empty : vehicleInformationDb.Lot;

            if (!Equals(vehicleInformationDb.Lot, vehicleInformation.Lot) && !(string.IsNullOrEmpty(vehicleInformationDb.Lot) && vehicleInformation.Lot == null))
            {
                historyList.Add(new HistoryOrderAction
                {
                    OrderId = vehicleInformationDb.OrderId,
                    VehicleId = vehicleInformationDb.Id,
                    FieldAction = nameof(vehicleInformation.Lot),
                    ContentFrom = vehicleInformationDb.Lot ?? string.Empty,
                    ContentTo = vehicleInformation.Lot ?? string.Empty,
                    DateTimeAction = dateTimeUpdate,
                    ActionType = ActionType.Update,
                });

                vehicleInformationDb.Lot = vehicleInformation.Lot ?? string.Empty;
            }

            if(vehicleInformationDb.VehicleModelId != vehicleInformation.VehicleModelId)
            {
                AddHistoryToUpdateVehicleInformation(vehicleInformationDb?.VehicleModel?.Name ?? string.Empty,
                    vehicleInformation?.VehicleModel?.Name ?? string.Empty, "Vehicle Model", vehicleInformationDb.OrderId, vehicleInformationDb.Id, dateTimeUpdate);

                AddHistoryToUpdateVehicleInformation(vehicleInformationDb?.VehicleModel?.VehicleBody?.Name ?? string.Empty,
                    vehicleInformation?.VehicleModel?.VehicleBody?.Name ?? string.Empty, "Vehicle Body", vehicleInformationDb.OrderId, vehicleInformationDb.Id, dateTimeUpdate);
                
                AddHistoryToUpdateVehicleInformation(vehicleInformationDb?.VehicleModel?.VehicleBrand?.Name ?? string.Empty,
                    vehicleInformation?.VehicleModel?.VehicleBrand?.Name ?? string.Empty, "Vehicle Brand", vehicleInformationDb.OrderId, vehicleInformationDb.Id, dateTimeUpdate);
                
                AddHistoryToUpdateVehicleInformation(vehicleInformationDb?.VehicleModel?.VehicleBrand?.VehicleType?.Name ?? string.Empty,
                    vehicleInformation?.VehicleModel?.VehicleBrand?.VehicleType?.Name ?? string.Empty, "Vehicle Type", vehicleInformationDb.OrderId, vehicleInformationDb.Id, dateTimeUpdate);

                vehicleInformationDb.VehicleModelId = vehicleInformation.VehicleModelId;
            }


            
            await db.SaveChangesAsync();
        }
        
        private bool AddHistoryToUpdateVehicleInformation(string oldValue, string newValue, string fieldAction, int orderId, int vehicleId, DateTime dateTimeAction)
        {
            try
            {
                oldValue = oldValue == null ? string.Empty : oldValue;

                if (!Equals(oldValue, newValue) && !(string.IsNullOrEmpty(oldValue) && newValue == null))
                {
                    db.HistoriesOrdersActions.Add(new HistoryOrderAction
                    {
                        OrderId = orderId,
                        VehicleId = vehicleId,
                        FieldAction = fieldAction,
                        ContentFrom = oldValue ?? string.Empty,
                        ContentTo = newValue ?? string.Empty,
                        DateTimeAction = dateTimeAction,
                        ActionType = ActionType.Update,
                    });

                    db.SaveChanges();
                    return true;
                }
            }
            catch(Exception ex) {
                return false;
            }

            return false;
        }

        private async Task AddOrder(Order order)
        {
            var isCheckOrder = CheckUrlOrder(order);
            try
            {
                if (isCheckOrder) return;
                await db.Orders.AddAsync(order);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }
        }
        
        private bool CheckUrlOrder(Order order)
        {
            return db.Orders.FirstOrDefault(s => s.UrlRequest == order.UrlRequest) != null;
        }
        
        private bool CheckOrder(Order order)
        {
            return db.Orders.FirstOrDefault(s => s.Id == order.Id) != null;
        }
        
        private ITransportationDispatch GetTransportationDispatch(string typeDispatch)
        {
            ITransportationDispatch transportationDispatch = null;
            
            switch (typeDispatch)
            {
                case  OrderConstants.CentralDispatch: /*transportationDispatch = new GetDataCentralDispatch();*/ break;
            }
            
            return transportationDispatch;
        }

        private async Task<EditOrderViewModel> UpdateOrderInDb(EditOrderViewModel model, string localDate)
        {
            if (model == null) return null;

            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var orderEdit = await db.Orders
                .Include(o => o.PhoneNumber)
                .Include(o => o.FaxNumber)
                .Include(o => o.Delivery)
                .ThenInclude(o => o.PhoneNumber)
                .Include(o => o.PickedUp)
                .ThenInclude(o => o.PhoneNumber)
                .Include(o => o.CurrentStatus)
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if (orderEdit != null)
            {
                List<HistoryOrderAction> historyEntries = new List<HistoryOrderAction>();
                var orderEditViewModel = mapper.Map<EditOrderViewModel>(orderEdit);

                CompareAndSaveUpdatedFieldsInOrder(orderEditViewModel, model, historyEntries, orderEdit.Id, dateTimeUpdate);

                await db.SaveChangesAsync();

                foreach (var historyEntry in historyEntries)
                {
                    db.HistoriesOrdersActions.Add(historyEntry);
                }

                await db.SaveChangesAsync();

                orderEdit.OrderId = model.OrderId ?? orderEdit.OrderId;

                var currentStatusId = db.CurrentStatuses.First(cs => cs.StatusName == model.CurrentStatus.StatusName).Id;
                var orderEditStatusId = orderEdit.CurrentStatusId;

                switch (model.CurrentStatus.StatusName)
                {
                    case OrderConstants.OrderStatusNewLoad:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusAssigned:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusPickedUp:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusDelivered:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusDeliveredBilled:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusDeliveredPaid:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusDeleted:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusDeletedBilled:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusDeletedPaid:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusArchived:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusArchivedBilled:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    case OrderConstants.OrderStatusArchivedPaid:
                        orderEdit.CurrentStatusId = currentStatusId;
                        break;
                    default:
                        break;
                }

                if(orderEditStatusId != orderEdit.CurrentStatusId)
                {
                    db.HistoriesOrdersActions.Add(new HistoryOrderAction
                    {
                        OrderId = orderEdit.Id,
                        ActionType = ActionType.Update,
                        DateTimeAction = dateTimeUpdate,
                        FieldAction = "Current Status",
                        ContentFrom = GetCurrentStatusById(orderEditStatusId).StatusName,
                        ContentTo = GetCurrentStatusById(orderEdit.CurrentStatusId).StatusName
                    });
                }

                orderEdit.Instructions = model.Instructions ?? string.Empty;
                orderEdit.PickedUp.Name = model.PickedUp.Name ?? string.Empty;
                orderEdit.PickedUp.ContactName = model.PickedUp.ContactName ?? string.Empty;
                orderEdit.PickedUp.Address = model.PickedUp.Address ?? string.Empty;
                orderEdit.PickedUp.City = model.PickedUp.City ?? string.Empty;
                orderEdit.PickedUp.State = model.PickedUp.State ?? string.Empty;
                orderEdit.PickedUp.ZipCode = model.PickedUp.ZipCode ?? string.Empty;
                if (model.PickedUp.PhoneNumber.DialCode != 0) orderEdit.PickedUp.PhoneNumber.DialCode = model.PickedUp.PhoneNumber.DialCode;
                orderEdit.PickedUp.PhoneNumber.Name = model.PickedUp.PhoneNumber.Name ?? string.Empty;
                orderEdit.PickedUp.PhoneNumber.Number = model.PickedUp.PhoneNumber.Number ?? string.Empty;
                orderEdit.PickedUp.PhoneNumber.Iso2 = model.PickedUp.PhoneNumber.Iso2 ?? string.Empty;
                orderEdit.PickedUp.Email = string.IsNullOrEmpty(model.PickedUp.Email) ? string.Empty : model.PickedUp.Email.ToLower();
                orderEdit.DateTimePickedUp = model.DateTimePickedUp;

                orderEdit.DateTimeDelivery = model.DateTimeDelivery;
                orderEdit.Delivery.Name = model.Delivery.Name ?? string.Empty;
                orderEdit.Delivery.ContactName = model.Delivery.ContactName ?? string.Empty;
                orderEdit.Delivery.Address = model.Delivery.Address ?? string.Empty;
                orderEdit.Delivery.City = model.Delivery.City ?? string.Empty;
                orderEdit.Delivery.State = model.Delivery.State ?? orderEdit.Delivery.State;
                orderEdit.Delivery.ZipCode = model.Delivery.ZipCode ?? string.Empty;

                if (model.Delivery.PhoneNumber.DialCode != 0) orderEdit.Delivery.PhoneNumber.DialCode = model.Delivery.PhoneNumber.DialCode;
                orderEdit.Delivery.PhoneNumber.Name = model.Delivery.PhoneNumber.Name ?? string.Empty;
                orderEdit.Delivery.PhoneNumber.Number = model.Delivery.PhoneNumber.Number ?? string.Empty;
                orderEdit.Delivery.PhoneNumber.Iso2 = model.Delivery.PhoneNumber.Iso2 ?? string.Empty;

                orderEdit.Delivery.Email = string.IsNullOrEmpty(model.Delivery.Email) ? string.Empty : model.Delivery.Email.ToLower();

                orderEdit.PaymentMethod = model.PaymentMethod ?? string.Empty;
                orderEdit.Price = model.Price;
                orderEdit.BrokerFee = model.BrokerFee;
                orderEdit.Contact = model.Contact ?? string.Empty;
                if (model.PhoneNumber.DialCode != 0) orderEdit.PhoneNumber.DialCode = model.PhoneNumber.DialCode;
                orderEdit.PhoneNumber.Name = model.PhoneNumber.Name ?? string.Empty;
                orderEdit.PhoneNumber.Number = model.PhoneNumber.Number ?? string.Empty;
                orderEdit.PhoneNumber.Iso2 = model.PhoneNumber.Iso2 ?? string.Empty;
                
                if (model.FaxNumber.DialCode != 0) orderEdit.FaxNumber.DialCode = model.FaxNumber.DialCode;
                orderEdit.FaxNumber.Name = model.FaxNumber.Name ?? string.Empty;
                orderEdit.FaxNumber.Number = model.FaxNumber.Number ?? string.Empty;
                orderEdit.FaxNumber.Iso2 = model.FaxNumber.Iso2 ?? string.Empty;

                orderEdit.McNumber = model.McNumber ?? string.Empty;
                orderEdit.DateTimeLastUpdate = dateTimeUpdate;

                await db.SaveChangesAsync();
            }

            var updatedOrder = db.Orders.FirstOrDefault(x => x.Id == model.Id);

            return mapper.Map<EditOrderViewModel>(updatedOrder);
        }

        private void CompareAndSaveUpdatedFieldsInOrder(object original, object updated, List<HistoryOrderAction> historyEntries,int orderId, DateTime dateTimeUpdate, string prefix = "")
        {
            var originalType = original.GetType();

            var properties = originalType.GetProperties();

            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(HistoryAttribute)))
                {
                    object originalValue = property.GetValue(original);
                    object updatedValue = property.GetValue(updated);
                    originalValue ??= string.Empty;
                    if (!Equals(originalValue, updatedValue) && !(string.IsNullOrEmpty(originalValue.ToString()) && updatedValue==null))
                    {
                        string fieldName = prefix + property.Name;

                        historyEntries.Add(new HistoryOrderAction
                        {
                            OrderId = orderId,
                            FieldAction = fieldName,
                            ContentFrom = originalValue?.ToString() ?? string.Empty,
                            ContentTo = updatedValue?.ToString() ?? string.Empty,
                            DateTimeAction = dateTimeUpdate,
                            ActionType = ActionType.Update,
                        });

                        property.SetValue(original, updatedValue);
                    }
                }

                if (property.PropertyType.IsClass && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                    var originalNested = property.GetValue(original);
                    var updatedNested = property.GetValue(updated);

                    if (originalNested != null && updatedNested != null)
                    {
                        CompareAndSaveUpdatedFieldsInOrder(originalNested, updatedNested, historyEntries, orderId, dateTimeUpdate, property.Name + " ");
                    }
                }
            }
        }
    }
}