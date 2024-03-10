using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting.Internal;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Constants.Identity;
using WebDispacher.Service;
using WebDispacher.ViewModels.Equipment.Enum;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Truck;
using WebDispacher.ViewModels.Truck.Enum;
using WebDispacher.ViewModels.Widget;

namespace WebDispacher.Controellers
{
    [Route("Equipment")]
    public class EquipmentController : BaseController
    {
        private readonly ITruckAndTrailerService truckAndTrailerService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;

        public EquipmentController(
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            IOrderService orderService,
            ICompanyService companyService) : base(userService)
        {
            this.orderService = orderService;
            this.companyService = companyService;
            this.truckAndTrailerService = truckAndTrailerService;
        }

        [Route("GetTrucks")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrucksByFilters(TruckFiltersViewModel filters)
        {
            var(trucks, countEntities) = await truckAndTrailerService.GetTrucks(filters, CompanyId);

            filters.CountPages = truckAndTrailerService.GetCountTrucksPagesByCountEntites(countEntities);
            filters.AvailableGroups = await truckAndTrailerService.GetActualTruckGroupByCompanyId(CompanyId);

            return PartialView($"~/Views/Equipment/PartView/_TruckTable.cshtml", new TruckShortVmList
            {
                Items = trucks,
                Filters = filters,
            });
        }

        [Route("GetTrucksInGroupByFilters")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrucksInGroupByFilters(TruckFilterViewModel filters)
        {
            try {
                var trucks = await truckAndTrailerService.GetTrucksInGroupByFilters(filters, CompanyId);

                return PartialView($"~/Views/Equipment/PartView/_TruckTable.cshtml", (trucks.Item1, filters, trucks.Item2));
            }
            catch (Exception ex)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("GetTrailersInGroupByFilters")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailersInGroupByFilters(TrailerFilterViewModel filters)
        {
            try {
                var trucks = await truckAndTrailerService.GetTrailerInGroupByFilters(filters, CompanyId);

                return PartialView($"~/Views/Equipment/PartView/_TrailerTable.cshtml", (trucks.Item1, filters, trucks.Item2));
            }
            catch (Exception ex)
            {

            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trucks")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Trucks(TruckFiltersViewModel filters)
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

                var (trucks, countEntities) = await truckAndTrailerService.GetTrucks(filters, CompanyId);

                filters.AvailableGroups = await truckAndTrailerService.GetActualTruckGroupByCompanyId(CompanyId);

                var currentWidgets = await truckAndTrailerService.GetCurrentCompanyTruckWidgets(CompanyId);

                filters.CountPages = truckAndTrailerService.GetCountTrucksPagesByCountEntites(countEntities);

                return View($"AllTruck", new TruckShortVmList
                {
                    Items = trucks,
                    Filters = filters,
                    Widgets = currentWidgets,
                });
            }
            catch (Exception ex)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Truck/GetWidgets")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTruckWidgetsInCompany()
        {
            var listTruckStatusThemes = await truckAndTrailerService.GetCurrentCompanyTruckWidgets(CompanyId);

            return PartialView("~/Views/Equipment/PartView/_TruckWidgets.cshtml", listTruckStatusThemes);
        }
        
        [Route("Truck/GetCurrentWidgets")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetCurrentTruckWidgetsInCompany()
        {
            var listTruckStatusThemes = await truckAndTrailerService.GetCurrentCompanyTruckWidgets(CompanyId);

            return PartialView("~/Views/PartView/Widgets/_TruckWidget.cshtml", listTruckStatusThemes);
        }
        
        [Route("Trailer/GetWidgets")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailerWidgetsInCompany()
        {
            var listTrailerStatusThemes = await truckAndTrailerService.GetCurrentCompanyTrailerWidgets(CompanyId);

            return PartialView("~/Views/Equipment/PartView/_TrailerWidgets.cshtml", listTrailerStatusThemes);
        }
        
        [Route("Trailer/GetCurrentWidgets")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetCurrentTrailerWidgetsInCompany()
        {
            var listTrailerStatusThemes = await truckAndTrailerService.GetCurrentCompanyTrailerWidgets(CompanyId);

            return PartialView("~/Views/PartView/Widgets/_TrailerWidget.cshtml", listTrailerStatusThemes);
        }

        [Route("GetTrailers")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailersByFilters(TrailerFiltersViewModel filters)
        {
            var (trailers, countEntities) = await truckAndTrailerService.GetTrailers(filters, CompanyId);

            filters.CountPages = truckAndTrailerService.GetCountTrucksPagesByCountEntites(countEntities);
            filters.AvailableGroups = await truckAndTrailerService.GetActualTrailerGroupByCompanyId(CompanyId);

            return PartialView($"~/Views/Equipment/PartView/_TrailerTable.cshtml", new TrailerShortVmList
            {
                Items = trailers,
                Filters = filters,
            });
        }

        [Route("Trailers")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Trailers(TrailerFiltersViewModel filters)
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

                var (trailers, countEntities) = await truckAndTrailerService.GetTrailers(filters, CompanyId);

                filters.CountPages = truckAndTrailerService.GetCountTrailersPagesByCountEntites(countEntities);

                var currentWidgets = await truckAndTrailerService.GetCurrentCompanyTrailerWidgets(CompanyId);

                filters.AvailableGroups = await truckAndTrailerService.GetActualTrailerGroupByCompanyId(CompanyId);

                return View($"AllTrailer", new TrailerShortVmList
                {
                    Items = trailers,
                    Filters = filters,
                    Widgets = currentWidgets,
                });
            }
            catch (Exception ex)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        public IActionResult LoadVehicleImages(string vehicleName)
        {
            ViewData["VehicleName"] = vehicleName;
            return PartialView("~/Views/Equipment/VehicleImages.cshtml", ViewData);
        }

        [HttpGet]
        [Route("GetTruckImages")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTruckImages(string vehicleSlug)
        {
            if (string.IsNullOrEmpty(vehicleSlug)) return Json(new string[0]);

            var directoryPath = $"../TruckPattern/" + vehicleSlug;

            try
            {
                var imageFiles = Directory.GetFiles(directoryPath)
                    .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    .Select(Path.GetFileName);

                return Json(imageFiles);
            }
            catch(Exception ex) 
            { 
            
            }

            return Json(new string[0]);
        }
        
        [HttpGet]
        [Route("GetTrailerImages")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailerImages(string vehicleSlug)
        {
            if (string.IsNullOrEmpty(vehicleSlug)) return Json(new string[0]);

            var directoryPath = $"../TrailerPattern/" + vehicleSlug;

            try
            {
                var imageFiles = Directory.GetFiles(directoryPath)
                    .Where(file => file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    .Select(Path.GetFileName);

                return Json(imageFiles);
            }
            catch(Exception ex) 
            { 
            
            }

            return Json(new string[0]);
        }

        [Route("GetTruckSlug")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTruckSlugByName(string vehicleName)
        {
            var vehicleSlug = await truckAndTrailerService.GetTruckTypeSlugByName(vehicleName);

            return Json(vehicleSlug);
        }
        
        [Route("GetTrailerSlug")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailerSlugByName(string vehicleName)
        {
            var vehicleSlug = await truckAndTrailerService.GetTrailerTypeSlugByName(vehicleName);

            return Json(vehicleSlug);
        }

        [Route("GetTruckTypes")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTruckTypes(string categoryId)
        {
            var truckTypes = await truckAndTrailerService.GetTruckTypes(categoryId);

            return Json(truckTypes);
        }
        
        [Route("GetTrailerTypes")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailerTypes(string categoryId)
        {
            var trailerTypes = await truckAndTrailerService.GetTrailerTypes(categoryId);

            return Json(trailerTypes);
        }
        
        [Route("Truck/GetVehicleCategiries")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTruckVehicleCategiries()
        {
            var vehicleCategories = await truckAndTrailerService.GetTruckVehicleCategiries();

            return Json(vehicleCategories);
        }
        
        [Route("Trailer/GetVehicleCategiries")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailerVehicleCategiries()
        {
            var vehicleCategories = await truckAndTrailerService.GetTrailerVehicleCategiries();

            return Json(vehicleCategories);
        }

        [Route("Truck/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTruck(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTruck(id);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]        
        [Route("TruckGroup/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> RemoveTruckGroup(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var result = await truckAndTrailerService.RemoveTruckGroupByCompanyId(id);

                return result;
            }
            catch (Exception)
            {

            }

            return false;
        }
        
        [HttpGet]        
        [Route("TrailerGroup/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> RemoveTrailerGroup(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var result = await truckAndTrailerService.RemoveTrailerGroupByCompanyId(id);

                return result;
            }
            catch (Exception)
            {

            }

            return false;
        }
        
        [HttpGet]        
        [Route("TruckGroup/Rename")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> RenameTruckGroup(int groupId, string name)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var result = await truckAndTrailerService.RenameTruckGroupByCompanyId(groupId, name, CompanyId);

                return result;
            }
            catch (Exception)
            {

            }

            return false;
        }
        
        [HttpGet]        
        [Route("TrailerGroup/Rename")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> RenameTrailerGroup(int groupId, string name)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                var result = await truckAndTrailerService.RenameTrailerGroupByCompanyId(groupId, name, CompanyId);

                return result;
            }
            catch (Exception)
            {

            }

            return false;
        }

        [HttpPost]
        [Route("Truck/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> DeleteTruck(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTruck(id);

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }
        
        [HttpPost]
        [Route("Truck/RemoveWidget")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> RemoveTruckWidget(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTruckWidget(id, CompanyId);

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }
        
        [HttpPost]
        [Route("Trailer/RemoveWidget")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> RemoveTrailerWidget(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTrailerWidget(id, CompanyId);

                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        [HttpGet]
        [Route("CreateTruck")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult CreateTruck()
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
                    
                return View("CreateTruck");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("CreateTruck")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CreateTruck(TruckViewModel truck, IFormFile truckRegistrationDoc, 
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage, 
            IFormFile nYHUTDoc, string dateTimeLocal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.CreateTruck(truck, CompanyId, truckRegistrationDoc,
                        truckLeaseAgreementDoc, truckAnnualInspection, bobTailPhysicalDamage, nYHUTDoc, dateTimeLocal);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                if(truck.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = truck.VehicleCategoryId;
                }
                
                if(truck.TruckTypeId != null)
                {
                    ViewData["SelectedTruckTypeId"] = truck.TruckTypeId;
                }
                
                if(truck.TruckGroupId != 0)
                {
                    ViewData["SelectedTruckGroupId"] = truck.TruckGroupId;
                }
                
                if(truck.TruckStatusId != 0)
                {
                    ViewData["SelectedTrucStatusId"] = truck.TruckStatusId;
                }


                return View("CreateTruck", truck);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTrailer(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTrailer(id);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Trailer/Remove")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> DeleteTrailer(int id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveTrailer(id);

                return true;
            }
            catch (Exception)
            {

            }
            
            return false;
        }
        
        [HttpGet]
        [Route("Truck/IsVisibleLocation")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> IsVisibleTruckLocation(LocationType selectedLocationType)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                return await truckAndTrailerService.IsVisibleTruckLocation(selectedLocationType);
            }
            catch (Exception)
            {

            }
            
            return false;
        }
        
        [HttpGet]
        [Route("Trailer/IsVisibleLocation")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> IsVisibleTrailerLocation(LocationType selectedLocationType)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                return await truckAndTrailerService.IsVisibleTrailerLocation(selectedLocationType);
            }
            catch (Exception)
            {

            }
            
            return false;
        }

        [HttpGet]
        [Route("CreateTrailer")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult CreateTrailer()
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
                    
                return View("CreateTraler");
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Truck/IsHaveTruckGroup")]
        public async Task<JsonResult> IsHaveTruckGroup()
        {
            var canProceed = await truckAndTrailerService.CheckTruckHaveGroup(CompanyId);

            return Json(new { canProceed });
        }
        
        [HttpPost]
        [Route("Trailer/IsHaveTrailerGroup")]
        public async Task<JsonResult> IsHaveTrailerGroup()
        {
            var canProceed = await truckAndTrailerService.CheckTrailerHaveGroup(CompanyId);

            return Json(new { canProceed });
        }
        
        [HttpGet]
        [Route("Truck/GetTruckGroupsDropdownItems")]
        public async Task<JsonResult> GetTruckGroupsDropdownItems()
        {
            var truckGroups = await truckAndTrailerService.GetTruckGroupsDropdownItems(CompanyId);

            return Json(truckGroups);
        }
        
        [HttpGet]
        [Route("Truck/GetTruckStatusDropdownItems")]
        public async Task<JsonResult> GetTruckStatusDropdownItems()
        {
            var truckStatuses = await truckAndTrailerService.GetTruckStatusDropdownItems(CompanyId);

            return Json(truckStatuses);
        }
        
        [HttpGet]
        [Route("Truck/GetStatusWithoutWidgetsDropdownItems")]
        public async Task<JsonResult> GetTruckStatusWithoutWidgetsDropdownItems()
        {
            var truckStatuses = await truckAndTrailerService.GetTruckStatusWithoutWidgetsDropdownItems(CompanyId);

            return Json(truckStatuses);
        }
        
        [HttpGet]
        [Route("Trailer/GetStatusWithoutWidgetsDropdownItems")]
        public async Task<JsonResult> GetTrailerStatusWithoutWidgetsDropdownItems()
        {
            var truckStatuses = await truckAndTrailerService.GetTrailerStatusWithoutWidgetsDropdownItems(CompanyId);

            return Json(truckStatuses);
        }
        
        [HttpGet]
        [Route("Truck/GetEditStatusWithoutWidgetsDropdownItems")]
        public async Task<JsonResult> GetEditTruckStatusWithoutWidgetsDropdownItems(int currentItemId)
        {
            var truckStatuses = await truckAndTrailerService.GetTruckStatusWithoutWidgetsWithCurrentDropdownItems(currentItemId,CompanyId);

            return Json(truckStatuses);
        }
        
        [HttpGet]
        [Route("Trailer/GetEditStatusWithoutWidgetsDropdownItems")]
        public async Task<JsonResult> GetEditTrailerStatusWithoutWidgetsDropdownItems(int currentItemId)
        {
            var trailerStatuses = await truckAndTrailerService.GetTrailerStatusWithoutWidgetsWithCurrentDropdownItems(currentItemId,CompanyId);

            return Json(trailerStatuses);
        }
        
        [HttpGet]
        [Route("Trailer/GetTrailerStatusDropdownItems")]
        public async Task<JsonResult> GetTrailerStatusDropdownItems()
        {
            var trailerGroups = await truckAndTrailerService.GetTrailerStatusDropdownItems(CompanyId);

            return Json(trailerGroups);
        }
        
        [HttpGet]
        [Route("Trailer/GetTrailerGroupsDropdownItems")]
        public async Task<JsonResult> GetTrailerGroupsDropdownItems()
        {
            var trailerGroups = await truckAndTrailerService.GetTrailerGroupsDropdownItems(CompanyId);

            return Json(trailerGroups);
        }

        [HttpPost]
        [Route("SaveTruckStatus")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> SaveTruckStatus(int truckId, int truckStatusId)
        {
            if (truckId != 0 && truckStatusId != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    return await truckAndTrailerService.SaveTruckStatus(truckId, truckStatusId, CompanyId);
                }
                catch (Exception e)
                {

                }
            }

            return false;
        }
        
        [HttpPost]
        [Route("SaveTruckLocation")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> SaveTruckLocation(int truckId, string truckLocation)
        {
            if (truckId != 0 && !string.IsNullOrEmpty(truckLocation))
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    return await truckAndTrailerService.SaveTruckLocation(truckId, truckLocation, CompanyId);
                }
                catch (Exception e)
                {

                }
            }

            return false;
        }
        
        [HttpPost]
        [Route("SaveTrailerLocation")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> SaveTrailerLocation(int trailerId, string trailerLocation)
        {
            if (trailerId != 0 && !string.IsNullOrEmpty(trailerLocation))
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    return await truckAndTrailerService.SaveTrailerLocation(trailerId, trailerLocation, CompanyId);
                }
                catch (Exception e)
                {

                }
            }

            return false;
        }
        
        [HttpPost]
        [Route("SaveTrailerStatus")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<bool> SaveTrailerStatus(int trailerId, int trailerStatusId)
        {
            if (trailerId != 0 && trailerStatusId != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    return await truckAndTrailerService.SaveTrailerStatus(trailerId, trailerStatusId, CompanyId);
                }
                catch (Exception e)
                {

                }
            }

            return false;
        }
        
        [HttpPost]
        [Route("Truck/SaveGroup")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveTruckGroup(string name, string dateTimeLocalTruck, string returnUrl)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.AddTruckGroupInCompany(name, dateTimeLocalTruck, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {

                }
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Trailer/SaveGroup")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveTrailerGroup(string name, string dateTimeLocalTrailer, string returnUrl)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.AddTrailerGroupInCompany(name, dateTimeLocalTrailer, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {

                }
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [HttpPost]
        [Route("Truck/SaveWidget")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveTruckWidget(CreateWidgetVm model, string dateTimeLocalCreateTruck, string returnUrl = "none")
        {
            model.TypeWidget = ViewModels.Widget.Enum.TypeWidget.Truck;
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.AddTruckWidgetInCompany(model, dateTimeLocalCreateTruck, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {

                    return PartialView("~/Views/PartView/Modals/CreateWidget.cshtml", (model, returnUrl));
                }
            }

            return PartialView("~/Views/PartView/Modals/CreateWidget.cshtml", (model, returnUrl));
        }
        
        [HttpPost]
        [Route("Trailer/SaveWidget")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveTrailerkWidget(CreateWidgetVm model, string dateTimeLocalCreateTruck, string returnUrl = "none")
        {
            model.TypeWidget = ViewModels.Widget.Enum.TypeWidget.Trailer;
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.AddTrailerWidgetInCompany(model, dateTimeLocalCreateTruck, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {
                    return PartialView("~/Views/PartView/Modals/CreateWidget.cshtml", (model, returnUrl));
                }
            }

            return PartialView("~/Views/PartView/Modals/CreateWidget.cshtml", (model, returnUrl));
        }
        
        [HttpPost]
        [Route("Truck/UpdateWidget")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> UpdateTruckWidget(WidgetViewModel model, string dateTimeLocalCreateTruck, string returnUrl = "none")
        {
            model.TypeWidget = ViewModels.Widget.Enum.TypeWidget.Truck;
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.UpdateTruckWidgetInCompany(model, dateTimeLocalCreateTruck, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {
                    return PartialView("~/Views/PartView/Modals/EditWidget.cshtml", (model, returnUrl));
                }
            }

            return PartialView("~/Views/PartView/Modals/EditWidget.cshtml", (model, returnUrl));
        }
        
        [HttpPost]
        [Route("Trailer/UpdateWidget")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> UpdateTrailerWidget(WidgetViewModel model, string dateTimeLocalCreateTruck, string returnUrl = "none")
        {
            model.TypeWidget = ViewModels.Widget.Enum.TypeWidget.Trailer;
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.UpdateTrailerWidgetInCompany(model, dateTimeLocalCreateTruck, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {
                    return PartialView("~/Views/PartView/Modals/EditWidget.cshtml", (model, returnUrl));
                }
            }

            return PartialView("~/Views/PartView/Modals/EditWidget.cshtml", (model, returnUrl));
        }
        
        [HttpPost]
        [Route("Truck/SaveCustomStatus")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveCustomTruckStatus(CreateTruckStatusVm model, string dateTimeLocalCreateTruck, string returnUrl = "none")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.AddCustomTruckStatusInCompany(model, dateTimeLocalCreateTruck, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {
                    model.TruckStatusThemes = await truckAndTrailerService.GetAvailableTruckStatusThemes();

                    return PartialView("~/Views/PartView/Modals/Equipment/CreateTruckStatus.cshtml", (model, returnUrl));
                }
            }

            model.TruckStatusThemes = await truckAndTrailerService.GetAvailableTruckStatusThemes();

            return PartialView("~/Views/PartView/Modals/Equipment/CreateTruckStatus.cshtml", (model, returnUrl));
        }
        
        [HttpPost]
        [Route("Trailer/SaveCustomStatus")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveCustomTrailerStatus(CreateTrailerStatusVm model, string dateTimeLocalCreateTrailer, string returnUrl = "none")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.AddCustomTrailerStatusInCompany(model, dateTimeLocalCreateTrailer, CompanyId);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                }
                catch (Exception e)
                {
                    model.TrailerStatusThemes = await truckAndTrailerService.GetAvailableTrailerStatusThemes();

                    return PartialView("~/Views/PartView/Modals/Equipment/CreateTrailerStatus.cshtml", (model, returnUrl));
                }
            }

            model.TrailerStatusThemes = await truckAndTrailerService.GetAvailableTrailerStatusThemes();

            return PartialView("~/Views/PartView/Modals/Equipment/CreateTrailerStatus.cshtml", (model, returnUrl));
        }

        [HttpGet]
        [Route("Truck/CreateTruckStatusForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetCreateTruckStatusForm(string returnUrl)
        {
            var listTruckStatusThemes = await truckAndTrailerService.GetAvailableTruckStatusThemes();

            return PartialView("~/Views/PartView/Modals/Equipment/CreateTruckStatus.cshtml", (new CreateTruckStatusVm { TruckStatusThemes = listTruckStatusThemes}, returnUrl));
        }
        
        
        [HttpGet]
        [Route("Truck/CreateWidgetForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetCreateWidgetForm(string returnUrl)
        {
            return PartialView("~/Views/PartView/Modals/CreateWidget.cshtml", (new CreateWidgetVm { TypeWidget = ViewModels.Widget.Enum.TypeWidget.Truck }, returnUrl));
        }
        
        [HttpGet]
        [Route("Trailer/CreateWidgetForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailerCreateWidgetForm(string returnUrl)
        {
            return PartialView("~/Views/PartView/Modals/CreateWidget.cshtml", (new CreateWidgetVm { TypeWidget = ViewModels.Widget.Enum.TypeWidget.Trailer }, returnUrl));
        }
        
        [HttpGet]
        [Route("Truck/EditWidgetForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetWidgetForm(int id, string returnUrl)
        {
            var widget = await truckAndTrailerService.GetTruckWidgetById(id, CompanyId);

            return PartialView("~/Views/PartView/Modals/EditWidget.cshtml", (widget, returnUrl));
        }
        
        [HttpGet]
        [Route("GetUploadExpDocForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult GetUploadExpDocForm(string modelId)
        {
            ViewData["ModalId"] = modelId;
            return PartialView("~/Views/PartView/Modals/Equipment/UpdateExpWithDoc.cshtml");
        }
        
        [HttpGet]
        [Route("Trailer/EditWidgetForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetTrailerWidgetForm(int id, string returnUrl)
        {
            var widget = await truckAndTrailerService.GetTrailerWidgetById(id, CompanyId);

            return PartialView("~/Views/PartView/Modals/EditWidget.cshtml", (widget, returnUrl));
        }
        
        [HttpGet]
        [Route("Trailer/CreateTrailerStatusForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> GetCreateTrailerStatusForm(string returnUrl)
        {
            var listTrailerStatusThemes = await truckAndTrailerService.GetAvailableTrailerStatusThemes();

            return PartialView("~/Views/PartView/Modals/Equipment/CreateTrailerStatus.cshtml", (new CreateTrailerStatusVm { TrailerStatusThemes = listTrailerStatusThemes }, returnUrl));
        }
        
        [HttpGet]
        [Route("Truck/CreateTruckGroupForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult GetCreateTruckGroupForm(string returnUrl)
        {
            return PartialView("~/Views/PartView/Modals/Equipment/RequiredCreateTruckGroup.cshtml", (new TruckGroup(), returnUrl));
        }
        
        [HttpGet]
        [Route("Trailer/CreateTrailerGroupForm")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult GetCreateTrailerGroupForm(string returnUrl)
        {
            return PartialView("~/Views/PartView/Modals/Equipment/RequiredCreateTrailerGroup.cshtml", (new TrailerGroup(), returnUrl));
        }

        [HttpPost]
        [Route("CreateTrailer")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> CreateTrailer(TrailerViewModel trailer,
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc, string dateTimeLocal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   /* if (DateTime.TryParseExact(trailer.Exp, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {

                    }*/

                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.CreateTrailer(trailer, CompanyId, trailerRegistrationDoc,
                        trailerAnnualInspectionDoc, leaseAgreementDoc, dateTimeLocal);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                if (trailer.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = trailer.VehicleCategoryId;
                }

                if (trailer.TrailerTypeId != null)
                {
                    ViewData["SelectedTrailerTypeId"] = trailer.TrailerTypeId;
                }

                if (trailer.TrailerGroupId != 0)
                {
                    ViewData["SelectedTrailerGroupId"] = trailer.TrailerGroupId;
                }

                if (trailer.TrailerStatusId != null)
                {
                    ViewData["SelectedTrailerStatusId"] = trailer.TrailerStatusId;
                }


                return View("CreateTraler", trailer);
            }


            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditTruck")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditTruck(int truckId)
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
                    
                var truck = await truckAndTrailerService.GetTruckById(truckId);

                ViewBag.TruckDocs = await truckAndTrailerService.GetBaseTruckDoc(truckId.ToString());

                if (truck.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = truck.VehicleCategoryId;
                }

                if (truck.TruckTypeId != null)
                {
                    ViewData["SelectedTruckTypeId"] = truck.TruckTypeId;
                }
                
                if (truck.TruckGroupId != 0)
                {
                    ViewData["SelectedTruckGroupId"] = truck.TruckGroupId;
                }

                if (truck.TruckStatusId != 0)
                {
                    ViewData["SelectedTrucStatusId"] = truck.TruckStatusId;
                }

                return View(truck);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("Truck/UpdateDoc")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateTruckDocuments(int entryId, DateTime docExp, IFormFile updateDocument, TypeChangeDocument typeChangeDocument)
        {
            try
            {
                switch (typeChangeDocument)
                {
                    case TypeChangeDocument.AnnualInspection:
                        await truckAndTrailerService.UpdateTruckAnnualInspectionWithDoc(entryId, docExp, updateDocument);
                        break;
                    case TypeChangeDocument.Plate:
                        await truckAndTrailerService.UpdateTruckPlateWithDoc(entryId, docExp, updateDocument);
                        break;
                    default: return NotFound();
                }
            }
            catch(Exception e) 
            {
                return NotFound();
            }

            return Ok();
        }
        
        [HttpPost]
        [Route("Trailer/UpdateDoc")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateTrailerDocuments(int entryId, DateTime docExp, IFormFile updateDocument, TypeChangeDocument typeChangeDocument)
        {
            try
            {
                switch (typeChangeDocument)
                {
                    case TypeChangeDocument.AnnualInspection:
                        await truckAndTrailerService.UpdateTrailerAnnualInspectionWithDoc(entryId, docExp, updateDocument);
                        break;
                    case TypeChangeDocument.Plate:
                        await truckAndTrailerService.UpdateTrailerPlateWithDoc(entryId, docExp, updateDocument);
                        break;
                    default: return NotFound();
                }
            }
            catch(Exception e) 
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [Route("EditTruck")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditTruck(TruckViewModel truck, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string localDate)
        {
            if (ModelState.IsValid && truck.Id != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.EditTruck(truck, truckRegistrationDoc,
                        truckLeaseAgreementDoc, truckAnnualInspection, bobTailPhysicalDamage, nYHUTDoc, CompanyId, localDate);


                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                if (truck.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = truck.VehicleCategoryId;
                }

                if (truck.TruckTypeId != null)
                {
                    ViewData["SelectedTruckTypeId"] = truck.TruckTypeId;
                }

                if (truck.TruckGroupId != 0)
                {
                    ViewData["SelectedTruckGroupId"] = truck.TruckGroupId;
                }

                if (truck.TruckStatusId != 0)
                {
                    ViewData["SelectedTrucStatusId"] = truck.TruckStatusId;
                }

                ViewBag.TruckDocs = await truckAndTrailerService.GetBaseTruckDoc(truck.Id.ToString());
                return View("EditTruck", truck);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpGet]
        [Route("EditTrailer")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public IActionResult EditTrailer(int trailerId)
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
                    
                var model = truckAndTrailerService.GetTrailerById(trailerId);

                if (model.VehicleCategoryId != null)
                {
                    ViewData["SelectedVehicleCategoryId"] = model.VehicleCategoryId;
                }

                if (model.TrailerTypeId != null)
                {
                    ViewData["SelectedTrailerTypeId"] = model.TrailerTypeId;
                }

                if (model.TrailerGroupId != 0)
                {
                    ViewData["SelectedTrailerGroupId"] = model.TrailerGroupId;
                }

                if (model.TrailerStatusId != null)
                {
                    ViewData["SelectedTrailerStatusId"] = model.TrailerStatusId;
                }

                return View(model);
            }
            catch (Exception)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost] 
        [Route("EditTrailer")]
        [DisableRequestSizeLimit]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> EditTrailer(TrailerViewModel trailer, string localDate)
        {
            if (ModelState.IsValid && trailer.Id != 0)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.EditTrailer(trailer, CompanyId, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/trailers");
                }
                catch (Exception)
                {

                }
            }
            else
            {
                if (trailer.TrailerGroupId != 0)
                {
                    ViewData["SelectedTrailerGroupId"] = trailer.TrailerGroupId;
                }

                if (trailer.TrailerStatusId != null)
                {
                    ViewData["SelectedTrailerStatusId"] = trailer.TrailerStatusId;
                }

                return View(trailer);
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [HttpPost]
        [Route("SaveFile")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public string AddFile(IFormFile uploadedFile, string id)
        {
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                if (uploadedFile != null)
                {
                    var path = $"../Document/Truck/{id}/" + uploadedFile.FileName;
                    Directory.CreateDirectory($"../Document/Truck/{id}");
                    orderService.SavePath(id, path);
                        
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        uploadedFile.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception e)
            {
                return DocAndFileConstants.False;
            }
            
            return DocAndFileConstants.True;
        }

        [HttpGet]
        [Route("Document")]
        public async Task<IActionResult> Get(string id)
        {
            FileStream stream = null;
            try
            {
                var docPath = await orderService.GetDocument(id);
                
                stream = new FileStream(docPath, FileMode.Open);
            }
            catch (Exception)
            {
                stream = null;
            }
            
            return new FileStreamResult(stream, DocAndFileConstants.ContentTypePdf);
        }

        [Route("Truck/Doc")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTruckDoc(int id)
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

                    var truckDocs = await truckAndTrailerService.GetTruckDoc(id);

                    ViewBag.TruckId = id;
                    
                    return View("DocTruck", truckDocs);
                }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/Doc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> GoToViewTraileDoc(int id)
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
                var trailerDocs = await truckAndTrailerService.GetTrailerDocsById(id);
                ViewBag.TrailerId = id;
                    
                return View("DocTrailer", trailerDocs);
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Truck/SaveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> SaveDocTruck(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTruck(uploadedFile, nameDoc, id, localDate);
                        
                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks");
            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Trailer/DocSaveById")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> Trailer(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTrailer(uploadedFile, nameDoc, id, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Truck/DocSaveById")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public async Task<IActionResult> DocSaveById(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTruck(uploadedFile, nameDoc, id, localDate);

                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc/?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trucks/");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/SaveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> SaveDocTrailer(IFormFile uploadedFile, string nameDoc, int id, string localDate)
        {
            if (!string.IsNullOrEmpty(nameDoc) && uploadedFile != null)
            {
                try
                {
                    ViewBag.BaseUrl = Config.BaseReqvesteUrl;

                    await truckAndTrailerService.SaveDocTrailer(uploadedFile, nameDoc, id, localDate);
                        
                    return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
                }
                catch (Exception e)
                {

                }
            }
            else
            {
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailers");
            }

            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("Trailer/RemoveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTrailerDoc(int docId, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveDocTrailer(docId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Trailer/Doc?id={id}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }
        
        [Route("Truck/RemoveDoc")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        public async Task<IActionResult> RemoveTruckDoc(int docId, string id)
        {
            ViewData[NavConstants.TypeNavBar] = NavConstants.BaseCompany;
            try
            {
                ViewBag.BaseUrl = Config.BaseReqvesteUrl;
                
                await truckAndTrailerService.RemoveDocTruck(docId);
                    
                return Redirect($"{Config.BaseReqvesteUrl}/Equipment/Truck/Doc?id={id}");
            }
            catch (Exception e)
            {

            }
            
            return Redirect(Config.BaseReqvesteUrl);
        }

        [Route("GetDock")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDock(string docPath, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, type);
        }

        [Route("GetDockPDF")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetDockPDF(string docPath)
        {
            var imageFileStream = System.IO.File.OpenRead(docPath);
            
            return File(imageFileStream, DocAndFileConstants.ContentTypePdf);
        }

        [HttpGet]
        [Route("Image")]
        [Authorize(Policy = PolicyIdentityConstants.CarrierCompany)]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        public IActionResult GetShiping(string name, string type)
        {
            var imageFileStream = System.IO.File.OpenRead(name);
            
            return File(imageFileStream, $"image/{type}");
        }
    }
}