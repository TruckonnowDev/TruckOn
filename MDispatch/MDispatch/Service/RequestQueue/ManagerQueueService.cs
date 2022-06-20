using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service.GlobalHelper;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.GlobalDialogView;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.RequestQueue
{
    public class ManagerQueueService : IManagerQueueService
    {
        private List<RequestModel> RequvestAll = new List<RequestModel>();
        private int CountWorkRequest = 0;
        private readonly IManagerDispatchMobService _managerDispatchMob;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IHelperViewService _helperView;

        public ManagerQueueService()
        {
            _globalHelper = DependencyService.Get<IGlobalHelperService>();
            _helperView = DependencyService.Get<IHelperViewService>();
            _managerDispatchMob = DependencyService.Get<IManagerDispatchMobService>();
        }

        public async Task AddRequest(string nameRequvest, string token, params dynamic[] param)
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

        private async Task StartRequest()
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

        [System.Obsolete]
        private async Task SaveInspactionDriver(RequestModel request)
        {
            string description = null;
            int state = 0;
            string idDriver = request.ParamsRequvest[0];
            Photo photo = request.ParamsRequvest[1];
            int indexCurent = request.ParamsRequvest[2];
            string typeTransportVehicle = request.ParamsRequvest[3];
            await Task.Run(() =>
            {
                state = _managerDispatchMob.AskWork(request.NameRequvest, request.Token, idDriver, photo, ref description, null, indexCurent, typeTransportVehicle);
            });
            Device.BeginInvokeOnMainThread(async () =>
            {

                if (state == 1)
                {
                    _globalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                { 
                    _helperView.CallError(description);
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
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            });
        }

        private async Task SavePhoto(RequestModel request)
        {
            string description = null;
            int state = 0;
            string idVecl = request.ParamsRequvest[0];
            PhotoInspection photoInspection = request.ParamsRequvest[1];
            await Task.Run(() =>
            {
                state = _managerDispatchMob.AskWork(request.NameRequvest, request.Token, idVecl, photoInspection, ref description);
            });
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (state == 1)
                {
                    _globalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    _helperView.CallError(description);
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
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            });
        }
    }
}