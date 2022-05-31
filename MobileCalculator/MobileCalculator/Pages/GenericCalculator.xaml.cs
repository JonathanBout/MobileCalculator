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
        double previousCalculation = 0;
        double currentCalculation = 0;
        private List<string> typedExpression = new List<string>();
        public GenericCalculator()
        {
            InitializeComponent();
            defaultCol = FormulaField.TextColor;
            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
            SetViewStackOrientation(DeviceDisplay.MainDisplayInfo.Orientation);
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            SetViewStackOrientation(e.DisplayInfo.Orientation);
        }

        void SetViewStackOrientation(DisplayOrientation orientation)
        {
            KeyboardStack.Orientation = orientation == DisplayOrientation.Portrait ? StackOrientation.Vertical : StackOrientation.Horizontal;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            FormulaField.TextColor = defaultCol;
            if (isClear)
            {
                Clear(false);
                isClear = false;
            }
            if (isAnswer)
            {
                isAnswer = false;
                if (button.Text == "=")
                {
                    Clear(false);
                }
            }
            switch (button.Text)
            {
                case "C":
                    Clear();
                    isClear = true;
                    break;
                case "DEL":
                    if (FormulaField.Text.Length > 0 & !isClear)
                        RemoveLastText();
                    break;
                case "=":
                    try
                    {
                        //FormulaField.Text = ExtraMath.Evaluate(FormulaField.Text, PreviousValueField.Text);
                        previousCalculation = currentCalculation;
                        PreviousValueField.Text = previousCalculation.ToString();
                        currentCalculation = ExtraMath.EvaluateAdvanced(string.Join("", typedExpression), new Dictionary<string, double> { { "ANS", previousCalculation} });
                        Clear(false);
                        AddText(currentCalculation.ToString());
                        isAnswer = true;
                    }catch (Exception ex)
                    {
                        FormulaField.TextColor = Color.Red;
                        Clear(false);
                        AddText(ex.Message);
                        isAnswer = true;
                    }
                    break;
                default:
                    AddText(button.Text);
                    break;
            }
        }

        public void AddText(string text)
        {
            typedExpression.Add(text);
            Update();
        }

        void Update()
        {
            FormulaField.Text = string.Join("", typedExpression);
        }

        public void RemoveLastText()
        {
            typedExpression = typedExpression.ToArray()[0..^1].ToList();
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