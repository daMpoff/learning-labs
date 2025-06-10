using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MathNet.Symbolics;

namespace NumericalDifferentiation
{
    public partial class MainWindow : Window
    {
        private readonly List<(double x, double y)> userPoints = new();
        private List<(double x, double y)> funcPoints = new();

        public MainWindow()
        {
            InitializeComponent();

            PointsDiffMethodComboBox.ItemsSource = DiscreteDifferentiatorRegistry.All;
            PointsDiffMethodComboBox.DisplayMemberPath = "Name";
            PointsDiffMethodComboBox.SelectedIndex = 0;
        }

        private void InputModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool funcMode = ((InputModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Функция");
            if (FunctionPanel != null)
                FunctionPanel.Visibility = funcMode ? Visibility.Visible : Visibility.Collapsed;
            if (PointsPanel != null)
                PointsPanel.Visibility = funcMode ? Visibility.Collapsed : Visibility.Visible;
            if (DiffMethodPanel != null)
                DiffMethodPanel.Visibility = funcMode ? Visibility.Visible : Visibility.Collapsed;
            UpdatePointsUI();
        }

        private void AddPointButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(XInputBox?.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                !double.TryParse(YInputBox?.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
            {
                MessageBox.Show("Некорректный ввод координат!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (userPoints.Any(p => Math.Abs(p.x - x) < 1e-10))
            {
                MessageBox.Show($"Точка с x = {x} уже есть!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            userPoints.Add((x, y));
            if (XInputBox != null) XInputBox.Text = "";
            if (YInputBox != null) YInputBox.Text = "";
            UpdatePointsUI();
        }

        private void RemovePointButton_Click(object sender, RoutedEventArgs e)
        {
            int idx = PointsListBox?.SelectedIndex ?? -1;
            if (idx >= 0 && idx < userPoints.Count)
                userPoints.RemoveAt(idx);
            UpdatePointsUI();
        }

        private void DifferenceMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DifferenceMethodComboBox.SelectedItem is ComboBoxItem item &&
                item.Content?.ToString() == "Любого порядка")
            {
                if (OrderPanel != null)
                    OrderPanel.Visibility = Visibility.Visible;
            }
            else
            {
                if (OrderPanel != null)
                    OrderPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdatePointsUI()
        {
            if (InputModeComboBox == null || PointsListBox == null)
                return;

            bool isFunc = ((InputModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Функция");

            if (isFunc)
            {
                if (FormulaBox == null || StartXBox == null || EndXBox == null || StepBox == null)
                    return;
                try
                {
                    string formula = FormulaBox.Text.Trim();
                    var expr = Infix.ParseOrThrow(formula);
                    double x0 = double.Parse(StartXBox.Text, CultureInfo.InvariantCulture);
                    double x1 = double.Parse(EndXBox.Text, CultureInfo.InvariantCulture);
                    double step = double.Parse(StepBox.Text, CultureInfo.InvariantCulture);

                    funcPoints = new List<(double x, double y)>();
                    for (double x = x0; x <= x1 + 1e-8; x += step)
                        funcPoints.Add((x,
                            Evaluate.Evaluate(new Dictionary<string, FloatingPoint> { ["x"] = x }, expr).RealValue));
                }
                catch
                {
                    funcPoints.Clear();
                }
            }
            else
            {
                var sorted = userPoints.OrderBy(p => p.x).ToList();
                PointsListBox.ItemsSource = sorted.Select(p =>
                        $"{p.x.ToString(CultureInfo.InvariantCulture)}; {p.y.ToString(CultureInfo.InvariantCulture)}")
                    .ToList();
            }
        }

        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {
            UpdatePointsUI();

            Plot.Plot.Clear();

            bool isFunc = ((InputModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Функция");
            if (isFunc)
            {
                if (funcPoints.Count == 0)
                {
                    MessageBox.Show("Нет точек для построения.");
                    return;
                }

                Plot.Plot.Add.ScatterPoints(funcPoints.Select(p => p.x).ToArray(),
                    funcPoints.Select(p => p.y).ToArray(), color: ScottPlot.Color.FromHex("#4a90e2"));
                Plot.Plot.Title("Значения функции по формуле");
            }
            else
            {
                var sorted = userPoints.OrderBy(p => p.x).ToList();
                if (sorted.Count == 0)
                {
                    MessageBox.Show("Список точек пуст!");
                    return;
                }

                Plot.Plot.Add.ScatterPoints(sorted.Select(p => p.x).ToArray(), sorted.Select(p => p.y).ToArray(),
                    color: ScottPlot.Color.FromHex("#FF8000"));
                Plot.Plot.Title("Введённые точки");
            }

            Plot.Plot.ShowLegend();
            Plot.Refresh();
        }

        private double EvaluateFunc(string formula, double x)
        {
            var expr = Infix.ParseOrThrow(formula);
            return Evaluate.Evaluate(new Dictionary<string, FloatingPoint> { ["x"] = x }, expr).RealValue;
        }

        private void DiffButton_Click(object sender, RoutedEventArgs e)
        {
            string mode = (InputModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            // Проверяем ввод x
            if (string.IsNullOrWhiteSpace(DiffXInputBox?.Text))
            {
                DiffResultTextBlock.Text = "Введите точку для производной!";
                return;
            }

            if (!double.TryParse(DiffXInputBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
            {
                DiffResultTextBlock.Text = "Некорректное значение x!";
                return;
            }

            double h = 0;

            // === Режим "Функция" ===
            if (mode == "Функция")
            {
                if (FormulaBox == null || StepBox == null)
                {
                    DiffResultTextBlock.Text = "Нет данных для вычисления!";
                    return;
                }

                h = double.Parse(StepBox.Text, CultureInfo.InvariantCulture);
                string formula = FormulaBox.Text.Trim();
                var expr = Infix.ParseOrThrow(formula);
                Func<double, double> f = val =>
                    Evaluate.Evaluate(new Dictionary<string, FloatingPoint> { ["x"] = val }, expr).RealValue;

                string method = (DifferenceMethodComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                double result = 0;

                if (method == "Левая разность")
                    result = FiniteDifferenceRegistry.Left(f(x), f(x + h), h);
                else if (method == "Правая разность")
                    result = FiniteDifferenceRegistry.Right(f(x), f(x + h), h);
                else if (method == "Центральная разность")
                    result = FiniteDifferenceRegistry.Central(f(x - h), f(x + h), h);
                else if (method == "Любого порядка")
                {
                    if (OrderInputBox == null || !int.TryParse(OrderInputBox.Text, out int order) || order < 1)
                    {
                        DiffResultTextBlock.Text = "Некорректный порядок!";
                        return;
                    }

                    result = FiniteDifferenceRegistry.HigherOrderDerivative(f, x, h, order);
                }
                else
                {
                    DiffResultTextBlock.Text = "Выберите метод!";
                    return;
                }

                DiffResultTextBlock.Text = $"{method}: {result:F6}";

                // Визуализация
                Plot.Plot.Clear();
                if (funcPoints.Count > 0)
                {
                    Plot.Plot.Add.ScatterPoints(funcPoints.Select(p => p.x).ToArray(),
                        funcPoints.Select(p => p.y).ToArray(),
                        color: ScottPlot.Color.FromHex("#4a90e2"));
                }

                Plot.Plot.Add.VerticalLine(x, color: ScottPlot.Color.FromHex("#E2854A"));
                Plot.Plot.Title($"Производная в x={x}");
                Plot.Plot.ShowLegend();
                Plot.Refresh();
                return;
            }

            // === Режим "Точки" ===
            var sorted = userPoints.OrderBy(p => p.x).ToList();

            // Проверяем метод для точек
            if (PointsDiffMethodComboBox.SelectedItem is not IDiscreteDifferentiator differentiator)
            {
                DiffResultTextBlock.Text = "Выберите метод!";
                return;
            }

            // --- Для МНК выбираем нужное количество ближайших точек ---
            int numPoints = sorted.Count;
            if (differentiator is UndeterminedCoefficientsDifferentiator)
            {
                if (!int.TryParse(PointsCountBox.Text, out numPoints) || numPoints < differentiator.RequiredPoints)
                    numPoints = differentiator.RequiredPoints;
                if (numPoints > sorted.Count)
                    numPoints = sorted.Count;
                sorted = sorted
                    .OrderBy(p => Math.Abs(p.x - x))
                    .Take(numPoints)
                    .OrderBy(p => p.x)
                    .ToList();
            }
            // ----------------------------------------------------------

            // Достаточно точек для метода?
            if (sorted.Count < differentiator.RequiredPoints)
            {
                DiffResultTextBlock.Text =
                    $"Для метода \"{differentiator.Name}\" нужно минимум {differentiator.RequiredPoints} точек!";
                return;
            }

            // Проверка что x попадает в диапазон
            double minX = sorted.First().x;
            double maxX = sorted.Last().x;
            if (x < minX || x > maxX)
            {
                DiffResultTextBlock.Text = $"Точка x={x} вне диапазона ({minX} ... {maxX})!";
                return;
            }

            try
            {
                var usedIndices = new List<int>();
                double result = differentiator.Differentiate(sorted, x, out usedIndices);

                // Визуализация: все точки и выделенные используемые
                Plot.Plot.Clear();
                // Все точки (оранжевым)
                Plot.Plot.Add.ScatterPoints(sorted.Select(p => p.x).ToArray(), sorted.Select(p => p.y).ToArray(),
                    color: ScottPlot.Color.FromHex("#FF8000"));
                // Используемые (красным)
                var usedPoints = usedIndices.Select(i => sorted[i]).ToList();
                if (usedPoints.Count > 0)
                {
                    Plot.Plot.Add.ScatterPoints(usedPoints.Select(p => p.x).ToArray(),
                        usedPoints.Select(p => p.y).ToArray(),
                        color: ScottPlot.Color.FromHex("#C30000"));
                }

                Plot.Plot.Title($"Производная в x={x}");
                Plot.Plot.ShowLegend();
                Plot.Refresh();

                DiffResultTextBlock.Text = $"dY/dX({x}) ≈ {result:F6} \nМетод: {differentiator.Name}";
            }
            catch (Exception ex)
            {
                DiffResultTextBlock.Text = $"Ошибка: {ex.Message}";
            }
        }
    }
}
