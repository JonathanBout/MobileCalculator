using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileCalculator.Pages
{
    public partial class GenericCalculator : ContentPage
    {
        readonly Color defaultCol;
        bool isClear = true;
        bool isAnswer = false;
        double previousResult = 0;
        double currentResult { get; set; } = 0;
        private List<string> typedExpression = new List<string>();
        public string Expression
        {
            get
            {
                return string.Join("", typedExpression);
            } 
        }
        public string LastExpression { get; private set; } = "0";
        public GenericCalculator()
        {
            InitializeComponent();
            defaultCol = FormulaField.TextColor;
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
            SetViewStackOrientation(DeviceDisplay.MainDisplayInfo.Orientation);
            Clear();
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            SetViewStackOrientation(e.DisplayInfo.Orientation);
        }

        void SetViewStackOrientation(DisplayOrientation orientation)
        {
            KeyboardStack.Orientation = orientation == DisplayOrientation.Portrait ? StackOrientation.Vertical : StackOrientation.Horizontal;
            Spacer.IsVisible = orientation == DisplayOrientation.Portrait;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            GetInput(button);
        }

        public void GetInput(Button sender)
        {
            FormulaField.TextColor = defaultCol;
            string expression = Expression;
            if (isAnswer || isClear)
            {
                Clear(false);
                /*isAnswer = !(*/
                isClear = true;//);
                PreviousValueField.Text = currentResult.ToString();
            }
            switch (sender.Text)
            {
                case "C":
                    isAnswer = false;
                    Clear();
                    isClear = true;
                    break;
                case "DEL":
                    isAnswer = false;
                    RemoveLastText();
                    break;
                case "=":
                    if (isAnswer)
                    {
                        expression = LastExpression;
                    }
                    previousResult = currentResult;
                    PreviousValueField.Text = previousResult.ToString();
                    try
                    {
                        currentResult = ExtraMath.EvaluateAdvanced(expression, new Dictionary<string, double>() { { "ANS", previousResult} }, ("π", "PI"), ("φ", "PHI"));
                        Clear(false);
                        AddText(currentResult);
                    }catch (Exception ex)
                    {
                        DisplayAlert("Error", $"Operation failed: {ex.Message}", "Ok");
                        return;
                    }
                    isAnswer = true;
                    LastExpression = expression;
                    break;
                default:
                    isAnswer = false;
                    isClear = false;
                    AddText(sender.Text);
                    break;
            }
        }

        public void AddText(string text)
        {
            typedExpression.Add(text);
            Update();
        }

        public void AddText(object text)
        {
            AddText(text.ToString());
        }

        void Update()
        {
            FormulaField.Text = string.Join("", typedExpression);
        }

        public void RemoveLastText()
        {
            if (typedExpression is null || typedExpression.Count < 1)
            {
                typedExpression = new List<string>() { "0" };
                Update();
                return;
            }
            typedExpression = typedExpression.ToArray()[0..^1].ToList();
            if (typedExpression.Count < 1)
            {
                Clear();
                isClear = true;
            }
            Update();
        }

        public void Clear(bool addZero = true)
        {
            if (addZero)
                typedExpression = new List<string>() { "0" };
            else
                typedExpression = new List<string>();
            Update();
        }
    }
}