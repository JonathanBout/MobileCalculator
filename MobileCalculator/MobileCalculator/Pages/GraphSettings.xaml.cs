using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCalculator.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GraphSettings : ContentPage
    {
        public int MinimumX
        {
            get
            {
                return (int)MinX.Value;
            }
            set
            {
                MinX.Value = value;
            }
        }

        public int MaximumX
        {
            get
            {
                return (int)MaxX.Value;
            }
            set
            {
                MaxX.Value = value;
            }
        }

        public int MinimumY
        {
            get
            {
                return (int)MinY.Value;
            }
            set
            {
                MinY.Value = value;
            }
        }

        public int MaximumY
        {
            get
            {
                return (int)MaxY.Value;
            }
            set
            {
                MaxY.Value = value;
            }
        }

        public double XStepSize 
        { 
            get 
            { 
                return XStep.Value; 
            } 
            set 
            { 
                XStep.Value = value;
            } 
        }

        public GraphSettings()
        {
            InitializeComponent();
            MinimumX = -10;
            MaximumX = 10;
            MinimumY = -10;
            MaximumY = 10;

            XStepSize = .2d;

            MaxX.Minimum = MinimumX + 1;
            MinX.Maximum = MaximumX - 1;

            MaxY.Minimum = MinimumY + 1;
            MinY.Maximum = MaximumY - 1;

            LowXLabel.Text = MinX.Value.ToString();
            HighXLabel.Text = MaxX.Value.ToString();
            LowYLabel.Text = MinY.Value.ToString();
            HighYLabel.Text = MaxY.Value.ToString();
            StepLabel.Text = XStep.Value.ToString();

        }

        private void MinX_Completed(object sender, EventArgs e)
        {
            MaxX.Minimum = MinimumX + 1;
            LowXLabel.Text = MinX.Value.ToString();
        }

        private void MaxX_Completed(object sender, EventArgs e)
        {
            MinX.Maximum = MaximumX - 1;
            HighXLabel.Text = MaxX.Value.ToString();
        }

        private void MaxY_Completed(object sender, EventArgs e)
        {
            MaxY.Minimum = MinimumY + 1;
            HighYLabel.Text = MaxY.Value.ToString();
        }

        private void MinY_Completed(object sender, EventArgs e)
        {
            MinY.Maximum = MaximumY - 1;
            LowYLabel.Text = MinY.Value.ToString();
        }

        private void XStep_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            StepLabel.Text = XStep.Value.ToString();
        }
    }
}