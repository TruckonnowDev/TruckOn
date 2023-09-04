using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;

namespace WebDispacher.Controellers
{
    public class BolInspectionController : BaseController
    {
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;

        public BolInspectionController(
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            IOrderService orderService,
            ICompanyService companyService) : base(userService)
        {
            this.orderService = orderService;
            this.companyService = companyService;
            this.truckAndTrailerService = truckAndTrailerService;
        }

        [Route("Photo/BOL/{idVech}")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetPhotoInspection(int idVech)
        {
            ViewBag.BaseUrl = Config.BaseReqvesteUrl;
            
                var isCancelSubscribe = companyService.GetCancelSubscribe(CompanyId);
                
                if (isCancelSubscribe)
                {
                    return Redirect($"{Config.BaseReqvesteUrl}/Settings/Subscription/Subscriptions");
                }
                
                //var shipping = orderService.GetShippingCurrentVehiclwIn(idVech.ToString());
                
                //var vehiclwInformation = shipping.VehiclwInformations.FirstOrDefault(v => v.Id == idVech);
                
                /*if (shipping != null)
                {
                    ViewBag.NameCompany = GetCookieCompanyName();
                    ViewData[NavConstants.TypeNavBar] = companyService.GetTypeNavBar(key, idCompany);
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                    ViewBag.Shipp = shipping;
                    ViewBag.Vehiclw = vehiclwInformation;
                    
                    return View("InspectionVech");
                }*/

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Welcome/Photo/BOL/{idVech}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetWelcomePhotoInspection(int idVech)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;
            
            /*var shipping = orderService.GetShippingCurrentVehiclwIn(idVech.ToString());
            var vehiclwInformation = shipping.VehiclwInformations.FirstOrDefault(v => v.Id == idVech);
            
            if (shipping != null)
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                ViewBag.Shipp = shipping;
                ViewBag.Vehiclw = vehiclwInformation;
                
                return View("WelcomeInspectionVech");
            }*/
            
            return null;
        }

        [Route("Doc/{idDriver}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTruckDoc(string idDriver)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;

            var truck = await truckAndTrailerService.GetTruck(idDriver);
            ViewBag.Truck = truck;
            ViewBag.TruckDoc = await truckAndTrailerService.GetTruckDoc(truck?.Id ?? 0);

            var trailer = await truckAndTrailerService.GetTrailer(idDriver);
            ViewBag.Trailer = trailer;
            ViewBag.TrailerDoc = await truckAndTrailerService.GetTrailerDocsById(trailer?.Id ?? 0);

            return View($"DocDriver");
        }

        [HttpGet]
        [Route("Doc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTruckDoc(string truckPlate, string trailerPlate)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseAllUsers;

            var truck = await truckAndTrailerService.GetTruckByPlate(truckPlate);
            ViewBag.Truck = truck;
            ViewBag.TruckDoc = await truckAndTrailerService.GetTruckDoc(truck?.Id ?? 0);

            var trailer = await truckAndTrailerService.GetTrailerByPlate(trailerPlate);
            ViewBag.Trailer = trailer;
            ViewBag.TrailerDoc = await truckAndTrailerService.GetTrailerDocsById(trailer?.Id ?? 0);
            
            return View($"DocDriver");
        }

        [HttpGet]
        [Route(".well-known/acme-challenge/{file}")]
        public string GetWellKnownFile(string file)
        {
            if (System.IO.File.Exists($"../Root/{file}"))
            {
                var imageFileStream = System.IO.File.OpenRead($"../Root/{file}");
                
                return System.IO.File.ReadAllText($"../Root/{file}");
            }

            if (!System.IO.Directory.Exists($"../Root/{file}")) return "No such file exists";
            
            var files = System.IO.Directory.GetFiles($"../Root/{file}").ToList();
            files.AddRange(System.IO.Directory.GetDirectories($"../Root/{file}").ToList());
                
            var returnString = string.Empty;
            foreach (var file1 in files)
            {
                returnString += file1.Remove(0, file1.LastIndexOf(@"\") + 1) + "\n";
            }

            return returnString;

        }

        [HttpGet]
        [Route(".well-known/acme-challenge")]
        public string GetWellKnownOnlyFile()
        {
            var stringReturn = string.Empty;
            
            var files = System.IO.Directory.GetFiles(@"..\Root").ToList();
            files.AddRange(System.IO.Directory.GetDirectories(@"..\Root").ToList());
            
            foreach (var file1 in files)
            {
                stringReturn += file1.Remove(0, file1.LastIndexOf(@"\")+1) + "\n";
            }
            
            return stringReturn;
        }

        [HttpGet]
        [Route("Root/{file}")]
        public string GetRootFile(string file)
        {
            if (System.IO.File.Exists($"../Root/{file}"))
            {
                var imageFileStream = System.IO.File.OpenRead($"../Root/{file}");
                
                return System.IO.File.ReadAllText($"../Root/{file}");
            }

            if (!System.IO.Directory.Exists($"../Root/{file}")) return "No such file exists";
            
            var files = System.IO.Directory.GetFiles($"../Root/{file}").ToList();
            files.AddRange(System.IO.Directory.GetDirectories($"../Root/{file}").ToList());
            var returnString = string.Empty;
                
            foreach(var file1 in files)
            {
                returnString += file1.Remove(0, file1.LastIndexOf(@"\") + 1) + "\n";
            }
            
            return returnString;
        }

        [HttpGet]
        [Route("Root/File/{file}")]
        public IActionResult GetRootFile(string file, string prefName)
        {
            var stream = new FileStream($"../Root/{file}", FileMode.Open);
            
            return new FileStreamResult(stream, $"application/{prefName}");
        }

        [HttpGet]
        [Route("Root")]
        public string GetRootOnlyFile()
        {
            var files = System.IO.Directory.GetFiles(@"..\Root").ToList();
            files.AddRange(System.IO.Directory.GetDirectories(@"..\Root").ToList());
            
            var returnString = string.Empty;
            
            foreach (var file1 in files)
            {
                returnString += file1.Remove(0, file1.LastIndexOf(@"\") + 1) + "\n";
            }
            
            return returnString;
        }

        private List<Photo> SortPhotoInspections(List<Photo> photos)
        {
            var photos1 = new List<Photo>();
            
            foreach (var photo in photos)
            {
                var image = Convert.FromBase64String(photo.Base64);
                var ms = new MemoryStream(image);
                
                var img = Image.FromStream(ms);
            }
            
            return photos1;
        }
    }
}