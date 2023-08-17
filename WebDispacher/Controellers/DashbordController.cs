using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Business.Services;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Models;
using WebDispacher.Service;
using WebDispacher.ViewModels;
using WebDispacher.ViewModels.Dashboard;
using WebDispacher.ViewModels.Order;
using WebDispacher.ViewModels.Vehicles;

namespace WebDispacher.Controellers
{
    public class DashbordController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public DashbordController(
            IOrderService orderService,
            IUserService userService,
            ICompanyService companyService,
            IDriverService driverService,
            IMapper mapper) : base(userService)
        {
            this.orderService = orderService;
            this.driverService = driverService;
            this.companyService = companyService;
            this.mapper = mapper;
        }
        private string Status { get; set; }

        [HttpPost]
        [Route("New")]
        public async Task<string> New(string linck, string key)
        {
            var urlPage = linck.Remove(0, linck.IndexOf("'") + 1);
            
            urlPage = urlPage.Remove(urlPage.IndexOf("'"));
            urlPage = $"https://www.centraldispatch.com{urlPage}";
            
            try
            {
                var dispatcher = companyService.CheckKeyDispatcher(key);
                
                if (dispatcher != null)
                {
                    //var shipping = await orderService.AddNewOrder(urlPage, dispatcher);
                }
            }
            catch (Exception exception)
            {
                // ignored
            }

            return null;
        }

        [HttpPost]
        [Route("/Dashboard/Company/SaveDocs")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveCompanyDoc(IFormFile certificateOfInsurance, IFormFile mcLetter)
        {
            if (certificateOfInsurance != null && mcLetter != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await companyService.UploadCompanyRequiredDoc(certificateOfInsurance, mcLetter, CompanyId);

                    return Redirect(Config.BaseReqvesteUrl);
                }
                catch (Exception e)
                {

                }
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/NewLoad")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> NewLoad(string loadId, string name, string address, string phone, string email, decimal price, int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewBag.CheckedCompany = await companyService.CheckCompanyRequiredDoc(CompanyId);

                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                if(isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                var orders = await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusNewLoad, page, loadId, name, address, phone, email, price);

                ViewBag.Drivers = await driverService.GetDriversByCompanyId(CompanyId);

                var countPages = await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusNewLoad, loadId, name, address, phone, email, price);

                ViewBag.count = countPages;// orderService.GetCountPage(countPage);
                    

                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("NewLoad", mapper.Map<List<ShortOrderViewModel>>(orders));
            }
            catch (Exception exception)
            {
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Dashbord/Assign")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<string> DriverSelect(int orderId, string driverId, string localDate)
        {
            var actionResult = false;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();

                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                if (!string.IsNullOrEmpty(driverId) && orderId != 0)
                {
                    await orderService.Assign(orderId, driverId);
                    //await orderService.AddHistory(key, "0", idOrder, "0",  idDriver, OrderConstants.ActionAssign, localDate);
                    actionResult = true;
                }
            }
            catch (Exception exception)
            {

            }
            
            return actionResult.ToString();
        }

        [HttpPost]
        [Route("Dashbord/Unassign")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<string> DriverUnSelect(int orderId, string localDate)
        {
            var actionResult = false;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                if (orderId != 0)
                {
                    //orderService.AddHistory(key, "0", idOrder, "0", "0", OrderConstants.ActionUnAssign, localDate);
                    await orderService.Unassign(orderId);
                    actionResult = true;
                }
            }
            catch (Exception exception)
            {
            }
            
            return actionResult.ToString();
        }

        [HttpGet]
        [Route("Dashbord/Order/Solved")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult Solved(string id,string localDate, string page)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                orderService.Solved(id);
                //Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", OrderConstants.ActionSolved, localDate));
                    
                return Redirect($"{page}");
            }
            catch (Exception exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Archived")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Archived(string loadId, string name, string address, string phone, string email, decimal price, int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                var shippings = await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusArchivedBilled, page, loadId, name, address, phone, email, price);
                
                if (shippings.Count < 20)
                {
                    shippings.AddRange(await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusArchivedPaid, page, loadId,  name, address, phone, email, price));
                }
                if (shippings.Count < 20)
                {
                    shippings.AddRange(await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusArchived, page, loadId, name, address, phone, email, price));
                }
                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                    
                ViewBag.Drivers = drivers;
                //ViewBag.Orders = GetShippingDTOs(shippings, drivers);

                var countPages = await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusArchived, loadId, name, address, phone, email, price);

                countPages += await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusArchivedBilled, loadId, name, address, phone, email, price);

                countPages += await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusArchivedPaid, loadId, name, address, phone, email, price);

                ViewBag.count = countPages;

                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("Archived", mapper.Map<List<ShortOrderViewModel>>(shippings));
            }
            catch (Exception exception)
            {
                
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Assigned")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Assigned(string loadId, string name, string address, string phone, string email, decimal price, int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                    
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                    
                var shippings =
                    await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusAssigned, page, loadId, name, address, phone, email, price);
                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                    
                // ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                ViewBag.Drivers = drivers;

                var countPages = await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusAssigned, loadId, name, address, phone, email, price);

                ViewBag.count = countPages; //orderService.GetCountPage(countPage);
                    
                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("Assigned", mapper.Map<List<ShortOrderViewModel>>(shippings));
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Billed")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Billed(string loadId, string name, string address, string phone, string email, decimal price,int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var shippings =
                    await orderService.GetOrders(CompanyId ,OrderConstants.OrderStatusDeliveredBilled, page, loadId, name, address, phone, email, price);
                    
                ViewBag.Drivers = await driverService.GetDriversByCompanyId(CompanyId);;
                //ViewBag.Orders = GetShippingDTOs(shippings, ViewBag.Drivers);
                    
                var countPage = await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDeliveredBilled, loadId, name, address, phone, email, price);

                ViewBag.count = orderService.GetCountPage(countPage);
                    
                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("Billed", mapper.Map<List<ShortOrderViewModel>>(shippings));
            }
            catch (Exception exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Deleted")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Deleted(string loadId, string name, string address, string phone, string email, decimal price, int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
        
                var shippings = await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusDeletedBilled, page, loadId, name, address, phone, email, price);
                    
                if (shippings.Count < 20)
                {
                    shippings.AddRange(await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusDeletedPaid, page, loadId, name, address, phone, email, price));
                }
                    
                if (shippings.Count < 20)
                {
                    shippings.AddRange(await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusDeleted, page, loadId, name, address, phone, email, price));
                }
                    
                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                    
                //ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    
                ViewBag.Drivers = drivers;
        
                var countPages = await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDeleted, loadId, name, address, phone, email, price);
        
                countPages += await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDeletedBilled, loadId, name, address, phone, email, price);
        
                countPages += await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDeletedPaid, loadId, name, address, phone, email, price);

                ViewBag.count = countPages;
                    
                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("Deleted", mapper.Map<List<ShortOrderViewModel>>(shippings));
            }
            catch (Exception)
            {
        
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Delivered")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Delivered(string loadId, string name, string address, string phone, string email, decimal price, int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var shippings = await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusDeliveredPaid, page, loadId, name, address, phone, email, price);
                    
                if (shippings.Count < 20)
                {
                    shippings.AddRange(await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusDeliveredBilled, page, loadId, name, address, phone, email, price));
                }

                if (shippings.Count < 20)
                {
                    shippings.AddRange(await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusDelivered, page, loadId, name, address, phone, email, price));
                }

                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                    
                //ViewBag.Orders = GetShippingDTOs(shippings, drivers);

                ViewBag.Drivers = drivers;

                var countPages = await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDeliveredBilled, loadId, name, address, phone, email, price);

                countPages += await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDeliveredPaid, loadId, name, address, phone, email, price);
                countPages += await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDelivered, loadId, name, address, phone, email, price);

                ViewBag.count = countPages;
                    
                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("Delivered", mapper.Map<List<ShortOrderViewModel>>(shippings));
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Paid")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Paid(string loadId, string name, string address, string phone, string email, decimal price, int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                var shippings =
                    await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusDeliveredPaid, page, loadId, name, address, phone, email, price);
                    
                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                //ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                ViewBag.Drivers = drivers;

                var countPages = await 
                    orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusDeliveredPaid, loadId, name, address, phone, email, price);

                ViewBag.count = countPages;

                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("Paid", mapper.Map<List<ShortOrderViewModel>>(shippings));
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Pickedup")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Pickedup(string loadId, string name, string address, string phone, string email, decimal price, int page = 1)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                    
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                var shippings = await orderService.GetOrders(CompanyId, OrderConstants.OrderStatusPickedUp, page, loadId, name, address, phone, email, price);
                    
                var drivers = await driverService.GetDriversByCompanyId(CompanyId);
                    
                //ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                ViewBag.Drivers = drivers;

                var countPages = await orderService.GetCountPage(CompanyId, OrderConstants.OrderStatusPickedUp, loadId, name, address, phone, email, price);

                ViewBag.count = countPages;

                ViewBag.LoadId = loadId;
                ViewBag.Name = name;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.Email = email;
                ViewBag.price = price == 0 ? string.Empty : price.ToString();

                ViewBag.SelectedPage = page;

                return View("Pickedup", mapper.Map<List<ShortOrderViewModel>>(shippings));
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/DeletedOrder")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> DeletedOrder(int id, string status, string localDate, string filters)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await orderService.DeleteOrder(id, localDate);
                   
                return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}?{filters}");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Dashbord/Order/DeletedOrder")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> DeletedOrder(int id, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await orderService.DeleteOrder(id, localDate);

                //await orderService.AddHistory(key, "0", id, "0", "0", OrderConstants.ActionDeletedOrder, localDate);

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }
        
        [Route("Dashbord/Order/ArchivedOrder")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> ArchivedOrder(int id, string filters, string localDate, string status = OrderConstants.OrderStatusNewLoad)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await orderService.ArchiveOrder(id, localDate);
                
                return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}?{filters}");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Dashbord/Order/ArchivedOrder")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> ArchivedOrder(int id,string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await orderService.ArchiveOrder(id, localDate);
                    
                //await Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", OrderConstants.ActionArchivedOrder, localDate));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Route("Dashbord/Order/FullInfoOrder")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> FullInfoOrder(int id, string status)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                    
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }


                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                if (id == 0) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                var order = await orderService.GetOrderWithHistory(CompanyId, id);

                return View("FullInfoOrder", order);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("Dashbord/Order/Edit")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditOrder(int id, string status)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                 
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                if (id==0) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                     
                var order = await orderService.GetEditCompanyOrderById(CompanyId, id);
                     
                Status = status;
                ViewBag.Status = status;

                return View(order);
            }
            catch (Exception)
            {   
         
            }
             
            return Redirect(Config.BaseReqvesteUrl);
        }
         
        [Route("Dashbord/Order/EditReload")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult EditReload(string id, string status, ShippingViewModel model)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);

                if (string.IsNullOrEmpty(id)) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                var searchOrder = new ShippingViewModel();// orderService.GetOrder(id);
                        
                var order = searchOrder ?? new ShippingViewModel();
                        
                order.NameP = model.NameP;
                order.ContactNameP = model.ContactNameP;
                order.CurrentStatus = model.CurrentStatus;
                order.AddresP = model.AddresP;
                order.CityP = model.CityP;
                order.StateP = model.StateP;
                order.IdOrder = model.IdOrder;
                order.ZipP = model.ZipP;
                order.PhoneP = model.PhoneP;
                order.EmailP = model.EmailP;
                order.PickupExactly = model.PickupExactly;
                order.Titl1DI = model.Titl1DI;
                order.NameD = model.NameD;
                order.ContactNameD = model.ContactNameD;
                order.AddresD = model.AddresD;
                order.CityD = model.CityD;
                order.StateD = model.StateD;
                order.ZipD = model.ZipD;
                order.PhoneD = model.PhoneD;
                order.EmailD = model.EmailD;
                order.TotalPaymentToCarrier = model.TotalPaymentToCarrier;
                order.PriceListed = model.PriceListed;
                order.BrokerFee = model.BrokerFee;
                order.ContactC = model.ContactC;
                order.FaxC = model.FaxC;
                order.PhoneC = model.PhoneC;
                order.IccmcC = model.IccmcC;
                        
                Status = status;
                ViewBag.Status = status;

                return View("EditOrder", order);

            }
            catch (Exception)
            {

            }
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Dashbord/Order/Create")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public async Task<IActionResult> CreatOrderpage(string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                var shipping = await orderService.CreateOrder(CompanyId, localDate);

                return RedirectToAction("EditOrder", new { id = shipping.Id, status = "NewLoad" });
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Dashbord/Order/Create")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<string> CreatOrder(string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                //ViewBag.NameCompany = GetCookieCompanyName();
                ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
                var shipping = await orderService.CreateOrder(CompanyId, localDate);

                //await orderService.AddHistory(key, "0", shipping.Id, "0", "0", OrderConstants.ActionCreate, localDate);

                return shipping.Id.ToString();
                //return shipping.Id;
            }
            catch (Exception)
            {

            }
            
            return null;
        }

        [HttpPost]
        [Route("Dashbord/Order/SavaOrder")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveOrder(EditOrderViewModel model, string dateTimeLocal)
        {
            ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(CompanyId);
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    var updatedOrder = await orderService.UpdateOrder(model, dateTimeLocal);

                    var navReturnPage = updatedOrder.CurrentStatus.StatusName.Replace(" ", "");

                    return Redirect(updatedOrder == null
                        ? $"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad"
                        : $"{Config.BaseReqvesteUrl}/Dashbord/Order/{navReturnPage}");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/SavaVech")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> SavaVech(int orderId, int vehicleId, string VIN, string Year,
            string Make, string Model, string Type, string body, string Color, string LotNumber, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if(!string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Model) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(body))
                    await orderService.SaveVechicle(vehicleId, VIN, Year, Make, Model, Type, body, Color, LotNumber, localDate);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Route("Dashbord/Order/RemoveVech")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<PartialViewResult> RemoveVech(int vehicleId, int orderId, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                await orderService.RemoveVechicle(vehicleId);

                var updatedVehiclesList = await orderService.GetVehicleDetailsByOrderId(orderId);

                return PartialView("~/Views/PartView/Vehicles/_VehiclesTable.cshtml", updatedVehiclesList);
            }
            catch (Exception)
            {
                return PartialView("~/Views/PartView/Vehicles/_VehiclesTable.cshtml", new List<VehicleDetails>());
            }
        }

        [Route("Dashbord/Order/GetVehicleModels")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<List<string>> GetVehicleModels(string searchName, string vehicleBrand)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var vehiclesModels = await orderService.GetVehicleModels(searchName, vehicleBrand);

                return vehiclesModels;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        
        [Route("Dashbord/Order/GetVehicleBrands")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<List<string>> GetVehicleBrands(string searchName, string vehicleType)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var vehicleBrands = await orderService.GetVehicleBrands(searchName, vehicleType);

                return vehicleBrands;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        
        [Route("Dashbord/Order/GetVehiclesTypes")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<List<string>> GetVehiclesTypes(string searchName)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var vehicleTypes = await orderService.GetVehicleTypes(searchName);

                return vehicleTypes;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        [HttpPost]
        [Route("Dashbord/Order/AddVech")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<PartialViewResult> AddVech(int orderId, string localDate)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                var vehiclwInformation = await orderService.AddVechicle(orderId, localDate);

                var updatedVehiclesList = await orderService.GetVehicleDetailsByOrderId(orderId);

                return PartialView("~/Views/PartView/Vehicles/_VehiclesTable.cshtml", updatedVehiclesList);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/PartView/Vehicles/_VehiclesTable.cshtml", new List<VehicleDetails>());
            }
        }

        [HttpGet]
        [Route("Dashbord/Image")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetShiping(string name, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(name);
            return File(imageFileStream, $"image/{type}");
        }

        private List<ShippingDTO> GetShippingDTOs(List<DaoModels.DAO.Models.Order> shippings, List<Driver> drivers)
        {
            /*return shippings.Select(z => new ShippingDTO()
            {
                Id = z.Id,
                AddresD = z.AddresD,
                AddresP = z.AddresP,
                Ask2 = z.Ask2,
                askForUserDelyveryM = z.askForUserDelyveryM,
                AskFromUser = z.AskFromUser,
                BrokerFee = z.BrokerFee,
                CDReference = z.CDReference,
                CityD = z.CityD,
                CityP = z.CityP,
                CompanyOwesCarrier = z.CompanyOwesCarrier,
                Condition = z.Condition,
                ContactC = z.ContactC,
                ContactNameD = z.ContactNameD,
                ContactNameP = z.ContactNameP,
                CurrentStatus = z.CurrentStatus,
                DamageForUsers = z.DamageForUsers,
                DataCancelOrder = z.DataCancelOrder,
                DataFullArcive = z.DataFullArcive,
                DataPaid = z.DataPaid,
                DeliveryEstimated = z.DeliveryEstimated,
                Description = z.Description,
                DispatchDate = z.DispatchDate,
                EmailD = z.EmailD,
                EmailP = z.EmailP,
                FaxC = z.FaxC,
                IccmcC = z.IccmcC,
                IdDriver = z.IdDriver,
                idOrder = z.idOrder,
                InternalLoadID = z.InternalLoadID,
                IsProblem = z.IsProblem,
                LastUpdated = z.LastUpdated,
                NameD = z.NameD,
                NameDriver = drivers.FirstOrDefault(d => d.Id == z.IdDriver) != null ? drivers.FirstOrDefault(d => d.Id == z.IdDriver).FullName : "",
                NameP = z.NameP,
                OnDeliveryToCarrier = z.OnDeliveryToCarrier,
                PhoneC = z.PhoneC,
                PhoneD = z.PhoneD,
                PhoneDriver = drivers.FirstOrDefault(d => d.Id == z.IdDriver) != null ? drivers.FirstOrDefault(d => d.Id == z.IdDriver).PhoneNumber : "",
                PhoneP = z.PhoneP,
                PickupExactly = z.PickupExactly,
                PriceListed = z.PriceListed,
                ShipVia = z.ShipVia,
                StateD = z.StateD,
                StateP = z.StateP,
                Titl1DI = z.Titl1DI,
                TotalPaymentToCarrier = z.TotalPaymentToCarrier,
                UrlReqvest = z.UrlReqvest,
                VehiclwInformations = z.VehiclwInformations,
                ZipD = z.ZipD,
                ZipP = z.ZipP
            }).ToList();*/
            return null;
        }
    }
}