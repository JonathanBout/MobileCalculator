using MobileCalculator.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace MobileCalculator
{
    public partial class TabPage : TabbedPage
    {
        public GraphSettings graphSettings => CalculatorSettings;
        public TabPage()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                Label source = (Label)sender;
                string url = "http://" + source.FormattedText.Spans.First(x => x.Text.StartsWith(" ")).Text.Trim(' ');
                Browser.OpenAsync(url);
            }catch (Exception ex)
            {
                DisplayAlert("Error", "Can't open browser: " + ex.Message, "Ok");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InfoLabel.Text = $"{AppInfo.Name} v{AppInfo.Version}";
#if DEBUG
            InfoLabel.Text += " [DEBUG]";
#endif
        }
    }
}
