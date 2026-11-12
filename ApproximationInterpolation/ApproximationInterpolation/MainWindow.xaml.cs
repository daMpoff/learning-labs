using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MathNet.Symbolics;

namespace ApproximationInterpolation
{
    public partial class MainWindow : Window
    {
        private List<(double x, double y)> points = new();

        public MainWindow()
        {
            InitializeComponent();

            InterpolationComboBox.ItemsSource = InterpolatorsRegistry.AllInterpolators;
            InterpolationComboBox.SelectedIndex = 0;

            ApproximationComboBox.ItemsSource = ApproximatorsRegistry.AllApproximators;
            ApproximationComboBox.SelectedIndex = 0;

            ModeComboBox.SelectionChanged += ModeComboBox_SelectionChanged;
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Показываем нужный ComboBox
            if (ModeComboBox.SelectedIndex == 0)
            {
                InterpGrid.Visibility = Visibility.Visible;
                ApproxGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                InterpGrid.Visibility = Visibility.Collapsed;
                ApproxGrid.Visibility = Visibility.Visible;
            }
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

            if (!int.TryParse(DegreeBox.Text, out int degree) || degree < 1)
            {
                MessageBox.Show("Степень полинома должна быть положительным числом.");
                return;
            }

            if (!double.TryParse(StepBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double funcStep) || funcStep <= 0)
            {
                MessageBox.Show("Некорректный шаг функции.");
                return;
            }
            if (!double.TryParse(InterpStepBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double interpStep) || interpStep <= 0)
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

            if (ModeComboBox.SelectedIndex == 0) // Интерполяция
            {
                var selectedInterpolator = InterpolationComboBox.SelectedItem as IInterpolator;
                // Если выбран какой-то интерполятор, который зависит от степени — пересоздай его (например, если добавишь МНК-интерполятор)
                var currentInterpolator = selectedInterpolator;
                currentInterpolator.SetPoints(points);
                DrawPlot(currentInterpolator.Interpolate, x0, x1, interpStep);
            }
            else // Аппроксимация
            {
                var selectedApproximator = ApproximationComboBox.SelectedItem as IApproximator;
                IApproximator currentApproximator = null;
                // Для каждого класса аппроксиматора — пересоздай с правильными параметрами
                if (selectedApproximator is LeastSquaresPolynomialApproximator)
                    currentApproximator = new LeastSquaresPolynomialApproximator(degree);
                else if (selectedApproximator is TaylorApproximator)
                    currentApproximator = new TaylorApproximator(FormulaBox.Text, degree);
                else if (selectedApproximator is ChebyshevApproximator)
                    currentApproximator = new ChebyshevApproximator(
                        FormulaBox.Text,
                        degree,
                        double.Parse(StartXBox.Text, CultureInfo.InvariantCulture),
                        double.Parse(EndXBox.Text, CultureInfo.InvariantCulture)
                    );
                else if (selectedApproximator is FourierApproximator)
                    currentApproximator = new FourierApproximator(FormulaBox.Text, degree);
                else
                    currentApproximator = selectedApproximator;

                currentApproximator.SetPoints(points);
                DrawPlot(currentApproximator.Approximate, x0, x1, interpStep);
            }
        }

        private void DrawPlot(Func<double, double> func, double x0, double x1, double interpStep)
        {
            var xs = new List<double>();
            for (double x = x0; x <= x1 + 1e-8; x += interpStep)
                xs.Add(x);

            // Добавляем все узлы (x_i) в xs, если их ещё нет в списке (с учётом погрешности)
            foreach (var p in points)
                if (!xs.Any(xx => Math.Abs(xx - p.x) < 1e-8))
                    xs.Add(p.x);

            xs = xs.OrderBy(v => v).ToList();
            var ys = xs.Select(func).ToArray();

            Plot.Plot.Clear();
            Plot.Plot.Add.Scatter(xs.ToArray(), ys);
            Plot.Plot.Add.Markers(points.Select(p => p.x).ToArray(), points.Select(p => p.y).ToArray());
            Plot.Refresh();
        }

        private void ClearPlot_Click(object sender, RoutedEventArgs e)
        {
            Plot.Plot.Clear();
            Plot.Refresh();
        }
    }
}
