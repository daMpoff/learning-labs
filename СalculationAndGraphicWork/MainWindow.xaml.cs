using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using MathNet.Symbolics;

namespace СalculationAndGraphicWork
{
    public partial class MainWindow : Window
    {
        private List<(double x, double y)> points = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                points.Clear();
                string formula = FormulaBox.Text;
                var expr = Infix.ParseOrThrow(formula);

                double x0 = double.Parse(StartXBox.Text, CultureInfo.InvariantCulture);
                double x1 = double.Parse(EndXBox.Text, CultureInfo.InvariantCulture);
                double step = double.Parse(StepBox.Text, CultureInfo.InvariantCulture);

                for (double x = x0; x <= x1 + 1e-8; x += step)
                {
                    var y = Evaluate.Evaluate(new Dictionary<string, FloatingPoint> { ["x"] = x }, expr).RealValue;
                    points.Add((x, y));
                }

                Plot.Plot.Clear();
                Plot.Plot.Add.Markers(points.Select(p => p.x).ToArray(), points.Select(p => p.y).ToArray());
                Plot.Plot.ShowLegend();
                Plot.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Interpolate_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count < 2)
            {
                MessageBox.Show("Добавьте хотя бы две точки.");
                return;
            }

            if (!double.TryParse(StepBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double funcStep) ||
                funcStep <= 0)
            {
                MessageBox.Show("Некорректный шаг функции.");
                return;
            }

            if (!double.TryParse(InterpStepBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out double interpStep) || interpStep <= 0)
            {
                MessageBox.Show("Некорректный шаг интерполяции.");
                return;
            }

            if (interpStep >= funcStep)
            {
                MessageBox.Show("Шаг интерполяции должен быть меньше шага функции!");
                return;
            }

            double x0 = points.First().x, x1 = points.Last().x;

            DrawAllInterpolators(x0, x1, interpStep);
        }

        private void DrawAllInterpolators(double x0, double x1, double interpStep)
        {
            Plot.Plot.Clear();

            // Рисуем исходные точки
            Plot.Plot.Add.Markers(points.Select(p => p.x).ToArray(), points.Select(p => p.y).ToArray());

            // Рисуем все методы интерполяции
            foreach (var interpolator in InterpolatorsRegistry.AllInterpolators)
            {
                interpolator.SetPoints(points);

                var xs = new List<double>();
                for (double x = x0; x <= x1 + 1e-8; x += interpStep)
                    xs.Add(x);

                foreach (var p in points)
                    if (!xs.Any(xx => Math.Abs(xx - p.x) < 1e-8))
                        xs.Add(p.x);

                xs = xs.OrderBy(v => v).ToList();
                var ys = xs.Select(interpolator.Interpolate).ToArray();

                Plot.Plot.Add.Scatter(xs.ToArray(), ys);
            }

            Plot.Plot.ShowLegend();
            Plot.Refresh();
        }

        private void ClearPlot_Click(object sender, RoutedEventArgs e)
        {
            Plot.Plot.Clear();
            Plot.Refresh();
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count < 2)
            {
                MessageBox.Show("Добавьте хотя бы две точки.");
                return;
            }

            double x0 = points.First().x, x1 = points.Last().x;
            string formula = FormulaBox.Text;
            var expr = Infix.ParseOrThrow(formula);
            Func<double, double> trueFunc = x =>
                Evaluate.Evaluate(new Dictionary<string, FloatingPoint> { ["x"] = x }, expr).RealValue;

            if (!double.TryParse(InterpStepBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out double interpStep) || interpStep <= 0)
            {
                MessageBox.Show("Некорректный шаг интерполяции.");
                return;
            }

            double controlStep = interpStep;
            var results = AnalyzeInterpolators(
                InterpolatorsRegistry.AllInterpolators,
                points,
                trueFunc,
                x0,
                x1,
                controlStep
            );

            ResultsDataGrid.ItemsSource = results;
        }

        private List<InterpolationAnalysisResult> AnalyzeInterpolators(
            List<IInterpolator> interpolators,
            List<(double x, double y)> trainPoints,
            Func<double, double> trueFunction,
            double x0, double x1, double step)
        {
            var controlXs = new List<double>();
            for (double x = x0; x <= x1 + 1e-8; x += step)
                controlXs.Add(x);

            var results = new List<InterpolationAnalysisResult>();
            foreach (var interpolator in interpolators)
            {
                interpolator.SetPoints(trainPoints);
                var errors = new List<double>();
                foreach (var x in controlXs)
                {
                    double interp = interpolator.Interpolate(x);
                    double real = trueFunction(x);
                    errors.Add(interp - real);
                }

                var mse = errors.Select(e => e * e).Average();
                var mae = errors.Select(Math.Abs).Average();
                var maxError = errors.Select(Math.Abs).Max();

                results.Add(new InterpolationAnalysisResult
                {
                    Method = interpolator.Name,
                    MSE = mse,
                    MAE = mae,
                    MaxError = maxError
                });
            }

            return results;
        }
    }
}
