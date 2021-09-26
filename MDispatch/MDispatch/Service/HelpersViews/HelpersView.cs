using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.Helpers
{
    public class HelpersView
    {
        private static StackLayout StackLayout { get; set; }
        private static FlexLayout FlexLayout { get; set; }
        private static AbsoluteLayout AbsoluteLayout { get; set; }
        private static bool IsAddAlert { get; set; } = false;

        public static void CallError(string info)
        {
            StackLayout stackLayout1 = new StackLayout()
            {
                HeightRequest = 15,
                BackgroundColor = Color.FromHex("#ff8080"),
                //TranslationY = -16,
                AutomationId = "la"
            };
            stackLayout1.Children.Add(new Label()
            {
                Text = info,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 12
            });
            if (StackLayout != null
                && StackLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la") == null
                && !IsAddAlert)
            {
                IsAddAlert = true;
                StackLayout.Children.Insert(0, stackLayout1);
                StackLayout.TranslationY = -15;
                UnHidden(StackLayout);
            }
            else if(FlexLayout != null
                && FlexLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la") == null
                && !IsAddAlert)
            {
                IsAddAlert = true;
                FlexLayout.Children.Insert(0, stackLayout1);
                FlexLayout.TranslationY = -15;
                UnHidden(FlexLayout);
            }
        }

        private static void UnHidden(Layout layout)
        {
            layout.TranslateTo(0, 0);
        }

        public static async void Hidden()
        {
            if (StackLayout != null
                && StackLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la") != null
                && IsAddAlert)
            {
                IsAddAlert = false;
                await StackLayout.TranslateTo(0, -15);
                var el = StackLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la");
                if (el != null && !IsAddAlert)
                {
                    StackLayout.Children.Remove(el);
                }
                StackLayout.TranslationY = 0;
            }
            else if (FlexLayout != null
                && FlexLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la") != null
                && IsAddAlert)
            {
                IsAddAlert = false;
                await FlexLayout.TranslateTo(0, -15);
                var el = FlexLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la");
                if (el != null && !IsAddAlert)
                {
                    FlexLayout.Children.Remove(el);
                }
                FlexLayout.TranslationY = 0;
            }
        }

        public static void InitAlert(FlexLayout flexLayout)
        {
            StackLayout = null;
            FlexLayout = null;
            FlexLayout = flexLayout;
        }

        public static void InitAlert(StackLayout stackLayout)
        {
            StackLayout = null;
            FlexLayout = null;
            StackLayout = stackLayout;
        }

        internal static void ReSet()
        {
            if (StackLayout != null
                && StackLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la") != null)
            {
                IsAddAlert = false;
                StackLayout.TranslateTo(0, -15);
                var el = StackLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la");
                if (el != null)
                {
                    StackLayout.Children.Remove(el);
                }
                StackLayout.TranslationY = 0;
            }
            else if (FlexLayout != null
                && FlexLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la") != null)
            {
                IsAddAlert = false;
                FlexLayout.TranslateTo(0, -15);
                var el = FlexLayout.Children.ToList().FirstOrDefault(l => l.AutomationId == "la");
                if (el != null)
                {
                    FlexLayout.Children.Remove(el);
                }
                FlexLayout.TranslationY = 0;
            }
            StackLayout = null;
            FlexLayout = null;
            IsAddAlert = false;
        }
    }
}