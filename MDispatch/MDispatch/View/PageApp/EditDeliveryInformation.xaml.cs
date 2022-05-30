using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.ViewModels.PageAppMV;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.PageApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditDeliveryInformation : ContentPage
    {
        EditDeliveryInformationMV editDeliveryInformationMV = null;
        private readonly IHelperViewService _helperView;

        public EditDeliveryInformation(IManagerDispatchMobService managerDispatchMob, Shipping shipping)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            editDeliveryInformationMV = new EditDeliveryInformationMV(managerDispatchMob, shipping, Navigation);
            InitializeComponent();
            BindingContext = editDeliveryInformationMV;
            Init(shipping);
        }

        private async void Init(Shipping shipping)
        {
            if (shipping.idOrder == null || shipping.idOrder == "")
            {
                await lLoadId.TranslateTo(0, 13, 0);
                await lLoadId.FadeTo(0.5, 0);
            }
            else
            {
                await lLoadId.TranslateTo(0, 8, 0);
                lLoadId.TextColor = Color.FromHex("#131313");
            }
            if (shipping.NameD == null || shipping.NameD == "")
            {
                await lName.TranslateTo(0, 33, 0);
                await lName.FadeTo(0.5, 0);
            }
            else
            {
                await lName.TranslateTo(0, 8, 0);
                lName.TextColor = Color.FromHex("#131313");
            }
            if (shipping.ContactNameD == null || shipping.ContactNameD == "")
            {
                await lCName.TranslateTo(0, 33, 0);
                await lCName.FadeTo(0.5, 0);
            }
            else
            {
                await lCName.TranslateTo(0, 8, 0);
                lCName.TextColor = Color.FromHex("#131313");
            }
            if (shipping.AddresD == null || shipping.AddresD == "")
            {
                await lAddress.TranslateTo(0, 33, 0);
                await lAddress.FadeTo(0.5, 0);
            }
            else
            {
                await lAddress.TranslateTo(0, 8, 0);
                lAddress.TextColor = Color.FromHex("#131313");
            }
            if (shipping.CityD == null || shipping.CityD == "")
            {
                await lCity.TranslateTo(0, 33, 0);
                await lCity.FadeTo(0.5, 0);
            }
            else
            {
                await lCity.TranslateTo(0, 8, 0);
                lCity.TextColor = Color.FromHex("#131313");
            }
            if (shipping.StateD == null || shipping.StateD == "")
            {
                await lState.TranslateTo(0, 33, 0);
                await lState.FadeTo(0.5, 0);
            }
            else
            {
                await lState.TranslateTo(0, 8, 0);
                lState.TextColor = Color.FromHex("#131313");
            }
            if (shipping.ZipD == null || shipping.ZipD == "")
            {
                await lZip.TranslateTo(0, 33, 0);
                await lZip.FadeTo(0.5, 0);
            }
            else
            {
                await lZip.TranslateTo(0, 8, 0);
                lZip.TextColor = Color.FromHex("#131313");
            }
            if (shipping.PhoneD == null || shipping.PhoneD == "")
            {
                await lPhone.TranslateTo(0, 33, 0);
                await lPhone.FadeTo(0.5, 0);
            }
            else
            {
                await lPhone.TranslateTo(0, 8, 0);
                lPhone.TextColor = Color.FromHex("#131313");
            }
            if (shipping.EmailD == null || shipping.EmailD == "")
            {
                await lEmail.TranslateTo(0, 33, 0);
                await lEmail.FadeTo(0.5, 0);
            }
            else
            {
                await lEmail.TranslateTo(0, 8, 0);
                lEmail.TextColor = Color.FromHex("#131313");
            }
        }

        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lLoadId.TranslateTo(0, 8, 150),
                    lLoadId.FadeTo(1, 150)
                    );
                lLoadId.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lLoadId.TranslateTo(0, 33, 150),
                    lLoadId.FadeTo(0.5, 150)
                    );
                lLoadId.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void ELoadId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lName.TranslateTo(0, 8, 150),
                    lName.FadeTo(1, 150)
                    );
                lName.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lName.TranslateTo(0, 33, 150),
                    lName.FadeTo(0.5, 150)
                    );
                lName.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lCName.TranslateTo(0, 8, 150),
                    lCName.FadeTo(1, 150)
                    );
                lCName.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lCName.TranslateTo(0, 33, 150),
                    lCName.FadeTo(0.5, 150)
                    );
                lCName.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Entry_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lAddress.TranslateTo(0, 8, 150),
                    lAddress.FadeTo(1, 150)
                    );
                lAddress.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lAddress.TranslateTo(0, 33, 150),
                    lAddress.FadeTo(0.5, 150)
                    );
                lAddress.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Entry_TextChanged_3(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lCity.TranslateTo(0, 8, 150),
                    lCity.FadeTo(1, 150)
                    );
                lCity.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lCity.TranslateTo(0, 33, 150),
                    lCity.FadeTo(0.5, 150)
                    );
                lCity.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Entry_TextChanged_4(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lState.TranslateTo(0, 8, 150),
                    lState.FadeTo(1, 150)
                    );
                lState.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lState.TranslateTo(0, 33, 150),
                    lState.FadeTo(0.5, 150)
                    );
                lState.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Entry_TextChanged_5(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lZip.TranslateTo(0, 8, 150),
                    lZip.FadeTo(1, 150)
                    );
                lZip.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lZip.TranslateTo(0, 33, 150),
                    lZip.FadeTo(0.5, 150)
                    );
                lZip.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Entry_TextChanged_6(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lPhone.TranslateTo(0, 8, 150),
                    lPhone.FadeTo(1, 150)
                    );
                lPhone.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lPhone.TranslateTo(0, 33, 150),
                    lPhone.FadeTo(0.5, 150)
                    );
                lPhone.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Entry_TextChanged_7(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lEmail.TranslateTo(0, 8, 150),
                    lEmail.FadeTo(1, 150)
                    );
                lEmail.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lEmail.TranslateTo(0, 33, 150),
                    lEmail.FadeTo(0.5, 150)
                    );
                lEmail.TextColor = Color.FromHex("#b8babb");
            }
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _helperView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }
    }
}