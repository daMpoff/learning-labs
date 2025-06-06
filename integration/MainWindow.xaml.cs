using System.Globalization;
using System.Windows;
using MathNet.Symbolics;

namespace integration;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        // Заполняем ComboBox методами из Registry
        MethodBox.ItemsSource = IntegratorsRegistry.AllIntegrators;
        MethodBox.DisplayMemberPath = "Name";
        MethodBox.SelectedIndex = 0;
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

        // График функции
        int graphResolution = 400;
        double[] xs = Enumerable.Range(0, graphResolution)
            .Select(i => a + (b - a) * i / (graphResolution - 1)).ToArray();
        double[] ys = xs.Select(f).ToArray();

        WpfPlot1.Plot.Clear();
        WpfPlot1.Plot.Add.Scatter(xs, ys);

        // --- Проверяем тип интегратора ---
        var integrator = (IIntegrator)MethodBox.SelectedItem;
        double result;

        if (integrator is CubicSplineIntegrator splineIntegrator)
        {
            // У этого класса есть перегрузка с формулой
            result = splineIntegrator.Integrate(f, a, b, h, formula);
        }
        else
        {
            // Обычный вызов
            result = integrator.Integrate(f, a, b, h);
        }

        // Визуализация (у всех одинаково)
        integrator.Visualize(WpfPlot1.Plot, f, a, b, h);

        // Настройки осей и подписи
        WpfPlot1.Plot.ShowLegend();
        WpfPlot1.Plot.Title($"f(x) = {formula}");
        double minY = ys.Min();
        double maxY = ys.Max();
        WpfPlot1.Plot.Axes.SetLimits(a - 0.1, b + 0.1, minY - 0.2 * Math.Abs(minY), maxY + 0.2 * Math.Abs(maxY));

        // Результат
        ResultBlock.Text = $"S ≈ {result:F5}";

        WpfPlot1.Refresh();
    }
    catch (Exception ex)
    {
        MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
}
