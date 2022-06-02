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
        int minX = 0;
        int maxX = 10;
        public GraphCalculator()
        {
            InitializeComponent();
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Evaluate();
        }

        private void ViewSettings()
        {
            //Navigation.PushModalAsync(); <=== view settings page
            Thread.Sleep(1000);
            Navigation.PopModalAsync();
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
            Evaluate();
        }

        public void SetChart(params Series[] ChartEntries)
        {
            PlotModel model = new PlotModel()
            {
                PlotType = PlotType.XY,
                Title = "Result",
                TextColor = OxyColor.FromRgb(255, 255, 255),
            };
            model.Axes.Add(new LinearAxis() { AbsoluteMaximum = maxX, AbsoluteMinimum = minX }) ;
            foreach (Series chartEntry in ChartEntries)
            {
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
            line.Color = OxyColor.FromRgb(255, 255, 255);
            return line;
        }
    }
}