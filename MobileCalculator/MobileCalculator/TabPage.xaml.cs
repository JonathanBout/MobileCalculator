using MobileCalculator.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileCalculator
{
    public partial class TabPage : TabbedPage
    {
        public TabPage()
        {
            InitializeComponent();
            //GraphNavigation.ToolbarItems.Add(new ToolbarItem() { Text = "Help", });
            //GraphNavigation.PushAsync(new GraphCalculator());
        }
    }
}
