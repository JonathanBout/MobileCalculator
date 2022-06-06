//using Microcharts;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

namespace MobileCalculator.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GraphCalculator : ContentPage
    {
        //LineChart Chart { get; }
        int minX => settings.MinimumX;
        int maxX => settings.MaximumX;

        int minY => settings.MinimumY;
        int maxY => settings.MaximumY;
        double xStep => settings.XStepSize;
        GraphSettings settings => ((TabPage)Application.Current.MainPage).graphSettings;
        int seed = -1;
        public int Seed
        {
            get
            {
                seed++;
                return seed;
            }
        }
        public GraphCalculator()
        {
            InitializeComponent();
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
            SetViewStackOrientation(DeviceDisplay.MainDisplayInfo.Orientation);
            Device.InvokeOnMainThreadAsync(Evaluate);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Device.InvokeOnMainThreadAsync(Evaluate);
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
                (Y1Input, new Graph() {GraphName = "Y1"}),
                (Y2Input, new Graph() {GraphName = "Y2"}),
                (Y3Input, new Graph() {GraphName = "Y3"}),
                (Y4Input, new Graph() {GraphName = "Y4"}),
                (Y5Input, new Graph() {GraphName = "Y5"}),
            };
            formulaInputs = formulaInputs.FindAll(x => !string.IsNullOrEmpty(x.Item1.Text)).ToList();
            for (double x = minX; x < maxX; x = Math.Min(x + xStep, maxX))
            {
                Dictionary<string, double> vars = new Dictionary<string, double>() { { "X", x } };
                foreach ((Entry, Graph) formulaInput in formulaInputs)
                {
                    if (!string.IsNullOrEmpty(formulaInput.Item1.Text))
                    {
                        double result = ExtraMath.EvaluateAdvanced(formulaInput.Item1.Text, vars);
                        formulaInput.Item2.AddPoint(new DataPoint(x, result));
                    }
                }
            }
            Chart chart = new Chart(formulaInputs.Select(x => x.Item2));
            SetChart(chart.ToSeriesArray());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        public void SetChart(params Series[] ChartEntries)
        {
            PlotModel model = new PlotModel()
            {
                PlotType = PlotType.XY,
                TextColor = OxyColor.FromRgb(255, 255, 255),
                IsLegendVisible = true,
                LegendBackground = OxyColor.FromArgb(125, 100, 100, 100)
            };

            model.Axes.Clear();

            model.Axes.Add(new LinearAxis() { Key="xAxis", AbsoluteMaximum = maxX, AbsoluteMinimum = minX,
                MajorGridlineColor = OxyColor.FromArgb(150, 255, 255, 255), MajorGridlineStyle = LineStyle.Automatic,
                MinorGridlineColor = OxyColor.FromArgb(25, 255, 255, 255), MinorGridlineStyle = LineStyle.Automatic,
                MinorGridlineThickness = .5d, Position = AxisPosition.Bottom, Maximum = maxX, Minimum = minX }) ;

            model.Axes.Add(new LinearAxis() { Key = "yAxis", AbsoluteMaximum = maxY, AbsoluteMinimum = minY,
                MajorGridlineColor = OxyColor.FromArgb(150, 255, 255, 255), MajorGridlineStyle = LineStyle.Automatic,
                MinorGridlineColor = OxyColor.FromArgb(100, 255, 255, 255), MinorGridlineStyle = LineStyle.Automatic,
                MinorGridlineThickness = .5d, Position = AxisPosition.Left, Maximum = maxY, Minimum = minY }) ;

            foreach (LineSeries chartEntry in ChartEntries)
            {
                Random r = new Random(Seed);
                chartEntry.Color = OxyColor.FromUInt32(GetRandomUInt(r)).ChangeIntensity(1.4);
                model.Series.Add(chartEntry);
            }
            Graph.Model = model;
        }

        public uint GetRandomUInt(Random r)
        {
            uint thirtyBits = (uint)r.Next(1 << 30);
            uint twoBits = (uint)r.Next(1 << 2);
            return (thirtyBits << 2) | twoBits;
        }

    }

    public class Chart : IEnumerable<Graph>
    {
        List<Graph> graphs = new List<Graph>();

        public Chart(IEnumerable<Graph> graphs)
        {
            this.graphs = graphs.ToList();
        }

        public Series[] ToSeriesArray()
        {
            return graphs.Select(x => x.ToSeries()).ToArray();
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

    public class Graph : IEnumerable<DataPoint>
    {
        public string GraphName = "";
        List<DataPoint> points = new List<DataPoint>();

        public Graph(IEnumerable<DataPoint> points, string graphName)
        {
            this.points = points.ToList();
            GraphName = graphName;
        }

        public Graph() { }

        public void AddPoint(DataPoint value)
        {
            points.Add(value);
        }

        public IEnumerator<DataPoint> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Series ToSeries()
        {
            LineSeries line = new LineSeries();
            line.Points.AddRange(points);
            line.MarkerType = MarkerType.None;
            line.Title = GraphName;
            return line;
        }
    }
}