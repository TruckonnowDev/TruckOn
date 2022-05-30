using FormsControls.Base;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.StoreNotify;
using MDispatch.Service.Tasks;
using MDispatch.Service.Utils;
using MDispatch.View;
using MDispatch.View.A_R;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.TabPage;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels
{
    public class AvtorizationMV : BaseViewModel
    {
        private readonly IManagerDispatchMobService managerDispatchMob;
        private readonly IUtilsService _utilsService;
        public DelegateCommand AvtorizationCommand { get; set; }

        public AvtorizationMV(
            INavigation navigation)
            : base(navigation)
        {
            _utilsService = DependencyService.Get<IUtilsService>();
            managerDispatchMob = DependencyService.Get<IManagerDispatchMobService>();
            AvtorizationCommand = new DelegateCommand(Avtorization);
        }

        private string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string fullName;
        public string FullName
        {
            get => fullName;
            set => SetProperty(ref fullName, value);
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value);
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                SetProperty(ref email, value);
            }
        }

        private string feedBack = "    ";
        public string FeedBack
        {
            get { return feedBack; }
            set
            {
                SetProperty(ref feedBack, value);
            }
        }

        private string feedBack1 = "";
        public string FeedBack1
        {
            get { return feedBack1; }
            set
            {
                SetProperty(ref feedBack1, value);
            }
        }

        private async void Avtorization()
        {
            await _navigation.PushAsync(new LoadPage(), true);
            string token = null;
            string description = null;
            int state = 3;
            await Task.Run(() =>
            {
                state = managerDispatchMob.A_RWork("authorisation", Username, Password, ref description, ref token);
            });
            await _navigation.PopAsync(true);
            if (state == 1)
            {
                await _navigation.PushAsync(new Alert(description, null));
                FeedBack = "Not Network";
            }
            else if(state == 2)
            {
                await _navigation.PushAsync(new Alert(description, null));
                FeedBack = description;
            }
            else if(state == 3)
            {
                //AuthenticationID.AvtorizatiionID();
                App.isAvtorization = true;
                CrossSettings.Current.AddOrUpdateValue("Token", token.Split(',')[0]);
                CrossSettings.Current.AddOrUpdateValue("IdDriver", token.Split(',')[1]);
                CrossSettings.Current.AddOrUpdateValue("Username", Username);
                CrossSettings.Current.AddOrUpdateValue("Password", Password);
                await Task.Run(() =>
                {
                    DependencyService.Get<IStore>().OnTokenRefresh();
                    _utilsService.StartListening();
                    TaskManager.CommandToDo("CheckTask");
                });

                    Application.Current.MainPage = new AnimationNavigationPage(new TabPage());

            }
            else if(state == 4)
            {
                await _navigation.PushAsync(new Alert(description, null));
                FeedBack = "Technical work on the service";
            }
        }

        
        public async void RequestPasswordChanges()
        {
            await _navigation.PushAsync(new LoadPage(), true);
            string token = null;
            string description = null;
            int state = 3;
            await Task.Run(() =>
            {
                state = managerDispatchMob.A_RWork("RequestPasswordChanges", Email, FullName, ref description, ref token);
            });
            await _navigation.PopAsync(true);
            if (state == 1)
            {
                await _navigation.PushAsync(new Alert(description, null));
                FeedBack1 = "Not Network";
            }
            else if (state == 2)
            {
                await _navigation.PushAsync(new Alert(description, null));
                FeedBack1 = description;
            }
            else if (state == 3)
            {
                await _navigation.PopToRootAsync();
                FeedBack1 = "";
                await _navigation.PushAsync(new InfoRecovery(this));
            }
            else if (state == 4)
            {
                await _navigation.PushAsync(new Alert(description, null));
                FeedBack1 = "Technical work on the service";
            }
        }
    }
}