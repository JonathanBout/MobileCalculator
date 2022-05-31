using Jace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace MobileCalculator.Pages
{
    public static class ExtraMath
    {
        static readonly char[] ops = { '*', '/', '+', '-', '%' };
        public const double PHI = 1.61803398874989484820458683436;
        public static readonly IReadOnlyDictionary<string, double> constants = new Dictionary<string, double>() { { "PHI", PHI } };
        public static double Root(double A, int N = 2)
        {
            if (N == 2) { return Math.Sqrt(A); }
            double epsilon = 0.00001d;
            double n = N;
            double x = A / n;
            while (Math.Abs(A - Math.Pow(x, N)) > epsilon)
            {
                x = (1.0d / n) * ((n - 1) * x + (A / (Math.Pow(x, N - 1))));
            }
            return x;
        }

        [Obsolete]
        public static string Evaluate(string formula, string? ans = null, string? x = null)
        {
            DataTable table = new DataTable();
            var opsList = new List<char>() { '.', ',' };
            opsList.AddRange(ops);
            string expression = ReplaceMathSymbol(
                ReplaceMathSymbol(
                    formula.TrimEnd(opsList.ToArray())
                    .TrimStart(opsList.ToArray()), "(", "(", afterSymbol: false), ")", ")",
                beforeSymbol: false);

            expression = expression.Replace(")(", ")*(");
            if (!(x is null))
            {
                expression = ReplaceMathSymbol(expression, "X", x);
            }
            if (!(ans is null))
            {
                expression = ReplaceMathSymbol(expression, "ANS", ans);
            }

            try
            {
                return Convert.ToString(table.Compute(expression, ""));
            }catch (System.Exception ex)
            {
                throw new Exception(ex.Message, formula, ex.GetType());
            }
        }

        public static double EvaluateAdvanced(string expression, IDictionary<string, double> vars, params (string, string)[]? replaceValues)
        {
            if (replaceValues != null)
            {
                foreach (var replaceValue in replaceValues)
                {
                    expression = expression.Replace(replaceValue.Item1, replaceValue.Item2);
                }
            }
            if (expression.Count(x => x == '(') >  expression.Count(x=> x == ')'))
            {
                expression += ")";
            }
            var opsList = new List<char>() { '.', ',' };
            opsList.AddRange(ops);
            string formula = ReplaceMathSymbol(
                ReplaceMathSymbol(
                    expression.TrimEnd(opsList.ToArray())
                    .TrimStart(opsList.ToArray()), "(", "(", afterSymbol: false), ")", ")",
                beforeSymbol: false);
            formula = formula.Replace(")(", ")*(").ReplaceMathSymbol("√*(", "sqrt(", false, false);
            CalculationEngine engine = new CalculationEngine();
            foreach (var constant in constants)
            {
                engine.AddConstant(constant.Key, constant.Value);
            }
            return engine.Calculate(formula, vars);
        }

        public static string ReplaceMathSymbol(this string expression, string symbol, string replaceValue,
            bool beforeSymbol = true, bool afterSymbol = true)
        {
            try
            {
                if (!expression.Contains(symbol)) { return expression; }
                string[] parts = expression.Split(new string[] { symbol }, StringSplitOptions.None);
                for (int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i];
                    if (string.IsNullOrEmpty(part.Trim())) { continue; }
                    if (!ops.Any(x => x == part[0]) && i != 0 && afterSymbol && part.First() != '(' && part.First() != ')')
                    {
                        parts[i] = '*' + part;
                    }
                    if (!ops.Any(x => x == part[^1]) && i != parts.Length - 1 && beforeSymbol &&
                        part.Last() != '(' && part.Last() != ')')
                    {
                        parts[i] = part + '*';
                    }
                }
                return string.Join(symbol, parts).Replace(symbol, replaceValue);
            } catch (Exception)
            {
                throw;
            }
        }

        [DebuggerDisplay("{Message | At expression <{Expression}>}")]
        public class Exception : System.Exception
        {
            public string Expression;
            public Type BaseExceptionType;
            public Exception(string message, string expression, Type baseExceptionType) : base (message)
            {
                Expression = expression;
                BaseExceptionType = baseExceptionType;
            }
        }
    }
}