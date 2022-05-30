using MDispatch.Helpers;
using MDispatch.Service.HelperView;
using MDispatch.Service.Tasks;
using MDispatch.View.Popups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Settings;
using RestSharp;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.Utils
{
    public class UtilsService : IUtilsService
    {
        private bool isTimeUpdate = true;
        private readonly IHelperViewService _helpersView;

        public UtilsService()
        {
            _helpersView = DependencyService.Get<IHelperViewService>();
        }

        #region Implementation

        [Obsolete]
        public async Task StartListening(bool isTwoConection = false)
        {
            if (CrossGeolocator.Current.IsListening)
            {
                return;
            }
            try
            {
                bool s = await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 1, true);
                CrossGeolocator.Current.DesiredAccuracy = 50;
                CrossGeolocator.Current.PositionChanged += PositionChanged;
            }
            catch (GeolocationException)
            {
                var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (permissionStatus == PermissionStatus.Denied)
                {
                    if (isTwoConection)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        await PopupNavigation.PushAsync(new WarningPopupView());
                    }
                }
            }
        }

        public void RequestGPS(string longitude, string latitude)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                string token = CrossSettings.Current.GetValueOrDefault("Token", "");
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/GPS/Save", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 10000;
                request.AddParameter("token", token);
                request.AddParameter("longitude", longitude);
                request.AddParameter("latitude", latitude);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
            }
        }

        public async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
            {
                return;
            }
            await CrossGeolocator.Current.StopListeningAsync();
            CrossGeolocator.Current.PositionChanged -= PositionChanged;
        }

        public async void CheckNet(bool isInspection = false, bool isQueue = false)
        {
            var sync = SynchronizationContext.Current;
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Mobile/Net", Method.GET);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 3000;
                response = client.Execute(request);
                content = response.Content;

                if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    if (App.isNetwork)
                    {
                        TaskManager.isWorkTask = false;
                        App.isNetwork = false;
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        _helpersView.CallError(LanguageHelper.NotNetworkAlert);
                    });
                }
                else
                {
                    bool isCheck = false;
                    string description = null;
                    GetData(content, ref isCheck, ref description);
                    if (!isCheck)
                    {
                        if (App.isNetwork)
                        {
                            TaskManager.isWorkTask = false;
                            App.isNetwork = false;
                        }
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _helpersView.CallError(description);
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _helpersView.Hidden();
                        });
                        //if (!TaskManager.isWorkTask)
                        //{
                        //    TaskManager.CommandToDo("CheckTask");
                        //}
                        //if (!isQueue)
                        //{
                        //    await ManagerQueue.AddReqvest("", null);
                        //}
                        TaskManager.isWorkTask = true;
                        App.isNetwork = true;
                    }
                }
            }
            catch (Exception e)
            {
                App.isNetwork = false;
            }
        }


        #endregion

        #region Private Helpers

        [Obsolete]
        private async void PositionChanged(object sender, PositionEventArgs e)
        {
            await Task.Run(async () =>
            {
                //await Task.Run(() => Net.Utils.CheckNet());
                if (App.isNetwork && isTimeUpdate)
                {
                    Waite();
                    RequestGPS(e.Position.Longitude.ToString(), e.Position.Latitude.ToString());
                }
            });
        }

        private async void Waite()
        {
            await Task.Run(() =>
            {
                isTimeUpdate = false;
                Thread.Sleep(120000);
                isTimeUpdate = true;
            });
        }

        private static void GetData(string respJsonStr, ref bool isCheck, ref string description)
        {
            if (respJsonStr[0] == '!')
            {
                isCheck = false;
                description = "Technical work on the service";
                return;
            }
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                isCheck = Convert.ToBoolean((responseAppS.
                        SelectToken("ResponseStr").ToString()));
                description = JsonConvert.DeserializeObject<string>(responseAppS.
                        SelectToken("Description").ToString());
            }
            else
            {
                isCheck = false;
                description = "Not Network";
            }
        }

        #endregion
    }
}
