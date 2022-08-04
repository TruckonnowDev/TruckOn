using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using MDispatch.View.GlobalDialogView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Models;
using WebDispacher.Service;
using WebDispacher.ViewModels;

namespace WebDispacher.Controellers
{
    public class DashbordController : Controller
    {
        private readonly IUserService userService;
        private readonly ICompanyService companyService;
        private readonly IDriverService driverService;
        private readonly IOrderService orderService;

        public DashbordController(
            IOrderService orderService,
            IUserService userService,
            ICompanyService companyService,
            IDriverService driverService)
        {
            this.orderService = orderService;
            this.driverService = driverService;
            this.userService = userService;
            this.companyService = companyService;
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
                    var shipping = await orderService.AddNewOrder(urlPage, dispatcher);
                }
            }
            catch (Exception exception)
            {
                // ignored
            }

            return null;
        }

        [Route("Dashbord/Order/NewLoad")]
        public async Task<IActionResult> NewLoad(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if(isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    ViewBag.Orders = await orderService.GetOrders(OrderConstants.OrderStatusNewLoad, page, loadId, name, address, phone, email, price);

                    ViewBag.Drivers = await driverService.GetDrivers(idCompany);

                    var countPage = await orderService.GetCountPage(OrderConstants.OrderStatusNewLoad, loadId, name, address, phone, email, price);

                    ViewBag.count = orderService.GetCountPage(countPage);
                    
                    ViewBag.LoadId = loadId;
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("NewLoad");
                }
                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception exception)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Assign")]
        [HttpPost]
        public string DriverSelect(string idOrder, string idDriver)
        {
            var actionResult = false;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    if (!string.IsNullOrEmpty(idDriver) && !string.IsNullOrEmpty(idOrder))
                    {
                        orderService.Assign(idOrder, idDriver);
                        Task.Run(() => 
                            orderService.AddHistory(key, "0", idOrder, "0",  idDriver, OrderConstants.ActionAssign));
                        actionResult = true;
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
                }
            }
            catch (Exception exception)
            {

            }
            
            return actionResult.ToString();
        }

        [Route("Dashbord/Unassign")]
        [HttpPost]
        public string DriverUnSelect(string idOrder)
        {
            var actionResult = false;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    if (!string.IsNullOrEmpty(idOrder))
                    {
                        orderService.AddHistory(key, "0", idOrder, "0", "0", OrderConstants.ActionUnAssign);
                        orderService.Unassign(idOrder);
                        actionResult = true;
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                    {
                        Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                    }
                }
            }
            catch (Exception exception)
            {
            }
            
            return actionResult.ToString();
        }

        [Route("Dashbord/Order/Solved")]
        [HttpGet]
        public IActionResult Solved(string id, string page)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    orderService.Solved(id);
                    Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", OrderConstants.ActionSolved));
                    
                    return Redirect($"{page}");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Archived")]
        public async Task<IActionResult> Archived(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    var shippings = await orderService.GetOrders(OrderConstants.OrderStatusArchivedBilled, page, loadId, name, address, phone, email, price);
                    if (shippings.Count < 20)
                    {
                        shippings.AddRange(await orderService.GetOrders(OrderConstants.OrderStatusArchivedPaid, page, loadId,  name, address, phone, email, price));
                    }
                    if (shippings.Count < 20)
                    {
                        shippings.AddRange(await orderService.GetOrders(OrderConstants.OrderStatusArchived, page, loadId, name, address, phone, email, price));
                    }
                    var drivers = await driverService.GetDrivers(idCompany);
                    
                    ViewBag.Drivers = drivers;
                    ViewBag.Orders = GetShippingDTOs(shippings, drivers);

                    var countPage = await orderService.GetCountPage(OrderConstants.OrderStatusArchived, loadId, name, address, phone, email, price);

                    countPage += await orderService.GetCountPage(OrderConstants.OrderStatusArchivedBilled, loadId, name, address, phone, email, price);

                    countPage += await orderService.GetCountPage(OrderConstants.OrderStatusArchivedPaid, loadId, name, address, phone, email, price);

                    ViewBag.count = orderService.GetCountPage(countPage);
                    
                    ViewBag.LoadId = loadId;
                    
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Archived");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception exception)
            {
                
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        

        [Route("Dashbord/Order/Assigned")]
        public async Task<IActionResult> Assigned(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    
                    var shippings =
                        await orderService.GetOrders(OrderConstants.OrderStatusAssigned, page, loadId, name, address, phone, email, price);
                    var drivers = await driverService.GetDrivers(idCompany);
                    
                    ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    ViewBag.Drivers = drivers;

                    var countPage = await orderService.GetCountPage(OrderConstants.OrderStatusAssigned, loadId, name, address, phone, email, price);

                    ViewBag.count = orderService.GetCountPage(countPage);
                    
                    ViewBag.LoadId = loadId;
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Assigned");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Billed")]
        public async Task<IActionResult> Billed(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    var shippings =
                        await orderService.GetOrders(OrderConstants.OrderStatusDeliveredBilled, page, loadId, name, address, phone, email, price);
                    
                    ViewBag.Drivers = await driverService.GetDrivers(idCompany);;
                    ViewBag.Orders = GetShippingDTOs(shippings, ViewBag.Drivers);
                    
                    var countPage = await orderService.GetCountPage(OrderConstants.OrderStatusDeliveredBilled, loadId, name, address, phone, email, price);

                    ViewBag.count = orderService.GetCountPage(countPage);
                    
                    ViewBag.LoadId = loadId;
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Billed");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Deleted")]
        public async Task<IActionResult> Deleted(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
        
                    var shippings = await orderService.GetOrders(OrderConstants.OrderStatusDeletedBilled, page, loadId, name, address, phone, email, price);
                    
                    if (shippings.Count < 20)
                    {
                        shippings.AddRange(await orderService.GetOrders(OrderConstants.OrderStatusDeletedPaid, page, loadId, name, address, phone, email, price));
                    }
                    
                    if (shippings.Count < 20)
                    {
                        shippings.AddRange(await orderService.GetOrders(OrderConstants.OrderStatusDeleted, page, loadId, name, address, phone, email, price));
                    }
                    
                    var drivers = await driverService.GetDrivers(idCompany);
                    
                    ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    
                    ViewBag.Drivers = drivers;
        
                    var countPage = await orderService.GetCountPage(OrderConstants.OrderStatusDeleted, loadId, name, address, phone, email, price);
        
                    countPage += await orderService.GetCountPage(OrderConstants.OrderStatusDeletedBilled, loadId, name, address, phone, email, price);
        
                    countPage += await orderService.GetCountPage(OrderConstants.OrderStatusDeletedPaid, loadId, name, address, phone, email, price);
        
                    countPage = orderService.GetCountPage(countPage);

                    ViewBag.count = countPage;
                    
                    ViewBag.LoadId = loadId;
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Deleted");
                }
        
                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {
        
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Delivered")]
        public async Task<IActionResult> Delivered(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    var shippings = await orderService.GetOrders(OrderConstants.OrderStatusDeliveredPaid, page, loadId, name, address, phone, email, price);
                    
                    if (shippings.Count < 20)
                    {
                        shippings.AddRange(await orderService.GetOrders(OrderConstants.OrderStatusDeliveredBilled, page, loadId, name, address, phone, email, price));
                    }
                    
                    var drivers = await driverService.GetDrivers(idCompany);
                    
                    ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    ViewBag.Drivers = drivers;

                    var countPage = await orderService.GetCountPage(OrderConstants.OrderStatusDeliveredBilled, loadId, name, address, phone, email, price);

                    countPage += await orderService.GetCountPage(OrderConstants.OrderStatusDeliveredPaid, loadId, name, address, phone, email, price);

                    ViewBag.count = orderService.GetCountPage(countPage);
                    
                    ViewBag.LoadId = loadId;
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Delivered");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        public async Task<IActionResult> Paid(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany,  RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    var shippings =
                        await orderService.GetOrders(OrderConstants.OrderStatusDeliveredPaid, page, loadId, name, address, phone, email, price);
                    
                    var drivers = await driverService.GetDrivers(idCompany);
                    ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    ViewBag.Drivers = drivers;

                    var countPage = await 
                        orderService.GetCountPage(OrderConstants.OrderStatusDeliveredPaid, loadId, name, address, phone, email, price);
                    
                    ViewBag.count = orderService.GetCountPage(countPage);
                    
                    ViewBag.LoadId = loadId;
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Paid");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Pickedup")]
        public async Task<IActionResult> Pickedup(int page, string loadId, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    var shippings = await orderService.GetOrders(OrderConstants.OrderStatusPickedUp, page, loadId, name, address, phone, email, price);
                    
                    var drivers = await driverService.GetDrivers(idCompany);
                    
                    ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    ViewBag.Drivers = drivers;

                    var countPage = await orderService.GetCountPage(OrderConstants.OrderStatusPickedUp, loadId, name, address, phone, email, price);

                    ViewBag.count = orderService.GetCountPage(countPage);
                    
                    ViewBag.LoadId = loadId;
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;

                    return View("Pickedup");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/DeletedOrder")]
        public async Task<IActionResult> DeletedOrder(string id, string status, string filters)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    await orderService.DeleteOrder(id);
                    await Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", OrderConstants.ActionDeletedOrder));
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}?{filters}");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Dashbord/Order/ArchivedOrder")]
        public async Task<IActionResult> ArchivedOrder(string id, string status, string filters)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    await orderService.ArchiveOrder(id);
                    
                    await Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", OrderConstants.ActionArchivedOrder));

                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}?{filters}");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/FullInfoOrder")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult FullInfoOrder(string id, string status)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    if (string.IsNullOrEmpty(id)) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                    var order = orderService.GetOrder(id);
                    ViewBag.Historys = orderService.GetHistoryOrder(id).Select(x => new HistoryOrder()
                        {
                            Action = orderService.GetStrAction(key, x.IdConmpany.ToString(), x.IdOreder.ToString(),
                                x.IdVech.ToString(), x.IdDriver.ToString(), x.TypeAction),
                            DateAction = x.DateAction
                        })
                        .ToList();
                    
                    return View("FullInfoOrder", order);
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
         [Route("Dashbord/Order/Edit")]
         [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
         public IActionResult EditOrder(string id, string status)
         {
             try
             {
                 ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                 Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                 Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                 Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                 
                 if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                 {
                     ViewBag.NameCompany = companyName;
                     ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                     if (string.IsNullOrEmpty(id)) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                     
                     var order = orderService.GetOrder(id);
                     
                     Status = status;
                     
                     return View(order);
                 }

                 if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                 {
                     Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                 }
             }
             catch (Exception)
             {
         
             }
             
             return Redirect(Config.BaseReqvesteUrl);
         }
         
        [Route("Dashbord/Order/EditReload")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult EditReload(string id, string status, ShippingViewModel model)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);

                    if (string.IsNullOrEmpty(id)) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                    var searchOrder = orderService.GetOrder(id);
                        
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
                        
                    return View("EditOrder", order);

                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Dashbord/Order/Creat")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public async Task<IActionResult> CreatOrderpage()
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyNameKey, out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    var shipping = await orderService.CreateShipping();
                    await Task.Run(() => orderService.AddHistory(key, "0", shipping.Id, "0", "0", OrderConstants.ActionCreate));
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={shipping.Id}&status=NewLoad");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/SavaOrder")]
        public async Task<IActionResult> SaveOrder(ShippingViewModel shipping)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var updatedOrder = await orderService.UpdateOrder(shipping);
                    
                    await Task.Run(() => orderService.AddHistory(key, "0", shipping.Id, "0", "0", OrderConstants.ActionSaveOrder));
                    
                    return Redirect(updatedOrder == null ? 
                        $"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad" : $"{Config.BaseReqvesteUrl}/Dashbord/Order/{updatedOrder.CurrentStatus}");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/SavaVech")]
        public async Task<IActionResult> SavaVech(string idOrder, string idVech, string VIN, string Year,
            string Make, string Model, string Type, string Color, string LotNumber)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                   await orderService.SaveVechi(idVech, VIN, Year, Make, Model, Type,  Color, LotNumber);
                   await Task.Run(() => orderService.AddHistory(key, "0", "0", idVech, "0", OrderConstants.ActionSaveOrder));
                    
                   return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={idOrder}&status=NewLoad");
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
            }
            catch (Exception)
            {
                return null;
            }
            
            return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={idOrder}&status=NewLoad");
        }

        [Route("Dashbord/Order/RemoveVech")]
        public string RemoveVech(string idVech)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    orderService.AddHistory(key, "0", "0", idVech, "0", OrderConstants.ActionRemoveVech);
                    orderService.RemoveVechi(idVech);
                    
                    return OrderConstants.SuccessfullyRemovedVehicle;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
                return OrderConstants.UnAuthorizedUserCannotChangeOrder;
            }
            catch (Exception)
            {
                return OrderConstants.ErrorRemovedVehicle;
            }
        }

        [Route("Dashbord/Order/AddVech")]
        public async Task<string> AddVech(string idOrder)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue(CookiesKeysConstants.CarKey, out var key);
                Request.Cookies.TryGetValue(CookiesKeysConstants.CompanyIdKey, out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, RouteConstants.Dashboard))
                {
                    var vehiclwInformation = await orderService.AddVechi(idOrder);
                    
                    await Task.Run(() => orderService.AddHistory(key, "0", idOrder, vehiclwInformation.Id.ToString(),
                        "0", OrderConstants.ActionAddVech));
                    ViewBag.Vech = vehiclwInformation;
                    return OrderConstants.SuccessfullyAddedVehicle;
                }

                if (Request.Cookies.ContainsKey(CookiesKeysConstants.CarKey))
                {
                    Response.Cookies.Delete(CookiesKeysConstants.CarKey);
                }
                
                return OrderConstants.UnAuthorizedUserCannotChangeOrder;
            }
            catch (Exception)
            {
                return OrderConstants.ErrorAddedVehicle;
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

        private List<ShippingDTO> GetShippingDTOs(List<Shipping> shippings, List<Driver> drivers)
        {
            return shippings.Select(z => new ShippingDTO()
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
            }).ToList();
        }
    }
}