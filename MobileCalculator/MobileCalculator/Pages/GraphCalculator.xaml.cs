using Calculator.Pages;
using Microcharts;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCalculator.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GraphCalculator : ContentPage
    {
        LineChart Chart { get; }
        int minX = -5;
        int maxX = 5;
        public GraphCalculator()
        {
            InitializeComponent();
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
            Chart = new LineChart()
            {
                BackgroundColor = SkiaSharp.SKColor.Parse("#333"),
                LabelColor = SkiaSharp.SKColor.Parse("#FFF"),
                LineMode = LineMode.Straight,
                AnimationDuration = new TimeSpan(0, 0, 0, 0, 500),
                LabelOrientation = Orientation.Horizontal,
                LabelTextSize = 30f,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                YAxisTextPaint = new SkiaSharp.SKPaint { TextSize = 30f, Color = SkiaSharp.SKColor.Parse("#EEE") },
                YAxisLinesPaint = new SkiaSharp.SKPaint() { Color = SkiaSharp.SKColor.Parse("#EEE") },
                ShowYAxisLines = true,
                PointMode = PointMode.None,
                IsAnimated = false,
            };
            SetViewStackOrientation(DeviceDisplay.MainDisplayInfo.Orientation);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                decimal[][] values = new decimal[4][];
                //Fill the graph
                for (int i = minX; i < maxX; i++)
                {
                    values[0][i] = decimal.Parse(ExtraMath.Evaluate(Y1Input.Text, x: i.ToString()));
                    values[1][i] = decimal.Parse(ExtraMath.Evaluate(Y2Input.Text, x: i.ToString()));
                    values[2][i] = decimal.Parse(ExtraMath.Evaluate(Y3Input.Text, x: i.ToString()));
                    values[3][i] = decimal.Parse(ExtraMath.Evaluate(Y4Input.Text, x: i.ToString()));

                }
                List<ChartSerie> series = new List<ChartSerie>();
                foreach (decimal[] values2 in values)
                {
                    List<ChartEntry> entries = new List<ChartEntry>();
                    foreach (decimal value in values2)
                    {
                        entries.Add(new ChartEntry((float)value)
                        {
                            Color = SkiaSharp.SKColor.Parse("#FFF"),
                        });
                    }
                    series.Add(new ChartSerie()
                    {
                        Entries = entries
                    });
                }
                SetChart(series.ToArray());
            }catch (Exception ex)
            {
                DisplayAlert("Error", $"Error: {ex.Message} (Error Type: {ex.GetType()})", "Ok");
            }
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            SetViewStackOrientation(e.DisplayInfo.Orientation);
        }

        void SetViewStackOrientation(DisplayOrientation orientation)
        {
            ViewStack.Orientation = orientation == DisplayOrientation.Portrait ? StackOrientation.Vertical : StackOrientation.Horizontal;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Random r = new Random();
            var entries = new[]
            {
                new ChartEntry(r.Next(-5, 5))
                {
                    Color=SkiaSharp.SKColor.Parse("#FFF"),
                },
                new ChartEntry(r.Next(-5, 5))
                {
                    Color=SkiaSharp.SKColor.Parse("#FFF"),
                },
                new ChartEntry(r.Next(-5, 5))
                {
                    Color=SkiaSharp.SKColor.Parse("#FFF"),
                },
                new ChartEntry(r.Next(-5, 5))
                {
                    Color=SkiaSharp.SKColor.Parse("#FFF"),
                },
                new ChartEntry(r.Next(-5, 5))
                {
                    Color=SkiaSharp.SKColor.Parse("#FFF"),
                },
                new ChartEntry(r.Next(-5, 5))
                {
                    Color=SkiaSharp.SKColor.Parse("#FFF"),
                }
            };
            SetChart(new ChartSerie() { Entries = entries, Color=SkiaSharp.SKColor.Parse("#FFF") });
        }

        public void SetChart(params ChartSerie[] ChartEntries)
        {
            Chart.Series = ChartEntries;
            Graph.Chart = Chart;
        }
    }
}