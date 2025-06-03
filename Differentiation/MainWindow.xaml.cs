using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using MathNet.Symbolics;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace Differentiation
{
    public partial class MainWindow : Window
    {
        private List<System.Windows.Controls.TextBox> _initConditionBoxes = new();

        public MainWindow()
        {
            InitializeComponent();
            GenerateInitialConditionFields();
            OrderTextBox.TextChanged += (s, e) => GenerateInitialConditionFields();
        }

        private void GenerateInitialConditionFields()
        {
            if (!int.TryParse(OrderTextBox.Text, out int order) || order < 1) order = 1;
            InitialConditionsPanel.Children.Clear();
            _initConditionBoxes.Clear();

            for (int i = 0; i < order; i++)
            {
                var panel = new System.Windows.Controls.StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    Margin = new Thickness(4, 2, 4, 2),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var label = new System.Windows.Controls.TextBlock
                {
                    Text = $"y{i}₀",
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 28,
                    TextAlignment = System.Windows.TextAlignment.Right
                };
                var tb = new System.Windows.Controls.TextBox
                {
                    Width = 60,
                    Text = (i == 0 ? "1" : "0"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                _initConditionBoxes.Add(tb);
                panel.Children.Add(label);
                panel.Children.Add(tb);
                InitialConditionsPanel.Children.Add(panel);
            }
        }

        private enum OdeMethod
        {
            Euler,
            EulerCauchy,
            EulerIterative,
            EulerImproved,
            RungeKutta2,
            RungeKutta3,
            RungeKutta4,
            AdamsBashforth2,
            AdamsBashforth3,
            AdamsBashforth4,
            AdamsMoulton2
        }
        private OdeMethod SelectedMethod => (OdeMethod)MethodComboBox.SelectedIndex;

        private string SelectedMethodName =>
            MethodComboBox.SelectedIndex switch
            {
                0 => "Эйлер",
                1 => "Эйлер-Коши",
                2 => "Эйлер-Итерации",
                3 => "Улучшенный Эйлер",
                4 => "РК2",
                5 => "РК3",
                6 => "РК4",
                7 => "Адамс-Башфорт 2",
                8 => "Адамс-Башфорт 3",
                9 => "Адамс-Башфорт 4",
                10 => "Адамс-Мултон 2",
                _ => "Неизвестно"
            };


        private void SolveODE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int order = int.Parse(OrderTextBox.Text);
                double start = double.Parse(StartTextBox.Text, CultureInfo.InvariantCulture);
                double end = double.Parse(EndTextBox.Text, CultureInfo.InvariantCulture);
                double step = double.Parse(StepTextBox.Text, CultureInfo.InvariantCulture);

                List<double> init = new();
                foreach (var tb in _initConditionBoxes)
                    init.Add(double.Parse(tb.Text, CultureInfo.InvariantCulture));

                var rhs = SymbolicExpression.Parse(RhsTextBox.Text);

                int N = (int)Math.Ceiling((end - start) / step) + 1;
                var result = new Dictionary<string, List<double>>();
                for (int i = 0; i < order; i++)
                    result[$"y{i}"] = new List<double>(N) { init[i] };
                var tList = new List<double>(N) { start };

                switch (SelectedMethod)
                {
                    case OdeMethod.Euler:
                        SolveEuler(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.EulerCauchy:
                        SolveEulerCauchy(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.EulerIterative:
                        SolveEulerIterative(order, start, end, step, init, rhs, result, tList, 4); // 4 итерации
                        break;
                    case OdeMethod.EulerImproved:
                        SolveEulerImproved(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.RungeKutta2:
                        SolveRungeKutta2(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.RungeKutta3:
                        SolveRungeKutta3(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.RungeKutta4:
                        SolveRungeKutta4(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.AdamsBashforth2:
                        SolveAdamsBashforth2(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.AdamsBashforth3:
                        SolveAdamsBashforth3(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.AdamsBashforth4:
                        SolveAdamsBashforth4(order, start, end, step, init, rhs, result, tList);
                        break;
                    case OdeMethod.AdamsMoulton2:
                        SolveAdamsMoulton2(order, start, end, step, init, rhs, result, tList);
                        break;
                }


                AddMethodPlot(order, tList, result, SelectedMethodName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void SolveAdamsMoulton2(int order, double start, double end, double step, List<double> init, SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;

            // Первый шаг — Эйлер
            double[] k1 = Derivative(order, y, rhs);
            for (int i = 0; i < order; i++)
                y[i] += step * k1[i];
            for (int i = 0; i < order; i++)
                result[$"y{i}"].Add(y[i]);
            t += step;
            tList.Add(t);

            // Прогноз-Корректор
            for (int k = 2; k < N; k++)
            {
                double[] yPred = new double[order];
                for (int i = 0; i < order; i++)
                    yPred[i] = y[i] + step * Derivative(order, y, rhs)[i];
                double[] fCur = Derivative(order, y, rhs);
                double[] fPred = Derivative(order, yPred, rhs);
                for (int i = 0; i < order; i++)
                    y[i] += step / 2.0 * (fCur[i] + fPred[i]);
                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }
        private void SolveAdamsBashforth4(int order, double start, double end, double step, List<double> init, SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            var vars = new Dictionary<string, FloatingPoint>();
            // Начальные точки (РК4)
            var dys = new List<double[]>();
            for (int m = 0; m < 3; m++)
            {
                double[] k1 = Derivative(order, y, rhs);

                double[] yk2 = new double[order];
                for (int i = 0; i < order; i++)
                    yk2[i] = y[i] + 0.5 * step * k1[i];
                double[] k2 = Derivative(order, yk2, rhs);

                double[] yk3 = new double[order];
                for (int i = 0; i < order; i++)
                    yk3[i] = y[i] + 0.5 * step * k2[i];
                double[] k3 = Derivative(order, yk3, rhs);

                double[] yk4 = new double[order];
                for (int i = 0; i < order; i++)
                    yk4[i] = y[i] + step * k3[i];
                double[] k4 = Derivative(order, yk4, rhs);

                for (int i = 0; i < order; i++)
                    y[i] += step / 6.0 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
                dys.Add(k1);
            }
            dys.Add(Derivative(order, y, rhs));

            for (int k = 4; k < N; k++)
            {
                var f1 = dys[3];
                var f2 = dys[2];
                var f3 = dys[1];
                var f4 = dys[0];
                for (int i = 0; i < order; i++)
                    y[i] += step * (55 * f1[i] - 59 * f2[i] + 37 * f3[i] - 9 * f4[i]) / 24.0;
                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
                dys.RemoveAt(0);
                dys.Add(Derivative(order, y, rhs));
            }
        }


        private void SolveRungeKutta3(int order, double start, double end, double step, List<double> init, SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            var vars = new Dictionary<string, FloatingPoint>();

            for (int k = 1; k < N; k++)
            {
                // k1
                double[] k1 = Derivative(order, y, rhs);

                // k2
                double[] yk2 = new double[order];
                for (int i = 0; i < order; i++)
                    yk2[i] = y[i] + step / 3.0 * k1[i];
                double[] k2 = Derivative(order, yk2, rhs);

                // k3
                double[] yk3 = new double[order];
                for (int i = 0; i < order; i++)
                    yk3[i] = y[i] + 2 * step / 3.0 * k2[i];
                double[] k3 = Derivative(order, yk3, rhs);

                for (int i = 0; i < order; i++)
                    y[i] += step / 4.0 * (k1[i] + 3 * k3[i]);

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }
        private void SolveAdamsBashforth3(int order, double start, double end, double step, List<double> init, SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            var vars = new Dictionary<string, FloatingPoint>();
            // Начальные точки (получаем РК3)
            var dys = new List<double[]>();
            for (int m = 0; m < 2; m++)
            {
                double[] k1 = Derivative(order, y, rhs);
                double[] yk2 = new double[order];
                for (int i = 0; i < order; i++)
                    yk2[i] = y[i] + step / 3.0 * k1[i];
                double[] k2 = Derivative(order, yk2, rhs);
                double[] yk3 = new double[order];
                for (int i = 0; i < order; i++)
                    yk3[i] = y[i] + 2 * step / 3.0 * k2[i];
                double[] k3 = Derivative(order, yk3, rhs);
                for (int i = 0; i < order; i++)
                    y[i] += step / 4.0 * (k1[i] + 3 * k3[i]);
                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
                dys.Add(k1);
            }
            dys.Add(Derivative(order, y, rhs));

            for (int k = 3; k < N; k++)
            {
                var f1 = dys[2];
                var f2 = dys[1];
                var f3 = dys[0];
                for (int i = 0; i < order; i++)
                    y[i] += step * (23 * f1[i] - 16 * f2[i] + 5 * f3[i]) / 12.0;
                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
                dys.RemoveAt(0);
                dys.Add(Derivative(order, y, rhs));
            }
        }

        private void SolveEulerImproved(int order, double start, double end, double step, List<double> init, SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            var vars = new Dictionary<string, FloatingPoint>();

            for (int k = 1; k < N; k++)
            {
                // Считаем значение в середине интервала
                double[] yMid = new double[order];
                for (int i = 0; i < order - 1; i++)
                    yMid[i] = y[i] + step / 2 * y[i + 1];
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = y[i];
                double f = rhs.Evaluate(vars).RealValue;
                yMid[order - 1] = y[order - 1] + step / 2 * f;

                // Теперь шаг на полный интервал с учетом серединного значения
                for (int i = 0; i < order - 1; i++)
                    y[i] = y[i] + step * yMid[i + 1];
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = yMid[i];
                double fMid = rhs.Evaluate(vars).RealValue;
                y[order - 1] = y[order - 1] + step * fMid;

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }

        private void SolveEulerIterative(int order, double start, double end, double step, List<double> init, SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList, int iters)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            var vars = new Dictionary<string, FloatingPoint>();

            for (int k = 1; k < N; k++)
            {
                double[] yNew = new double[order];
                double[] yOld = new double[order];
                // Первый предиктор (Эйлер)
                for (int i = 0; i < order - 1; i++)
                    yNew[i] = y[i] + step * y[i + 1];
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = y[i];
                double f = rhs.Evaluate(vars).RealValue;
                yNew[order - 1] = y[order - 1] + step * f;

                // Итерации
                for (int iter = 1; iter < iters; iter++)
                {
                    Array.Copy(yNew, yOld, order);
                    for (int i = 0; i < order - 1; i++)
                        yNew[i] = y[i] + step / 2 * (y[i + 1] + yOld[i + 1]);
                    for (int i = 0; i < order; i++)
                        vars[$"y{i}"] = yOld[i];
                    double fOld = rhs.Evaluate(vars).RealValue;
                    for (int i = 0; i < order; i++)
                        vars[$"y{i}"] = yNew[i];
                    double fNew = rhs.Evaluate(vars).RealValue;
                    yNew[order - 1] = y[order - 1] + step / 2 * (f + fNew);
                }

                Array.Copy(yNew, y, order);

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }

        private void SolveEulerCauchy(int order, double start, double end, double step, List<double> init, SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            var vars = new Dictionary<string, FloatingPoint>();

            for (int k = 1; k < N; k++)
            {
                // Predictor (обычный Эйлер)
                double[] yPred = new double[order];
                for (int i = 0; i < order - 1; i++)
                    yPred[i] = y[i] + step * y[i + 1];
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = y[i];
                double f = rhs.Evaluate(vars).RealValue;
                yPred[order - 1] = y[order - 1] + step * f;

                // Corrector (усреднение правой части на концах отрезка)
                for (int i = 0; i < order - 1; i++)
                    y[i] = y[i] + step / 2 * (y[i + 1] + yPred[i + 1]);
                // Для последнего: усредняем правую часть f
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = yPred[i];
                double fPred = rhs.Evaluate(vars).RealValue;
                y[order - 1] = y[order - 1] + step / 2 * (f + fPred);

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }


        // Явный Эйлер
        private void SolveEuler(
            int order, double start, double end, double step, List<double> init,
            SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            var vars = new Dictionary<string, FloatingPoint>();
            int N = (int)Math.Ceiling((end - start) / step) + 1;

            for (int k = 1; k < N; k++)
            {
                double[] yNext = new double[order];
                for (int i = 0; i < order - 1; i++)
                    yNext[i] = y[i] + step * y[i + 1];

                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = y[i];
                double f = rhs.Evaluate(vars).RealValue;
                yNext[order - 1] = y[order - 1] + step * f;

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(yNext[i]);
                t += step;
                tList.Add(t);
                Array.Copy(yNext, y, order);
            }
        }

        // Рунге-Кутта 2 порядка (модифицированный Эйлер/метод средних)
        private void SolveRungeKutta2(
            int order, double start, double end, double step, List<double> init,
            SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;

            for (int k = 1; k < N; k++)
            {
                double[] k1 = Derivative(order, y, rhs);
                double[] yk2 = new double[order];
                for (int i = 0; i < order; i++)
                    yk2[i] = y[i] + step * k1[i];

                double[] k2 = Derivative(order, yk2, rhs);

                for (int i = 0; i < order; i++)
                    y[i] += step / 2.0 * (k1[i] + k2[i]);

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }

        // Рунге-Кутта 4 порядка
        private void SolveRungeKutta4(
            int order, double start, double end, double step, List<double> init,
            SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;

            for (int k = 1; k < N; k++)
            {
                double[] yPrev = (double[])y.Clone();

                // k1
                double[] k1 = Derivative(order, yPrev, rhs);

                // k2
                double[] yk2 = new double[order];
                for (int i = 0; i < order; i++)
                    yk2[i] = yPrev[i] + 0.5 * step * k1[i];
                double[] k2 = Derivative(order, yk2, rhs);

                // k3
                double[] yk3 = new double[order];
                for (int i = 0; i < order; i++)
                    yk3[i] = yPrev[i] + 0.5 * step * k2[i];
                double[] k3 = Derivative(order, yk3, rhs);

                // k4
                double[] yk4 = new double[order];
                for (int i = 0; i < order; i++)
                    yk4[i] = yPrev[i] + step * k3[i];
                double[] k4 = Derivative(order, yk4, rhs);

                // Итоговый шаг
                for (int i = 0; i < order; i++)
                    y[i] += step / 6.0 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }

        // Адамс-Бэшфорт 2-го порядка
        private void SolveAdamsBashforth2(
            int order, double start, double end, double step, List<double> init,
            SymbolicExpression rhs, Dictionary<string, List<double>> result, List<double> tList)
        {
            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            int N = (int)Math.Ceiling((end - start) / step) + 1;

            // Первый шаг – RK2
            double[] k1 = Derivative(order, y, rhs);
            double[] yk2 = new double[order];
            for (int i = 0; i < order; i++)
                yk2[i] = y[i] + step * k1[i];
            double[] k2 = Derivative(order, yk2, rhs);

            double[] prev_dy = new double[order];
            double[] cur_dy = new double[order];
            for (int i = 0; i < order; i++)
            {
                cur_dy[i] = k1[i];
                y[i] += step / 2.0 * (k1[i] + k2[i]);
                result[$"y{i}"].Add(y[i]);
            }
            t += step;
            tList.Add(t);

            // Далее — основной цикл
            for (int k = 2; k < N; k++)
            {
                prev_dy = (double[])cur_dy.Clone();
                cur_dy = Derivative(order, y, rhs);
                for (int i = 0; i < order; i++)
                    y[i] += step * (3 * cur_dy[i] - prev_dy[i]) / 2.0;

                for (int i = 0; i < order; i++)
                    result[$"y{i}"].Add(y[i]);
                t += step;
                tList.Add(t);
            }
        }

        // Получить вектор производных для системы
        private double[] Derivative(int order, double[] y, SymbolicExpression rhs)
        {
            double[] dy = new double[order];
            for (int i = 0; i < order - 1; i++)
                dy[i] = y[i + 1];

            var vars = new Dictionary<string, FloatingPoint>();
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            dy[order - 1] = rhs.Evaluate(vars).RealValue;
            return dy;
        }

        private void AddMethodPlot(int order, List<double> tList, Dictionary<string, List<double>> result, string methodName)
        {
            for (int i = 0; i < order; i++)
            {
                var curve = Graphic.Plot.Add.Markers(
                    tList.ToArray(),
                    result[$"y{i}"].ToArray()
                );
                curve.LegendText = $"{methodName} y{i}";
                // Можно добавить разницу в маркерах или цветах по методу
            }
            Graphic.Plot.XLabel("t");
            Graphic.Plot.YLabel("y");
            Graphic.Plot.Title("Решение ОДУ (несколько методов)");
            Graphic.Plot.ShowLegend();
            Graphic.Refresh();
        }
    }
}
