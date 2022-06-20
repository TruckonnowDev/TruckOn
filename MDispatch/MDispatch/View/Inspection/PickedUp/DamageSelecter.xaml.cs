using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.PickedUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DamageSelecter : PopupPage
    {
        private FullPagePhotoMV FullPagePhotoMV = null;
        private PageAddDamage pageAddDamage = null;

        public DamageSelecter(FullPagePhotoMV fullPagePhotoMV, PageAddDamage pageAddDamage)
        {
            this.pageAddDamage = pageAddDamage;
            FullPagePhotoMV = fullPagePhotoMV;
            InitializeComponent();
            damageCollection.ItemsSource = collectionDamadge;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            pageAddDamage.indexSelectDamage = collectionDamadge.IndexOf((string)e.SelectedItem);
            pageAddDamage.prefNameDamage = e.SelectedItem.ToString().Remove(e.SelectedItem.ToString().IndexOf('-')-1);
            pageAddDamage.nameDamage = e.SelectedItem.ToString().Remove(0, e.SelectedItem.ToString().IndexOf('-')+2);
            pageAddDamage.stateSelect = 0;
        }

        protected override bool OnBackgroundClicked()
        {
            pageAddDamage.stateSelect = 2;
            return true;
        }

        List<string> collectionDamadge = new List<string>()
        {
            "S - Scratched",
            "MS - Multiple Scratcheds",
            "D - Dent",
            "PC - Paint Chip",
            "MD - Major Damage",
            "CR - Cracked",
            "SC - Scuffed",
            "R - Rubbed",
            "FF - Foreingn Fluid",
            "M - Missing",
            "BR - Broken",
            "F - Faded",
            "FT - Flat Tire",
            "G - Gouge",
            "HD - Hail Damage",
            "LC - Loose Contents",
            "RU - Rust",
            "O - Other"
        };

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            pageAddDamage.stateSelect = 2;
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}