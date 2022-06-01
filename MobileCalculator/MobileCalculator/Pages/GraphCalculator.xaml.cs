using Microcharts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Evaluate();
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            SetViewStackOrientation(e.DisplayInfo.Orientation);
        }

        void SetViewStackOrientation(DisplayOrientation orientation)
        {
            ViewStack.Orientation = orientation == DisplayOrientation.Portrait ? StackOrientation.Vertical : StackOrientation.Horizontal;
        }

        void Evaluate()
        {
            List<(Entry, Graph)> formulaInputs = new List<(Entry, Graph)>()
            {
                (Y1Input, new Graph()), (Y2Input, new Graph()), (Y3Input, new Graph()), (Y4Input, new Graph()), (Y5Input, new Graph()),
            };
            formulaInputs = formulaInputs.FindAll(x => !string.IsNullOrEmpty(x.Item1.Text)).ToList();
            for (double x = minX; x < maxX; x+= .2d)
            {
                Dictionary<string, double> vars = new Dictionary<string, double>() { { "X", x } };
                foreach ((Entry, Graph) formulaInput in formulaInputs)
                {
                    if (!string.IsNullOrEmpty(formulaInput.Item1.Text))
                    {
                        double result = ExtraMath.EvaluateAdvanced(formulaInput.Item1.Text, vars);
                        formulaInput.Item2.AddPoint(result);
                    }
                }
            }
            Chart chart = new Chart(formulaInputs.Select(x => x.Item2));
            SetChart(chart.ToChartSerieArray());
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
            SetChart(new ChartSerie() { Entries = entries, Color = SkiaSharp.SKColor.Parse("#FFF") });
        }

        public void SetChart(params ChartSerie[] ChartEntries)
        {
            var random = new Random();
            foreach(ChartSerie serie in ChartEntries)
            {
                var color = string.Format("#{0:X6}", random.Next(16777216));
                serie.Color = SkiaSharp.SKColor.Parse(color);
            }
            Chart.Series = ChartEntries;
            Graph.Chart = Chart;
        }
    }

    public class Chart : IEnumerable<Graph>
    {
        List<Graph> graphs = new List<Graph>();

        public Chart(IEnumerable<Graph> graphs)
        {
            this.graphs = graphs.ToList();
        }

        public ChartSerie[] ToChartSerieArray()
        {
            return graphs.Select(x => x.ToChartSerie()).ToArray();
        }

        public void Add(Graph graph)
        {
            graphs.Add(graph);
        }
        public IEnumerator<Graph> GetEnumerator()
        {
            return graphs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Graph : IEnumerable<double>
    {
        List<double> points = new List<double>();

        public Graph(IEnumerable<double> points)
        {
            this.points = points.ToList();
        }

        public Graph() { }

        public void AddPoint(double value)
        {
            points.Add(value);
        }

        public IEnumerator<double> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ChartSerie ToChartSerie()
        {
            ChartSerie serie = new ChartSerie();
            List<ChartEntry> entries = new List<ChartEntry>();
            foreach (double point in points)
            {
                entries.Add(new ChartEntry((float)point));
            }
            serie.Entries = entries;
            return serie;
        }
    }
}