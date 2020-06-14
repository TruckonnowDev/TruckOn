﻿using FormsControls.Base;
using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.View.GlobalDialogView;
using MDispatch.ViewModels.PageAppMV;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.View.PageApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoOrder : ContentPage
    {
        private InfoOrderMV infoOrderMV = null;
        private bool isNextPage = false;
        private ManagerDispatchMob managerDispatchMob;
        private InitDasbordDelegate initDasbordDelegate;
        private string currentStatus;

        public InfoOrder(ManagerDispatchMob managerDispatchMob, InitDasbordDelegate initDasbordDelegate, string statusInspection, string idShipping)
        {
            this.infoOrderMV = new InfoOrderMV(managerDispatchMob, initDasbordDelegate, statusInspection, idShipping) { Navigation = this.Navigation} ;
            InitializeComponent();
            BindingContext = this.infoOrderMV;
        }

        private void StackLayout_SizeChanged(object sender, EventArgs e)
        {
            if(infoOrderMV.Shipping != null && infoOrderMV.Shipping.VehiclwInformations != null && infoOrderMV.Shipping.VehiclwInformations.Count != 0)
            {
                listVehic.HeightRequest = Convert.ToInt32(infoOrderMV.Shipping.VehiclwInformations.Count * 120);
            }
            else
            {
                listVehic.HeightRequest = 0;
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Button button = ((Button)sender);
            string id = button.FindByName<Label>("idL").Text;
            infoOrderMV.ToVehicleDetails(infoOrderMV.Shipping.VehiclwInformations.Find(v => v.Id == id));
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            if(infoOrderMV.Shipping == null)
            {
                HelpersView.CallError("Not Data");
                return;
            }

            if(infoOrderMV.Shipping.CurrentStatus == "Assigned")
            {
                infoOrderMV.ToStartInspection();
            }
            else if(infoOrderMV.Shipping.CurrentStatus == "Picked up")
            {
                infoOrderMV.ToStartInspectionDelyvery();
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new ContactInfo());
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            HelpersView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HelpersView.Hidden();
        }
    }
}