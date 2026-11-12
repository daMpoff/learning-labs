using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MathNet.Symbolics;

namespace integration
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            MethodBox.ItemsSource = IntegratorsRegistry.AllIntegrators;

            // По умолчанию порядки для Gauss (2..10)
            OrderBox.ItemsSource = Enumerable.Range(2, 9).ToList();
            OrderBox.SelectedIndex = 3; // n=5 по умолчанию

            MethodBox_SelectionChanged(null, null); // Для корректного скрытия OrderBox на старте
        }

        private void MethodBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = MethodBox.SelectedItem;
            if (selected is GaussIntegrator)
            {
                OrderBox.ItemsSource = Enumerable.Range(2, 9).ToList();
                OrderBox.SelectedIndex = 3; // n = 5
                OrderBox.Visibility = Visibility.Visible;
                OrderBoxLabel.Visibility = Visibility.Visible;
            }
            else if (selected is ChebyshevIntegrator)
            {
                OrderBox.ItemsSource = new[] { 3, 4, 5, 6, 7, 9 };
                OrderBox.SelectedIndex = 2; // n = 5
                OrderBox.Visibility = Visibility.Visible;
                OrderBoxLabel.Visibility = Visibility.Visible;
            }
            else
            {
                OrderBox.Visibility = Visibility.Collapsed;
                OrderBoxLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string formula = FunctionBox.Text.Trim();
                double a = double.Parse(ABox.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                double b = double.Parse(BBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                double h = double.Parse(StepBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture);

                if (h <= 0 || b <= a)
                    throw new Exception("Проверьте правильность ввода пределов и шага.");

                var expr = SymbolicExpression.Parse(formula);
                Func<double, double> f = x =>
                {
                    var vars = new Dictionary<string, FloatingPoint> { { "x", x } };
                    var result = expr.Evaluate(vars);
                    return (double)result.RealValue;
                };

                int graphResolution = 400;
                double[] xs = Enumerable.Range(0, graphResolution)
                    .Select(i => a + (b - a) * i / (graphResolution - 1)).ToArray();
                double[] ys = xs.Select(f).ToArray();

                WpfPlot1.Plot.Clear();
                WpfPlot1.Plot.Add.Scatter(xs, ys);

                var integrator = (IIntegrator)MethodBox.SelectedItem;
                double result;

                if (integrator is GaussIntegrator)
                {
                    int order = (int)OrderBox.SelectedItem;
                    var gauss = new GaussIntegrator(order);
                    result = gauss.Integrate(f, a, b, h);
                    gauss.Visualize(WpfPlot1.Plot, f, a, b, h);
                }
                else if (integrator is ChebyshevIntegrator)
                {
                    int order = (int)OrderBox.SelectedItem;
                    var cheb = new ChebyshevIntegrator();
                    result = cheb.Integrate(f, a, b, h);
                    cheb.Visualize(WpfPlot1.Plot, f, a, b, h);
                }
                else if (integrator is CubicSplineIntegrator splineIntegrator)
                {
                    result = splineIntegrator.Integrate(f, a, b, h, formula);
                    splineIntegrator.Visualize(WpfPlot1.Plot, f, a, b, h);
                }
                else
                {
                    result = integrator.Integrate(f, a, b, h);
                    integrator.Visualize(WpfPlot1.Plot, f, a, b, h);
                }

                WpfPlot1.Plot.ShowLegend();
                WpfPlot1.Plot.Title($"f(x) = {formula}");
                double minY = ys.Min();
                double maxY = ys.Max();
                WpfPlot1.Plot.Axes.SetLimits(a - 0.1, b + 0.1, minY - 0.2 * Math.Abs(minY), maxY + 0.2 * Math.Abs(maxY));

                ResultBlock.Text = $"S ≈ {result:F5}";

                WpfPlot1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
