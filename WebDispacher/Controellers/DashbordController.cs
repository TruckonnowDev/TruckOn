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
        public async Task<IActionResult> NewLoad(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if(isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async() =>
                    {
                        ViewBag.Orders = await orderService.GetOrders("NewLoad", page, name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.Drivers = await driverService.GetDrivers(idCompany);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await orderService.GetCountPage("NewLoad", name, address, phone, email, price);
                    }));
                    
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("NewLoad");
                }
                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
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
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    if (!string.IsNullOrEmpty(idDriver) && !string.IsNullOrEmpty(idOrder))
                    {
                        orderService.Assign(idOrder, idDriver);
                        Task.Run(() => orderService.AddHistory(key, "0", idOrder, "0",  idDriver, "Assign"));
                        actionResult = true;
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
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
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Dashbord"))
                {
                    if (!string.IsNullOrEmpty(idOrder))
                    {
                        orderService.AddHistory(key, "0", idOrder, "0", "0", "Unassign");
                        orderService.Unassign(idOrder);
                        actionResult = true;
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
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
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    orderService.Solved(id);
                    Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", "Solved"));
                    
                    return Redirect($"{page}");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Archived")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> Archived(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    
                    List<Shipping> shippings = null;
                    
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings = await orderService.GetOrders("Archived,Billed", page, name, address, phone, email, price);
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await orderService.GetOrders("Archived,Paid", page, name, address, phone, email, price));
                        }
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await orderService.GetOrders("Archived", page, name, address, phone, email, price));
                        }
                        var drivers = await driverService.GetDrivers(idCompany);
                        ViewBag.Drivers = drivers;
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Archived", name, address, phone, email, price);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Archived,Billed", name, address, phone, email, price);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Archived,Paid", name, address, phone, email, price);
                    }));
                    
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Archived");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception exception)
            {
                
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        

        [Route("Dashbord/Order/Assigned")]
        public async Task<IActionResult> Assigned(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        var shippings = await orderService.GetOrders("Assigned", page, name, address, phone, email, price);
                        var drivers = await driverService.GetDrivers(idCompany);
                        
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Assigned", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Assigned");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Billed")]
        public async Task<IActionResult> Billed(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany); 
                    // await Task.WhenAll(
                    // Task.Run(async () =>
                    // {
                    //     var shippings = await orderService.GetOrders("Delivered,Billed", page, name, address, phone, email, price);
                    //     var drivers = await driverService.GetDrivers(idCompany);
                    //     ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    //     ViewBag.Drivers = drivers;
                    // }),
                    // Task.Run(async () =>
                    // {
                    //     ViewBag.count = await orderService.GetCountPage("Delivered,Billed", name, address, phone, email, price);
                    // }));

                    var shippings = await orderService.GetOrders("Delivered,Billed", page, name, address, phone, email, price);
                    ViewBag.Drivers = await driverService.GetDrivers(idCompany);;
                    ViewBag.Orders = GetShippingDTOs(shippings, ViewBag.Drivers);
                    ViewBag.count = await orderService.GetCountPage("Delivered,Billed", name, address, phone, email, price);
                    
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Billed");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Deleted")]
        public async Task<IActionResult> Deleted(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    List<Shipping> shippings = null;
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings = await orderService.GetOrders("Deleted,Billed", page, name, address, phone, email, price);
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await orderService.GetOrders("Deleted,Paid", page, name, address, phone, email, price));
                        }
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await orderService.GetOrders("Deleted", page, name, address, phone, email, price));
                        }
                        var drivers = await driverService.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Deleted", name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Deleted,Billed", name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Deleted,Paid", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    
                    return View("Deleted");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Delivered")]
        public async Task<IActionResult> Delivered(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    
                    var shippings = new List<Shipping>();
                    
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings.AddRange(await orderService.GetOrders("Delivered,Paid", page, name, address, phone, email, price));
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await orderService.GetOrders("Delivered,Billed", page, name, address, phone, email, price));
                        }
                        var drivers = await driverService.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Delivered,Billed", name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Delivered,Paid", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    return View("Delivered");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        public async Task<IActionResult> Paid(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        var shippings = await orderService.GetOrders("Delivered,Paid", page, name, address, phone, email, price);
                        var drivers = await driverService.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Delivered,Paid", name, address, phone, email, price);
                    }));
                    
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    return View("Paid");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/Pickedup")]
        public async Task<IActionResult> Pickedup(int page, string name, string address, string phone, string email, string price)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        var shippings = await orderService.GetOrders("Picked up", page, name, address, phone, email, price);
                        var drivers = await driverService.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await orderService.GetCountPage("Picked up", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    return View("Pickedup");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/ArchivedOrder")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult DeletedOrder(string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    orderService.ArchiveOrder(id);
                    Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", "ArchivedOrder"));
                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/DeletedOrder")]
        public IActionResult DeletedOrder(string id, string status)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    orderService.DeleteOrder(id);
                    Task.Run(() => orderService.AddHistory(key, "0", id, "0", "0", "DeletedOrder"));
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/FullInfoOrder")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult FullInfoOrder(string id, string stasus)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);

                    if (string.IsNullOrEmpty(id)) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{stasus}");
                    var order = orderService.GetOrder(id);
                    ViewBag.Historys = orderService.GetHistoryOrder(id).Select(x => new HistoryOrder()
                        {
                            Action = orderService.GetStrAction(key, x.IdConmpany.ToString(), x.IdOreder.ToString(), x.IdVech.ToString(), x.IdDriver.ToString(), x.TypeAction),
                            DateAction = x.DateAction
                        })
                        .ToList();
                    
                    return View("FullInfoOrder", order);
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
         [Route("Dashbord/Order/Edit")]
         [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
         public IActionResult EditOrder(string id, string stasus)
         {
             try
             {
                 ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                 Request.Cookies.TryGetValue("KeyAvtho", out var key);
                 Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                 Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                 
                 if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                 {
                     ViewBag.NameCompany = companyName;
                     ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);

                     if (string.IsNullOrEmpty(id)) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{stasus}");
                     
                     var order = orderService.GetOrder(id);
                     Status = stasus;
                         
                     return View("EditOrder", order);

                 }

                 if (Request.Cookies.ContainsKey("KeyAvtho"))
                 {
                     Response.Cookies.Delete("KeyAvtho");
                 }
             }
             catch (Exception)
             {
         
             }
             
             return Redirect(Config.BaseReqvesteUrl);
         }
         
        [Route("Dashbord/Order/EditReload")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult EditReload(string id, string stasus, ShippingViewModel model)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);

                    if (string.IsNullOrEmpty(id)) return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{stasus}");
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
                        
                    Status = stasus;
                        
                    return View("EditOrder", order);

                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
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
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out var companyName);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    var shipping = await orderService.CreateShipping();
                    await Task.Run(() => orderService.AddHistory(key, "0", shipping.Id, "0", "0", "Creat"));
                    
                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={shipping.Id}&stasus=NewLoad");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/SavaOrder")]
        public IActionResult SaveOrder(ShippingViewModel shipping)
        {
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    orderService.UpdateOrder(shipping);
                    Task.Run(() => orderService.AddHistory(key, "0", shipping.Id, "0", "0", "SavaOrder"));
                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
                
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Dashbord/Order/SavaVech")]
        public IActionResult SavaVech(string idOrder, string idVech, string VIN, string Year,
            string Make, string Model, string Type, string Color, string LotNumber)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    orderService.SaveVechi(idVech, VIN, Year, Make, Model, Type,  Color, LotNumber);
                    Task.Run(() => orderService.AddHistory(key, "0", "0", idVech, "0", "SavaVech"));
                    return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={idOrder}&stasus=NewLoad");
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
            }
            catch (Exception)
            {
                return null;
            }
            
            return Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={idOrder}&stasus=NewLoad");
        }

        [Route("Dashbord/Order/RemoveVech")]
        public string RemoveVech(string idVech)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    orderService.AddHistory(key, "0", "0", idVech, "0", "RemoveVech");
                    orderService.RemoveVechi(idVech);
                    
                    return "Vehicle information removed successfully";
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
                return "Unauthorized user cannot change order";
            }
            catch (Exception)
            {
                return "Vehicle information not removed (ERROR)";
            }
        }

        [Route("Dashbord/Order/AddVech")]
        public async Task<string> AddVech(string idOrder)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out var key);
                Request.Cookies.TryGetValue("CommpanyId", out var idCompany);
                
                if (userService.CheckPermissions(key, idCompany, "Dashbord"))
                {
                    var vehiclwInformation = await orderService.AddVechi(idOrder);
                    
                    await Task.Run(() => orderService.AddHistory(key, "0", idOrder, vehiclwInformation.Id.ToString(), "0", "AddVech"));
                    ViewBag.Vech = vehiclwInformation;
                    return "Vehicle information Added successfully";
                }

                if (Request.Cookies.ContainsKey("KeyAvtho"))
                {
                    Response.Cookies.Delete("KeyAvtho");
                }
                
                return "Unauthorized user cannot change order";
            }
            catch (Exception)
            {
                return "Vehicle information not Added (ERROR)";
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