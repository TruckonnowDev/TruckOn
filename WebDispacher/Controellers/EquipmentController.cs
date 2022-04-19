using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    [Route("Equipment")]
    public class EquipmentController : Controller
    {
        private ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly IUserService userService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;

        public EquipmentController(
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            IOrderService orderService,
            ICompanyService companyService)
        {
            this.orderService = orderService;
            this.companyService = companyService;
            this.userService = userService;
            this.truckAndTrailerService = truckAndTrailerService;
        }

        [Route("Trucks")]
        public IActionResult Trucks()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Trucks = truckAndTrailerService.GetTrucks(idCompany);
                    actionResult = View($"AllTruck");
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

        [Route("Trailers")]
        public IActionResult Trailers()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Trailers = truckAndTrailerService.GetTrailers(idCompany);
                    actionResult = View($"AllTrailer");
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

        [Route("Truck/Remove")]
        public IActionResult RemoveTruck(string id)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    truckAndTrailerService.RemoveTruck(id);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
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

        [HttpGet]
        [Route("CreateTruck")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateDriver()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    actionResult = View("CreateTruck");
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

        [HttpPost]
        [Route("CreateTruck")]
        [DisableRequestSizeLimit]
        public IActionResult CreateDriver(string nameTruk, string yera, string make, string model, string typeTruk, string state, string exp, string vin, string owner, string plateTruk, string color, 
            IFormFile truckRegistrationDoc, IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, IFormFile nYHUTDoc)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    truckAndTrailerService.CreateTruk(nameTruk, yera, make, model, typeTruk, state, exp, vin, owner, plateTruk, color, idCompany, truckRegistrationDoc, truckLeaseAgreementDoc, truckAnnualInspection, bobTailPhysicalDamage, nYHUTDoc);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
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

        [Route("Trailer/Remove")]
        public IActionResult RemoveTrailer(string id)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    truckAndTrailerService.RemoveTrailer(id);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
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

        [HttpGet]
        [Route("CreateTrailer")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateTrailer()
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    actionResult = View("CreateTraler");
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

        [HttpPost]
        [Route("CreateTrailer")]
        [DisableRequestSizeLimit]
        public IActionResult CreateTrailer(string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns, 
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {

                    truckAndTrailerService.CreateTrailer(name, typeTrailer, year, make, howLong, vin, owner, color, plate, exp, annualIns, idCompany, trailerRegistrationDoc, trailerAnnualInspectionDoc, leaseAgreementDoc);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
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

        [HttpGet]
        [Route("EditTruck")]
        public IActionResult EditTruck(int idTruck)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Truck = truckAndTrailerService.GetTruckById(idTruck);
                    actionResult = View("EditTruck");
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

        [HttpPost]
        [Route("EditTruck")]
        [DisableRequestSizeLimit]
        public IActionResult EditTruck(int idTruck, string nameTruk, string yera, string make, string model, string typeTruk, string state, string exp, string vin, string owner, string plateTruk, string color)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    truckAndTrailerService.EditTruck(idTruck, nameTruk, yera, make, model, typeTruk, state, exp, vin, owner, plateTruk, color);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
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

        [HttpGet]
        [Route("EditTrailer")]
        public IActionResult EditTrailer(int idTrailer)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.Trailer = truckAndTrailerService.GetTrailerById(idTrailer);
                    actionResult = View("EditTrailer");
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

        [HttpPost]
        [Route("EditTrailer")]
        [DisableRequestSizeLimit]
        public IActionResult EditTrailer(int idTrailer, string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns)
        {
            IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {

                    truckAndTrailerService.EditTrailer(idTrailer, name, typeTrailer, year, make, howLong, vin, owner, color, plate, exp, annualIns);
                    actionResult = Redirect($"{Config.BaseReqvesteUrl}/Equipment/trailers");
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

        [HttpPost]
        [Route("SaveFile")]
        public string AddFile(IFormFile uploadedFile, string id)
        {
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvthoTaxi", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    if (uploadedFile != null)
                    {
                        string path = $"../Document/Truck/{id}/" + uploadedFile.FileName;
                        Directory.CreateDirectory($"../Document/Truck/{id}");
                        orderService.SavePath(id, path);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            uploadedFile.CopyTo(fileStream);
                        }
                    }
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvthoTaxi"))
                    {
                        Response.Cookies.Delete("KeyAvthoTaxi");
                    }
                }
            }
            catch (Exception e)
            {

            }
            return "true";
        }

        [HttpGet]
        [Route("Document")]
        public async Task<IActionResult> Get(string id)
        {
            FileStream stream = null;
            try
            {
                string docPath = await orderService.GetDocument(id);
                stream = new FileStream(docPath, FileMode.Open);
            }
            catch (Exception)
            {
                stream = null;
            }
            return new FileStreamResult(stream, "application/pdf");
        }

        [Route("Truck/Doc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTruckDoc(string id)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.TruckDoc = await truckAndTrailerService.GetTruckDoc(id);
                    ViewBag.TruckId = id;
                    actionResult = View("DocTruck");
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
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [Route("Trailer/Doc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTraileDoc(string id)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                    if (isCancelSubscribe)
                    {
                        return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                    }
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.TrailerDoc = await truckAndTrailerService.GetTraileDoc(id);
                    ViewBag.TrailerId = id;
                    actionResult = View("DocTrailer");
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
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [Route("Truck/SaveDoc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public void SaveDoc(IFormFile uploadedFile, string nameDoc, string id)
        {
            //IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    truckAndTrailerService.SaveDocTruck(uploadedFile, nameDoc, id);
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    //actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            //return actionResult;
        }

        [Route("Trailer/SaveDoc")]
        public void SaveDoc1(IFormFile uploadedFile, string nameDoc, string id)
        {
            //IActionResult actionResult = null;
            try
            {
                string key = null;
                string idCompany = null;
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                Request.Cookies.TryGetValue("KeyAvtho", out key);
                Request.Cookies.TryGetValue("CommpanyId", out idCompany);
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    truckAndTrailerService.SaveDocTrailer(uploadedFile, nameDoc, id);
                }
                else
                {
                    if (Request.Cookies.ContainsKey("KeyAvtho"))
                    {
                        Response.Cookies.Delete("KeyAvtho");
                    }
                    //actionResult = Redirect(Config.BaseReqvesteUrl);
                }
            }
            catch (Exception e)
            {

            }
            //return actionResult;
        }

        [Route("RemoveDoc")]
        public IActionResult RemoveDoc(string idDock, string id, string type)
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
                if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "Equipment"))
                {
                    orderService.RemoveDoc(idDock);
                    actionResult = Redirect($"{type}/Doc?id={id}");
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
            catch (Exception e)
            {

            }
            return actionResult;
        }

        [Route("GetDock")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDock(string docPath, string type)
        {
            IActionResult actionResult = null;
            var imageFileStream = System.IO.File.OpenRead(docPath);
            actionResult = File(imageFileStream, type);
            return actionResult;
        }

        [Route("GetDockPDF")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDockPDF(string docPath)
        {
            IActionResult actionResult = null;
            var imageFileStream = System.IO.File.OpenRead(docPath);
            actionResult = File(imageFileStream, "application/pdf");
            return actionResult;
        }

        [HttpGet]
        [Route("Image")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetShiping(string name, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(name);
            return File(imageFileStream, $"image/{type}");
        }
    }
}