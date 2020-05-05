﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
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
        public IActionResult New(string linck)
        {
            string urlPage = linck.Remove(0, linck.IndexOf("'") + 1);
            urlPage = urlPage.Remove(urlPage.IndexOf("'"));
            urlPage = $"https://www.centraldispatch.com/{urlPage}";
            IActionResult actionResult = null;
            try
            {
                managerDispatch.AddNewOrder(urlPage);
                actionResult = Ok();
            }
            catch (Exception)
            {
                actionResult = null;
            }

            return actionResult;
        }

        [Route("Dashbord/Order/NewLoad")]
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public async Task<IActionResult> NewLoad(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    await Task.WhenAll(
                    Task.Run(async() =>
                    {
                        ViewBag.Orders = await managerDispatch.GetOrders("NewLoad", page);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("NewLoad");
                    }));
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
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                string key = null;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
                {
                    if((idDriver != null && idDriver != "") && (idOrder != null && idOrder != ""))
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
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
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
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                string key = null;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
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
        public async Task<IActionResult> Archived(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                List<Shipping> shippings = null;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    await Task.WhenAll(
                    Task.Run(async() =>
                    {
                        shippings = await managerDispatch.GetOrders("Archived,Billed", page);
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Archived,Paid", page));
                        }
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Archived", page));
                        }
                        ViewBag.Orders = shippings;
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Archived");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Archived,Billed");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Archived,Paid");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }));
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
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public async Task<IActionResult> Assigned(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        ViewBag.Orders = await managerDispatch.GetOrders("Assigned", page);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Assigned");
                    }));
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
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> Billed(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        ViewBag.Orders = await managerDispatch.GetOrders("Delivered,Billed", page);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Billed");
                    }));
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
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> Deleted(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    List<Shipping> shippings = null;
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings = await managerDispatch.GetOrders("Deleted,Billed", page);
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Deleted,Paid", page));
                        }
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Deleted", page));
                        }
                        ViewBag.Orders = shippings;
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Deleted");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Deleted,Billed");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Deleted,Paid");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }));
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
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> Delivered(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    List<Shipping> shippings = new List<Shipping>();

                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        shippings.AddRange(await managerDispatch.GetOrders("Delivered,Paid", page));
                        if (shippings.Count < 20)
                        {
                            shippings.AddRange(await managerDispatch.GetOrders("Delivered,Billed", page));
                        }
                        ViewBag.Orders = shippings;
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Billed");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Paid");
                    }),
                    Task.Run(async() =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }));
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

        [Route("Dashbord/Order/Paid")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> Paid(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        ViewBag.Orders = await managerDispatch.GetOrders("Delivered,Paid", page);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Delivered,Paid");
                    }));
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
        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 300)]
        public async Task<IActionResult> Pickedup(int page)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key) && Request.Cookies.TryGetValue("CommpanyId", out idCompany))
                {
                    await Task.WhenAll(
                    Task.Run(async () =>
                    {
                        ViewBag.Orders = await managerDispatch.GetOrders("Picked up", page);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.Drivers = await managerDispatch.GetDrivers(idCompany);
                    }),
                    Task.Run(async () =>
                    {
                        ViewBag.count = await managerDispatch.GetCountPage("Picked up");
                    }));
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
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
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
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
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
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
                {
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
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
                {
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
            ViewData["TypeNavBar"] = "BaseCommpany";
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
                {
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
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
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
        public string SavaVech(string idVech, string VIN, string Year, string Make, string Model, string Type, string Color, string LotNumber)
        {
            string actionResult = null;
            try
            {
                string key = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
                {
                    managerDispatch.SaveVechi(idVech, VIN, Year, Make, Model, Type,  Color, LotNumber);
                    Task.Run(() => managerDispatch.AddHistory(key, "0", "0", idVech, "0", "SavaVech"));
                    actionResult = "Vehicle information saved successfully";
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
                actionResult = "Vehicle information not saved (ERROR)";
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
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
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
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                if (managerDispatch.CheckKey(key))
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
    }
}