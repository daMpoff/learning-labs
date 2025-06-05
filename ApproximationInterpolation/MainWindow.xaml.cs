using System.Globalization;
using System.Windows;
using MathNet.Symbolics;

namespace ApproximationInterpolation
{
    public partial class MainWindow : Window
    {
        private List<(double x, double y)> points = new();
        private IInterpolator currentInterpolator;

        public MainWindow()
        {
            InitializeComponent();

            MethodComboBox.ItemsSource = InterpolatorsRegistry.AllInterpolators;
            MethodComboBox.DisplayMemberPath = "Name";
            MethodComboBox.SelectedIndex = 0;
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

            currentInterpolator = (IInterpolator)MethodComboBox.SelectedItem;
            currentInterpolator.SetPoints(points);

            double x0 = points.First().x;
            double x1 = points.Last().x;

            // Получаем шаги
            double funcStep, interpStep;
            if (!double.TryParse(StepBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out funcStep) || funcStep <= 0)
            {
                MessageBox.Show("Некорректный шаг функции.");
                return;
            }
            if (!double.TryParse(InterpStepBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out interpStep) || interpStep <= 0)
            {
                MessageBox.Show("Некорректный шаг интерполяции.");
                return;
            }
            if (interpStep >= funcStep)
            {
                MessageBox.Show("Шаг интерполяции должен быть меньше шага функции!");
                return;
            }

            // Генерируем xs для интерполяции по мелкой сетке
            var xs = new List<double>();
            for (double x = x0; x <= x1 + 1e-8; x += interpStep)
                xs.Add(x);

            // Добавляем все узлы (x_i) в xs, если их ещё нет в списке (с учётом погрешности)
            foreach (var p in points)
                if (!xs.Any(xx => Math.Abs(xx - p.x) < 1e-8))
                    xs.Add(p.x);

            // Сортируем xs
            xs = xs.OrderBy(v => v).ToList();

            // Вычисляем значения интерполятора
            var ys = xs.Select(currentInterpolator.Interpolate).ToArray();

            // Очищаем график и рисуем заново
            Plot.Plot.Add.Scatter(xs.ToArray(), ys);
            Plot.Plot.Add.Markers(points.Select(p => p.x).ToArray(), points.Select(p => p.y).ToArray()); // маркеры узлов

            Plot.Refresh();
        }

        private void ClearPlot_Click(object sender, RoutedEventArgs e)
        {
            Plot.Plot.Clear();
            Plot.Refresh();
        }
    }
}
