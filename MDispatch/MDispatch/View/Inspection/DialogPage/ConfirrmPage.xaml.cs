using MDispatch.ViewModels.PageAppMV;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.AskPhoto.DialogPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfirrmPage : PopupPage
    {
        private InfoOrderMV infoOrderMV = null;
        private int selectedIndexDropDwn = -1;

        public ConfirrmPage (InfoOrderMV infoOrderMV)
		{
            this.infoOrderMV = infoOrderMV;
			InitializeComponent ();
            BindingContext = infoOrderMV;
            InitDropD();
        }

        private void InitDropD()
        {
            dropDwnCar.ItemsSource = new List<string>(infoOrderMV.Shipping.VehiclwInformations.Select(v => v.Make));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if(selectedIndexDropDwn != -1)
            {
                infoOrderMV.ToStartInspection();
                await PopupNavigation.Instance.PopAsync(true);
            }
            else
            {
                dropDwnCar.BorderColor = Color.Red;
            }
        }

        private void DropDwnCar_SelectedItemChanged(object sender, Plugin.InputKit.Shared.Utils.SelectedItemChangedArgs e)
        {
            selectedIndexDropDwn = e.NewItemIndex;
        }
    }
}