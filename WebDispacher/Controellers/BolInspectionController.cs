using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class BolInspectionController : Controller
    {
        ManagerDispatch managerDispatch = new ManagerDispatch();
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly IUserService userService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;

        public BolInspectionController(
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

        [Route("Photo/BOL/{idVech}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetPhotoInspection(int idVech)
        {
            IActionResult actionResult = null; 
            string key = null;
            string idCompany = null;
            string companyName = null;
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            Request.Cookies.TryGetValue("KeyAvtho", out key);
            Request.Cookies.TryGetValue("CommpanyId", out idCompany);
            Request.Cookies.TryGetValue("CommpanyName", out companyName);
            if (userService.CheckKey(key) && userService.IsPermission(key, idCompany, "BOL"))
            {
                bool isCancelSubscribe = companyService.GetCancelSubscribe(idCompany);
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                Shipping shipping = orderService.GetShippingCurrentVehiclwIn(idVech.ToString());
                VehiclwInformation vehiclwInformation = shipping.VehiclwInformations.FirstOrDefault(v => v.Id == idVech);
                if (shipping != null)
                {
                    ViewBag.NameCompany = companyName;
                    ViewData["TypeNavBar"] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    ViewBag.Shipp = shipping;
                    ViewBag.Vehiclw = vehiclwInformation;
                    actionResult = View("InspectionVech");
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
            return actionResult;
        }

        [Route("Welcome/Photo/BOL/{idVech}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetWelcomePhotoInspection(int idVech)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseAllUsers";
            Shipping shipping = orderService.GetShippingCurrentVehiclwIn(idVech.ToString());
            VehiclwInformation vehiclwInformation = shipping.VehiclwInformations.FirstOrDefault(v => v.Id == idVech);
            if (shipping != null)
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.Shipp = shipping;
                ViewBag.Vehiclw = vehiclwInformation;
                actionResult = View("WelcomeInspectionVech");
            }
            return actionResult;
        }

        [Route("Doc/{idDriver}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTruckDoc(string idDriver)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseAllUsers";

            Truck truck = null;
            Trailer trailer = null;

            await Task.WhenAll(
            Task.Run(async() => 
            {
                truck = await truckAndTrailerService.GetTruck(idDriver); 
                ViewBag.Truck = truck;
                ViewBag.TruckDoc = await truckAndTrailerService.GetTruckDoc((truck != null ? truck.Id : 0).ToString());
            }),
            Task.Run(async() =>
            {
                trailer = await truckAndTrailerService.GetTrailer(idDriver);
                ViewBag.Trailer = trailer;
                ViewBag.TrailerDoc = await truckAndTrailerService.GetTrailerDoc((trailer != null ? trailer.Id : 0).ToString());
            }));

            actionResult = View($"DocDriver");
            return actionResult;
        }

        [Route("Doc")]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTruckDoc(string truckPlate, string trailerPlate)
        {
            IActionResult actionResult = null;
            ViewData["TypeNavBar"] = "BaseAllUsers";

            Truck truck = null;
            Trailer trailer = null;

            await Task.WhenAll(
            Task.Run(async () =>
            {
                truck = await truckAndTrailerService.GetTruckByPlate(truckPlate);
                ViewBag.Truck = truck;
                ViewBag.TruckDoc = await truckAndTrailerService.GetTruckDoc((truck != null ? truck.Id : 0).ToString());
            }),
            Task.Run(async () =>
            {
                trailer = await truckAndTrailerService.GetTrailerByPlate(trailerPlate);
                ViewBag.Trailer = trailer;
                ViewBag.TrailerDoc = await truckAndTrailerService.GetTrailerDoc((trailer != null ? trailer.Id : 0).ToString());
            }));

            actionResult = View($"DocDriver");
            return actionResult;
        }

        [HttpGet]
        [Route(".well-known/acme-challenge/{file}")]
        public string GetWellKnownFile(string file)
        {
            string actionResult = "";
            if (System.IO.File.Exists($"../Root/{file}"))
            {
                var imageFileStream = System.IO.File.OpenRead($"../Root/{file}");
                actionResult = System.IO.File.ReadAllText($"../Root/{file}");
            }
            else if (System.IO.Directory.Exists($"../Root/{file}"))
            {
                List<string> files = System.IO.Directory.GetFiles($"../Root/{file}").ToList();
                files.AddRange(System.IO.Directory.GetDirectories($"../Root/{file}").ToList());
                foreach (string file1 in files)
                {
                    actionResult += file1.Remove(0, file1.LastIndexOf(@"\") + 1) + "\n";
                }
            }
            else
            {
                actionResult = "No such file exists";
            }
            return actionResult;
        }

        [HttpGet]
        [Route(".well-known/acme-challenge")]
        public string GetWellKnownOnlyFile()
        {
            string actionResult = "";
            List<string> files = System.IO.Directory.GetFiles(@"..\Root").ToList();
            files.AddRange(System.IO.Directory.GetDirectories(@"..\Root").ToList());
            foreach (string file1 in files)
            {
                actionResult += file1.Remove(0, file1.LastIndexOf(@"\")+1) + "\n";
            }
            return actionResult;
        }

        [HttpGet]
        [Route("Root/{file}")]
        public string GetRootFile(string file)
        {
            string actionResult = "";
            if (System.IO.File.Exists($"../Root/{file}"))
            {
                var imageFileStream = System.IO.File.OpenRead($"../Root/{file}");
                actionResult = System.IO.File.ReadAllText($"../Root/{file}");
            }
            else if(System.IO.Directory.Exists($"../Root/{file}"))
            {
                List<string> files = System.IO.Directory.GetFiles($"../Root/{file}").ToList();
                files.AddRange(System.IO.Directory.GetDirectories($"../Root/{file}").ToList());
                foreach(string file1 in files)
                {
                    actionResult += file1.Remove(0, file1.LastIndexOf(@"\") + 1) + "\n";
                }
            }
            else
            {
                actionResult = "No such file exists";
            }
            return actionResult;
        }

        [HttpGet]
        [Route("Root/File/{file}")]
        public IActionResult GetRootFile(string file, string prefName)
        {
            IActionResult actionResult = null;
            var stream = new FileStream($"../Root/{file}", FileMode.Open);
            actionResult = new FileStreamResult(stream, $"application/{prefName}");
            return actionResult;
        }

        [HttpGet]
        [Route("Root")]
        public string GetRootOnlyFile()
        {
            string actionResult = "";
            List<string> files = System.IO.Directory.GetFiles(@"..\Root").ToList();
            files.AddRange(System.IO.Directory.GetDirectories(@"..\Root").ToList());
            foreach (string file1 in files)
            {
                actionResult += file1.Remove(0, file1.LastIndexOf(@"\") + 1) + "\n";
            }
            return actionResult;
        }

        private List<Photo> SortPhotoInspections(List<Photo> photos)
        {
            List<Photo> photos1 = new List<Photo>();
            foreach (var photo in photos)
            {
                byte[] image = Convert.FromBase64String(photo.Base64);
                var ms = new MemoryStream(image);
                Image img = Image.FromStream(ms);
            }
            return photos1;
        }
    }
}