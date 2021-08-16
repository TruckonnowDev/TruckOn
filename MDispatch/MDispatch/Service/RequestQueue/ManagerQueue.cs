using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service.Helpers;
using MDispatch.Service.Net;
using MDispatch.View.GlobalDialogView;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MDispatch.Service.RequestQueue
{
    public class ManagerQueue
    {
        private static List<RequestModel> RequvestAll = new List<RequestModel>();
        private static int CountWorkRequest = 0;
        private static ManagerDispatchMob managerDispatchMob = new ManagerDispatchMob();

        public static async Task AddReqvest(string nameRequvest, string token, params dynamic[] param)
        {
            if (nameRequvest != "")
            {
                RequvestAll.Add(new RequestModel()
                {
                    NameRequvest = nameRequvest,
                    Token = token,
                    ParamsRequvest = new List<dynamic>(param)
                });
            }
            Task.Run(() => StartRequest());
        }

        private static async Task StartRequest()
        {
            if (App.isNetwork)
            {
                if (CountWorkRequest < 2 && RequvestAll.Count > 0)
                {
                    RequestModel request = RequvestAll.FirstOrDefault(r => !r.IsWork);
                    if (request != null && request.NameRequvest == "SaveInspactionDriver")
                    {
                        request.IsWork = true;
                        CountWorkRequest++;
                        await SaveInspactionDriver(request);
                    }
                    else if(request != null && request.NameRequvest == "SavePhoto")
                    {
                        request.IsWork = true;
                        CountWorkRequest++;
                        await SavePhoto(request);
                    }
                }
            }
            else
            {

            }
        }

        private async static Task SaveInspactionDriver(RequestModel request)
        {
            string description = null;
            int state = 0;
            string idDriver = request.ParamsRequvest[0];
            Photo photo = request.ParamsRequvest[1];
            int indexCurent = request.ParamsRequvest[2];
            string typeTransportVehicle = request.ParamsRequvest[3];
            await Task.Run(() =>
            {
                state = managerDispatchMob.AskWork(request.NameRequvest, request.Token, idDriver, photo, ref description, null, indexCurent, typeTransportVehicle);
            });
            Device.BeginInvokeOnMainThread(async () =>
            {

                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                { 
                    HelpersView.CallError(description);
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    RequvestAll.Remove(request);
                    CountWorkRequest--;
                    Task.Run(() => StartRequest());
                }
                else if (state == 4)
                {
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            });
        }

        private async static Task SavePhoto(RequestModel request)
        {
            string description = null;
            int state = 0;
            string idVecl = request.ParamsRequvest[0];
            PhotoInspection photoInspection = request.ParamsRequvest[1];
            await Task.Run(() =>
            {
                state = managerDispatchMob.AskWork(request.NameRequvest, request.Token, idVecl, photoInspection, ref description);
            });
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    HelpersView.CallError(description);
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    RequvestAll.Remove(request);
                    CountWorkRequest--;
                    Task.Run(() => StartRequest());
                }
                else if (state == 4)
                {
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            });
        }
    }
}