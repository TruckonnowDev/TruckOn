using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using DaoModels.DAO.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using xNet;
//using xNet;

namespace WebDispacher.Service.TransportationManager
{
    public class GetDataCentralDispatch : ITransportationDispatch
    {
        private List<string> proxyCloction = null;
        public string Tokene { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        private HttpRequest httpRequest = null;
        private CookieDictionary cooks;
        HtmlParser htmlParser = new HtmlParser();


        public GetDataCentralDispatch()
        {
            proxyCloction = new List<string>()
            {
                "137.74.64.246:1082",
            };
        }

        private void Init(bool isProxt)
        {
            if (httpRequest != null)
            {
                httpRequest.Close();
                httpRequest.ClearAllHeaders();
            }
            httpRequest = new HttpRequest()
            {
                Cookies = new CookieDictionary(),
                AllowAutoRedirect = true,
            };
            if (isProxt)
            {
                ProxyClient proxyClient = ProxyClient.Parse(ProxyType.Http, proxyCloction[new Random().Next(0, proxyCloction.Count)]);
                httpRequest.Proxy = proxyClient;
            }
        }

        private string Avthorization(bool isProxt, Dispatcher dispatcher)
        {
            string response = "";
            try
            {
                Init(isProxt);
                var htm = httpRequest.Get("https://www.centraldispatch.com/login?uri=%2Fprotected%2F").ToString();
                Tokene = httpRequest.Cookies.GetValueOrDefault("CSRF_TOKEN", ""); //Regex.Match(htm, @"CSRFToken.{4}lue\W\W(\w+)").Groups[1].Value;
                //httpRequest.AddParam("Username", "ATS2019");
                //httpRequest.AddParam("Password", "Dispatch4u2020@");
                httpRequest.AddParam("Username", dispatcher.Login);
                httpRequest.AddParam("Password", dispatcher.Password);
                httpRequest.AddParam("r", "");
                httpRequest.AddParam("CSRFToken", Tokene);
                var res = httpRequest.Post("https://www.centraldispatch.com/login?uri=/protected/");
                cooks = httpRequest.Cookies;
                if (res.Cookies.Count >= 5)
                {
                    response = "Yes";   
                }
                else
                {
                    response = "No";
                }
            }
            catch(Exception e)
            {
                response = "No";
            }
            return response;
        }

        private void GetToken()
        {
            while(true)
            {
                var s = httpRequest.EnumerateHeaders().Current;
                
                //if (httpRequest.EnumerateHeaders().Current.Value == null)
                //{
                //    break;
                //}
                //else
                if(httpRequest.EnumerateHeaders().Current.Key != "CSRF_TOKEN")
                {

                }
                else
                {
                    httpRequest.EnumerateHeaders().MoveNext();
                }
                      
            }
        }

        public async Task<Shipping> GetShipping(string urlPage, Dispatcher dispatcher)
        {
            Shipping shipping = null;
            string status = Avthorization(false, dispatcher);
            if(status == "Yes")
            {
                string sourseUrl = httpRequest.Get(urlPage).ToString();
                shipping = await ParseDataInUrl(sourseUrl, sourseUrl);
            }
            return shipping;
        }

        private async Task<Shipping> ParseDataInUrl(string sourse, string sourseUrl2)
        {
            Shipping shipping = new Shipping();
            shipping.UrlReqvest = sourseUrl2;
            SetHeadInform(sourse, ref shipping);
            SetOrderInform(sourse, ref shipping);
            SetContactInform(sourse, ref shipping);
            SetPickupInform(sourse, ref shipping);
            SetDeliveryInform(sourse, ref shipping);
            SetDispatchInform(sourse, ref shipping);
            SetVehicleInform(sourse, ref shipping);
            return shipping;
        }

        private void SetHeadInform(string sourse, ref Shipping shipping)
        {
            try
            {
                IHtmlDocument htmlDocument = htmlParser.Parse(sourse);
                var element = htmlDocument.GetElementsByClassName("col-xs-12 col-sm-7 col-md-8")[0]
                    .GetElementsByTagName("p");
                shipping.Id = element[0].TextContent.Remove(0, element[0].TextContent.IndexOf(": ") + 2);
                shipping.idOrder = element[0].TextContent.Remove(0, element[0].TextContent.IndexOf(": ") + 2);
                shipping.CurrentStatus = "NewLoad"; //element[1].TextContent.Remove(0, element[1].TextContent.IndexOf(": ") + 2);
                shipping.LastUpdated = element[2].TextContent.Remove(0, element[2].TextContent.IndexOf(": ") + 2);
                shipping.CDReference = element[3].TextContent.Remove(0, element[3].TextContent.IndexOf(": ") + 2);
            }
            catch (Exception)
            {
            }
        }

        private void SetOrderInform(string sourse, ref Shipping shipping)
        {
            try
            {
                IHtmlDocument htmlDocument = htmlParser.Parse(sourse);
                var element1 = htmlDocument.GetElementById("sheetDetails")
                    .GetElementsByClassName("panel panel-default")[1]
                    .GetElementsByClassName("panel-body")[0];
                var element = element1.GetElementsByClassName("col-xs-12 col-sm-6");
                var el = element[0].GetElementsByTagName("p");
                shipping.DispatchDate = el[0].TextContent.Remove(0, el[0].TextContent.IndexOf("Dispatch Date: ") + "Dispatch Date: ".Length);
                shipping.DispatchDate = shipping.DispatchDate.Remove(shipping.DispatchDate.IndexOf("\n"));
                shipping.PickupExactly = el[0].TextContent.Remove(0, el[0].TextContent.IndexOf("Pickup Exactly: ") + "Pickup Exactly: ".Length);
                shipping.PickupExactly = shipping.PickupExactly.Remove(shipping.PickupExactly.IndexOf("\n")).Trim();
                if (shipping.PickupExactly.IndexOf("Dispatch Date: ") != -1)
                {
                    shipping.PickupExactly = shipping.PickupExactly.Replace("Dispatch Date: ", "");
                }
                shipping.DeliveryEstimated = el[0].TextContent.Remove(0, el[0].TextContent.IndexOf("Delivery Estimated: ") + "Delivery Estimated: ".Length);
                shipping.DeliveryEstimated = shipping.DeliveryEstimated.Remove(shipping.DeliveryEstimated.IndexOf("\n")).Trim();
                if (shipping.DeliveryEstimated.IndexOf("Dispatch Date: ") != -1)
                {
                    shipping.DeliveryEstimated = shipping.DeliveryEstimated.Replace("Dispatch Date: ", "");
                }
                shipping.ShipVia = el[1].TextContent.Remove(0, el[1].TextContent.IndexOf(": ") + 2);
                shipping.Condition = el[2].TextContent.Remove(0, el[2].TextContent.IndexOf(": ") + 2);
                shipping.PriceListed = element[1].TextContent.Remove(0, element[1].TextContent.IndexOf("Total Payment to Carrier:") + "Total Payment to Carrier: ".Length);
                shipping.PriceListed = shipping.PriceListed.Remove(shipping.PriceListed.IndexOf("\n"));
                shipping.TotalPaymentToCarrier = element[1].TextContent.Remove(0, element[1].TextContent.IndexOf("On Delivery") + "On Delivery".Length).Trim();
                shipping.TotalPaymentToCarrier = shipping.TotalPaymentToCarrier.Remove(0, shipping.TotalPaymentToCarrier.IndexOf("to Carrier:") + "to Carrier:".Length).Trim();
                shipping.TotalPaymentToCarrier = shipping.TotalPaymentToCarrier.Remove(shipping.TotalPaymentToCarrier.IndexOf("\n"));
                shipping.OnDeliveryToCarrier = element1.TextContent.Remove(0, element1.TextContent.IndexOf("Company* owes Carrier:") + "Company* owes Carrier:".Length);
                shipping.OnDeliveryToCarrier = shipping.OnDeliveryToCarrier.Remove(0, shipping.OnDeliveryToCarrier.IndexOf(shipping.PriceListed) + shipping.PriceListed.Length).Trim().Replace("\n", "");
                while (shipping.OnDeliveryToCarrier.Contains("  ")) { shipping.OnDeliveryToCarrier = shipping.OnDeliveryToCarrier.Replace("  ", " "); }
                if (shipping.TotalPaymentToCarrier != "None")
                {
                    shipping.TotalPaymentToCarrier = shipping.TotalPaymentToCarrier.Remove(0, shipping.TotalPaymentToCarrier.IndexOf('*') + 1);
                }
                else
                {
                    shipping.TotalPaymentToCarrier = shipping.OnDeliveryToCarrier.Remove(0, shipping.OnDeliveryToCarrier.IndexOf("within") + "within".Length).Trim();
                    shipping.TotalPaymentToCarrier = shipping.TotalPaymentToCarrier.Remove(shipping.TotalPaymentToCarrier.IndexOf(" ")) + " days";
                }
                //shipping.CompanyOwesCarrier = element[1].TextContent.Remove(0, element[1].TextContent.IndexOf("Company") + "Company** owes Carrier:\n".Length);
                //shipping.CompanyOwesCarrier = shipping.CompanyOwesCarrier.Remove(0, shipping.CompanyOwesCarrier.IndexOf("\n")).TrimStart();
                //shipping.CompanyOwesCarrier = shipping.CompanyOwesCarrier.Remove(shipping.CompanyOwesCarrier.IndexOf("\n"));
            }
            catch (Exception)
            {
            }
        }

        private void SetContactInform(string sourse, ref Shipping shipping)
        {
            try
            {
                IHtmlDocument htmlDocument = htmlParser.Parse(sourse);
                var element = htmlDocument.GetElementById("sheetDetails")
                    .GetElementsByClassName("col-xs-12 col-sm-6 col-md-4")[1]
                    .GetElementsByClassName("panel panel-default")[0]
                    .GetElementsByClassName("panel-body")[0];
                shipping.ContactC = element.TextContent.Remove(0, element.TextContent.IndexOf("Contact: ") + "Contact: ".Length);
                shipping.ContactC = shipping.ContactC.Remove(shipping.ContactC.IndexOf("\n")).Trim();
                shipping.PhoneC = element.TextContent.Remove(0, element.TextContent.IndexOf("Phone: ") + "Phone: ".Length);
                shipping.PhoneC = shipping.PhoneC.Remove(shipping.PhoneC.IndexOf(" ")).Trim();
                shipping.FaxC = element.TextContent.Remove(0, element.TextContent.IndexOf("Fax: ") + "Fax: ".Length);
                shipping.FaxC = shipping.FaxC.Remove(shipping.FaxC.IndexOf("I")).Trim();
                shipping.IccmcC = element.TextContent.Remove(0, element.TextContent.IndexOf("ICCMC: ") + "ICCMC: ".Length);
                shipping.IccmcC = shipping.IccmcC.Trim();
            }
            catch (Exception)
            {
            }
        }

        private void SetPickupInform(string sourse, ref Shipping shipping)
        {
            try
            {
                IHtmlDocument htmlDocument = htmlParser.Parse(sourse);
                var element = htmlDocument.GetElementById("sheetBottom")
                    .GetElementsByClassName("col-xs-12 col-sm-6 col-md-4")[0]
                    .GetElementsByClassName("panel panel-default")[0]
                    .GetElementsByClassName("panel-body")[0].Children;
                shipping.ContactNameP = element[0].TextContent.Trim();
                shipping.ContactNameP = shipping.ContactNameP.Remove(shipping.ContactNameP.IndexOf("\n"));
                shipping.AddresP = element[0].TextContent.Trim();
                shipping.AddresP = shipping.AddresP.Remove(0, shipping.ContactNameP.Length).Trim();
                if (shipping.AddresP[0] == '(')
                {
                    shipping.AddresP = shipping.AddresP.Remove(0, shipping.AddresP.IndexOf(')') + 2).Trim();
                }
                shipping.AddresP = shipping.AddresP.Remove(shipping.AddresP.IndexOf("\n")).Trim();
                shipping.CityP = element[0].TextContent.Remove(0, element[0].TextContent.IndexOf(shipping.AddresP) + shipping.AddresP.Length).Trim();
                shipping.CityP = shipping.CityP.Remove(shipping.CityP.IndexOf(',')).Trim();
                if (shipping.CityP.IndexOf(shipping.AddresP) != -1)
                {
                    shipping.CityP = shipping.CityP.Replace(shipping.AddresP, "").Trim();
                }
                shipping.StateP = element[0].TextContent.Remove(0, element[0].TextContent.IndexOf(shipping.CityP) + shipping.CityP.Length + 2).Trim();
                shipping.StateP = shipping.StateP.Remove(2);
                shipping.ZipP = element[0].TextContent.Remove(0, element[0].TextContent.LastIndexOf(shipping.StateP) + 2).Trim();
                shipping.PhoneP = element[element.Length - 1].TextContent;
            }
            catch (Exception)
            {
            }
        }

        private void SetDeliveryInform(string sourse, ref Shipping shipping)
        {
            try
            {
                IHtmlDocument htmlDocument = htmlParser.Parse(sourse);
                var element = htmlDocument.GetElementById("sheetBottom")
                    .GetElementsByClassName("col-xs-12 col-sm-6 col-md-4")[1]
                    .GetElementsByClassName("panel panel-default")[0]
                    .GetElementsByClassName("panel-body")[0].Children;
                shipping.ContactNameD = element[0].TextContent.Trim();
                shipping.ContactNameD = shipping.ContactNameD.Remove(shipping.ContactNameD.IndexOf("\n"));
                shipping.AddresD = element[0].TextContent.Trim();
                shipping.AddresD = shipping.AddresD.Remove(0, shipping.ContactNameD.Length).Trim();
                if (shipping.AddresD[0] == '(')
                {
                    shipping.AddresD = shipping.AddresD.Remove(0, shipping.AddresD.IndexOf(')') + 2).Trim();
                }
                shipping.AddresD = shipping.AddresD.Remove(shipping.AddresD.IndexOf("\n")).Trim();
                shipping.CityD = element[0].TextContent.Remove(0, element[0].TextContent.IndexOf(shipping.AddresD) + shipping.AddresD.Length).Trim();
                shipping.CityD = shipping.CityD.Remove(shipping.CityD.IndexOf(',')).Trim();
                if (shipping.CityD.IndexOf(shipping.AddresD) != -1)
                {
                    shipping.CityD = shipping.CityD.Replace(shipping.AddresD, "").Trim();
                }
                shipping.StateD = element[0].TextContent.Remove(0, element[0].TextContent.IndexOf(shipping.CityD) + shipping.CityD.Length + 2).Trim();
                shipping.StateD = shipping.StateD.Remove(2);
                shipping.ZipD = element[0].TextContent.Remove(0, element[0].TextContent.LastIndexOf(shipping.StateD) + 2).Trim();
                shipping.PhoneD = element[element.Length - 1].TextContent;
            }
            catch (Exception e)
            {
            }
        }

        private void SetVehicleInform(string sourse, ref Shipping shipping)
        {
            try
            {
                shipping.VehiclwInformations = new List<VehiclwInformation>();
                VehiclwInformation vehiclwInformation = new VehiclwInformation();
                IHtmlDocument htmlDocument = htmlParser.Parse(sourse);
                var vehicles = htmlDocument.GetElementById("sheetBottom")
                    .GetElementsByClassName("panel panel-default")[0]
                    .GetElementsByClassName("panel-body table-responsive")[0]
                    .GetElementsByClassName("table table-striped table-hover")[0]
                    .GetElementsByTagName("tbody")[0].GetElementsByTagName("tr");
                foreach (var vehicle in vehicles)
                {
                    vehiclwInformation = new VehiclwInformation();
                    vehiclwInformation.Year = vehicle.Children[1].TextContent.Trim();
                    vehiclwInformation.Make = vehicle.Children[2].TextContent.Trim();
                    vehiclwInformation.Model = vehicle.Children[3].TextContent.Trim();
                    vehiclwInformation.Type = vehicle.Children[4].TextContent.Trim();
                    vehiclwInformation.Color = vehicle.Children[5].TextContent.Trim();
                    vehiclwInformation.Plate = vehicle.Children[6].TextContent.Trim();
                    vehiclwInformation.VIN = vehicle.Children[7].TextContent.Trim();
                    vehiclwInformation.Lot = vehicle.Children[8].TextContent.Trim();
                    vehiclwInformation.AdditionalInfo = vehicle.Children[9].TextContent.Trim();
                    shipping.VehiclwInformations.Add(vehiclwInformation);
                }
            }
            catch (Exception)
            {
            }
        }

        private void SetDispatchInform(string sourse, ref Shipping shipping)
        {
            try
            {
                IHtmlDocument htmlDocument = htmlParser.Parse(sourse);
                var element = htmlDocument.GetElementById("sheetBottom")
                    .GetElementsByClassName("col-xs-12 col-md-4")[2]
                    .Children[0].Children[1];
                shipping.Titl1DI = element.TextContent.Trim().Replace("\n", "");
                shipping.Titl1DI = System.Text.RegularExpressions.Regex.Replace(shipping.Titl1DI, @"\s+", " ");

            }
            catch (Exception)
            {
            }
        }
    }
}
