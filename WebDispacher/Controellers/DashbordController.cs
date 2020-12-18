﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using MDispatch.View.GlobalDialogView;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class DashbordController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private string Status { get; set; }

        [HttpPost]
        [Route("New")]
        public async Task<string> New(string linck, string key)
        {
            string urlPage = linck.Remove(0, linck.IndexOf("'") + 1);
            urlPage = urlPage.Remove(urlPage.IndexOf("'"));
            urlPage = $"https://www.centraldispatch.com/{urlPage}";
            string actionResult = null;
            try
            {
                if (managerDispatch.CheckKeyDispatcher(key))
                {
                    Shipping shipping = await managerDispatch.AddNewOrder(urlPage);
                }
            }
            catch (Exception)
            {
            }
            return actionResult;
        }

        [Route("Dashbord/Order/NewLoad")]
        public async Task<IActionResult> NewLoad(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async() =>
                    {
                        ViewBag.Orders = await managerDispatch.GetOrders("NewLoad", page, name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("NewLoad", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("NewLoad");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }

            return actionResult;
        }

        [Route("Dashbord/Assign")]
        [HttpPost]
        public string DriverSelect(string idOrder, string idDriver)
        {
            bool actionResult = false;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    if ((idDriver != null && idDriver != "") && (idOrder != null && idOrder != ""))
                    {
                        managerDispatch.Assign(idOrder, idDriver);
                        Task.Run(() => managerDispatch.AddHistory(key, "0", idOrder, "0",  idDriver, "Assign"));
                        actionResult = true;
                    }
                    else
                    {
                        actionResult = false;
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = false;
                }
            }
            catch (Exception)
            {

            }
            return actionResult.ToString();
        }

        [Route("Dashbord/Unassign")]
        [HttpPost]
        public string DriverUnSelect(string idOrder)
        {
            bool actionResult = false;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    if (idOrder != null && idOrder != "")
                    {
                        managerDispatch.AddHistory(key, "0", idOrder, "0", "0", "Unassign");
                        managerDispatch.Unassign(idOrder);
                        actionResult = true;
                    }
                    else
                    {
                        actionResult = false;
                    }

                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = false;
                }
            }
            catch (Exception)
            {

            }
            return actionResult.ToString();
        }

        [Route("Dashbord/Order/Solved")]
        [HttpGet]
        public IActionResult Solved(string id, string page)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    managerDispatch.Solved(id);
                    Task.Run(() => managerDispatch.AddHistory(key, "0", id, "0", "0", "Solved"));
                    actionResult = Redirect($"{page}");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/Archived")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> Archived(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                List<Shipping> shippings = null;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings = await managerDispatch.GetOrders("Archived,Billed", page, name, address, phone, email, price);
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Archived,Paid", page, name, address, phone, email, price));
                        }
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Archived", page, name, address, phone, email, price));
                        }
                        List<Driver> drivers = await managerDispatch.GetDrivers(idCompany);
                        ViewBag.Drivers = drivers;
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Archived", name, address, phone, email, price);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Archived,Billed", name, address, phone, email, price);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Archived,Paid", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("Archived");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/Assigned")]
        public async Task<IActionResult> Assigned(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        List<Shipping> shippings = await managerDispatch.GetOrders("Assigned", page, name, address, phone, email, price);
                        List<Driver> drivers = await managerDispatch.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Assigned", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("Assigned");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/Billed")]
        public async Task<IActionResult> Billed(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        List<Shipping> shippings = await managerDispatch.GetOrders("Delivered,Billed", page, name, address, phone, email, price);
                        List<Driver> drivers = await managerDispatch.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Billed", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("Billed");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/Deleted")]
        public async Task<IActionResult> Deleted(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    List<Shipping> shippings = null;
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings = await managerDispatch.GetOrders("Deleted,Billed", page, name, address, phone, email, price);
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Deleted,Paid", page, name, address, phone, email, price));
                        }
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Deleted", page, name, address, phone, email, price));
                        }
                        List<Driver> drivers = await managerDispatch.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Deleted", name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Deleted,Billed", name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Deleted,Paid", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("Deleted");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/Delivered")]
        public async Task<IActionResult> Delivered(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    List<Shipping> shippings = new List<Shipping>();
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings.AddRange(await managerDispatch.GetOrders("Delivered,Paid", page, name, address, phone, email, price));
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Delivered,Billed", page, name, address, phone, email, price));
                        }
                        List<Driver> drivers = await managerDispatch.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Billed", name, address, phone, email, price);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Paid", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("Delivered");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        public async Task<IActionResult> Paid(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        List<Shipping> shippings = await managerDispatch.GetOrders("Delivered,Paid", page, name, address, phone, email, price);
                        List<Driver> drivers = await managerDispatch.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Paid", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("Paid");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/Pickedup")]
        public async Task<IActionResult> Pickedup(int page, string name, string address, string phone, string email, string price)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        List<Shipping> shippings = await managerDispatch.GetOrders("Picked up", page, name, address, phone, email, price);
                        List<Driver> drivers = await managerDispatch.GetDrivers(idCompany);
                        ViewBag.Orders = GetShippingDTOs(shippings, drivers);
                        ViewBag.Drivers = drivers;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Picked up", name, address, phone, email, price);
                    }));
                    ViewBag.Name = name;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;
                    ViewBag.Email = email;
                    ViewBag.price = price;
                    actionResult = View("Pickedup");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/ArchivedOrder")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult DeletedOrder(string id)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    managerDispatch.ArchvedOrder(id);
                    Task.Run(() => managerDispatch.AddHistory(key, "0", id, "0", "0", "ArchivedOrder"));
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/DeletedOrder")]
        public IActionResult DeletedOrder(string id, string status)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    managerDispatch.DeletedOrder(id);
                    Task.Run(() => managerDispatch.AddHistory(key, "0", id, "0", "0", "DeletedOrder"));
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{status}");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/FullInfoOrder")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult FullInfoOrder(string id, string stasus)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    if (id != "" && id != null)
                    {
                        ViewBag.Order = managerDispatch.GetOrder(id);
                        ViewBag.Historys = managerDispatch.GetHistoryOrder(id).Select(x => new HistoryOrder()
                        {
                            Action = managerDispatch.GetStrAction(key, x.IdConmpany.ToString(), x.IdOreder.ToString(), x.IdVech.ToString(), x.IdDriver.ToString(), x.TypeAction),
                            DateAction = x.DateAction
                        })
                        .ToList();
                        actionResult = View("FullInfoOrder");
                    }
                    else
                    {
                        actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{stasus}");
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }
        
        [Route("Dashbord/Order/Edit")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public IActionResult EditOrder(string id, string stasus)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    if (id != "" && id != null)
                    {
                        ViewBag.Order = managerDispatch.GetOrder(id);
                        actionResult = View("EditOrder");
                        Status = stasus;
                    }
                    else
                    {
                        actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/{stasus}");
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/Creat")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public async Task<IActionResult> CreatOrderpage()
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                string companyName = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                Request.Cookies.TryGetValue("CommpanyName", out companyName);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = managerDispatch.GetTypeNavBar(key, idCompany);
                    Shipping shipping = await managerDispatch.CreateShiping();
                    Task.Run(() => managerDispatch.AddHistory(key, "0", shipping.Id, "0", "0", "Creat"));
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={shipping.Id}&stasus=NewLoad");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/SavaOrder")]
        public IActionResult SaveOrder(string idOrder, string idLoad, string internalLoadID, string driver, string status, string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP, string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD, string paymentMethod, string price, string paymentTerms, string brokerFee)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    managerDispatch.Updateorder(idOrder, idLoad, internalLoadID, driver, status, instructions, nameP, contactP, addressP, cityP, stateP, zipP,
                        phoneP, emailP, scheduledPickupDateP, nameD, contactD, addressD, cityD, stateD, zipD, phoneD, emailD, ScheduledPickupDateD, paymentMethod,
                        price, paymentTerms, brokerFee);
                    Task.Run(() => managerDispatch.AddHistory(key, "0", idOrder, "0", "0", "SavaOrder"));
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/NewLoad");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception)
            {

            }
            return actionResult;
        }

        [Route("Dashbord/Order/SavaVech")]
        public IActionResult SavaVech(string idOrder, string idVech, string VIN, string Year, string Make, string Model, string Type, string Color, string LotNumber)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    managerDispatch.SaveVechi(idVech, VIN, Year, Make, Model, Type,  Color, LotNumber);
                    Task.Run(() => managerDispatch.AddHistory(key, "0", "0", idVech, "0", "SavaVech"));
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={idOrder}&stasus=NewLoad");
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Dashbord/Order/Edit?id={idOrder}&stasus=NewLoad");
                }
            }
            catch (Exception)
            {
                actionResult = null;
            }
            return actionResult;
        }

        [Route("Dashbord/Order/RemoveVech")]
        public string RemoveVech(string idVech)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    managerDispatch.AddHistory(key, "0", "0", idVech, "0", "RemoveVech");
                    managerDispatch.RemoveVechi(idVech);
                    actionResult = "Vehicle information removed successfully";
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = "Unauthorized user cannot change order";
                }
            }
            catch (Exception)
            {
                actionResult = "Vehicle information not removed (ERROR)";
            }
            return actionResult;
        }

        [Route("Dashbord/Order/AddVech")]
        public async Task<string> AddVech(string idOrder)
        {
            string actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (managerDispatch.CheckKey(key) && managerDispatch.IsPermission(key, idCompany, "Dashbord"))
                {
                    VehiclwInformation vehiclwInformation = await managerDispatch.AddVechi(idOrder);
                    Task.Run(() => managerDispatch.AddHistory(key, "0", idOrder, vehiclwInformation.Id.ToString(), "0", "AddVech"));
                    ViewBag.Vech = vehiclwInformation;
                    actionResult = "Vehicle information Added successfully";
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    actionResult = "Unauthorized user cannot change order";
                }
            }
            catch (Exception)
            {
                actionResult = "Vehicle information not Added (ERROR)";
            }
            return actionResult;
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