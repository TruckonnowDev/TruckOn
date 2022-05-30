using MDispatch.View.PageApp;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.PickedUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DamageSelecter1 : PopupPage
    {
        private PageAddDamageFoUser pageAddDamageFoUser = null;
        private PageAddDamage1 pageAddDamage = null;

        public DamageSelecter1(PageAddDamageFoUser pageAddDamageFoUser, PageAddDamage1 pageAddDamage)
        {
            this.pageAddDamageFoUser = pageAddDamageFoUser;
            this.pageAddDamage = pageAddDamage;
            InitializeComponent();
            damageCollection.ItemsSource = collectionDamadge;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (pageAddDamageFoUser != null)
            {
                pageAddDamageFoUser.indexSelectDamage = collectionDamadge.IndexOf((string)e.SelectedItem);
                pageAddDamageFoUser.prefNameDamage = e.SelectedItem.ToString().Remove(e.SelectedItem.ToString().IndexOf('-') - 1);
                pageAddDamageFoUser.nameDamage = e.SelectedItem.ToString().Remove(0, e.SelectedItem.ToString().IndexOf('-') + 2);
                pageAddDamageFoUser.stateSelect = 0;
            }
            else
            {
                pageAddDamage.indexSelectDamage = collectionDamadge.IndexOf((string)e.SelectedItem);
                pageAddDamage.prefNameDamage = e.SelectedItem.ToString().Remove(e.SelectedItem.ToString().IndexOf('-') - 1);
                pageAddDamage.nameDamage = e.SelectedItem.ToString().Remove(0, e.SelectedItem.ToString().IndexOf('-') + 2);
                pageAddDamage.stateSelect = 0;
            }
        }

        protected override bool OnBackgroundClicked()
        {
            if (pageAddDamageFoUser != null)
            {
                pageAddDamageFoUser.stateSelect = 2;
            }
            else
            {
                pageAddDamage.stateSelect = 2;
            }
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
            "B - Bulged",
            "C - Chip",
            "DN - Ding",
            "HS - Hairline Scratch",
            "P - Peeling",
            "PT - Pitted",
            "O - Other"
        };

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            if (pageAddDamageFoUser != null)
            {
                pageAddDamageFoUser.stateSelect = 2;
            }
            else
            {
                pageAddDamage.stateSelect = 2;
            }
            await Navigation.PopAsync(true);
        }
    }
}