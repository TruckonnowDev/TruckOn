using System;
using System.Collections.Generic;
using MDispatch.Helpers;
using Plugin.Settings;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MDispatch.View.PageApp.Settings
{
    public partial class SelectLanguage : PopupPage
    {
        private Action<int> HandlerSelectLang;

        public SelectLanguage(Action<int> handlerSelectLang)
        {
            InitializeComponent();
            HandlerSelectLang = handlerSelectLang;
            collectionLangLv.ItemsSource = collectionLang;
        }

        private List<LangModel> collectionLang = new List<LangModel>()
        {
            new LangModel()
            {
                Titel = "English",
                Icon = "EnglishLanIcon.png",
            },
            new LangModel()
            {
                Titel = "Русский",
                Icon = "RussianLanIcon.png",
            },
            new LangModel()
            {
                Titel = "Español",
                Icon = "SpanishLanIcon.png",
            },
        };

        private async void CollectionLang_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            CrossSettings.Current.AddOrUpdateValue("Language", e.SelectedItemIndex);
            if (HandlerSelectLang != null)
            {
                HandlerSelectLang(e.SelectedItemIndex);
            }
            LanguageHelper.InitLanguage();
            await PopupNavigation.PopAsync();
        }

        private async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        private class LangModel
        {
            public string Titel { get; set; }
            public string Icon { get; set; }
        }
    }
}
