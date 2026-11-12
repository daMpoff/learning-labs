using MathNet.Symbolics;
using System;
using System.Collections.Generic;

namespace Differentiation
{
    public class AdamsMoultonSolver : IODESolver
    {
        public string Name => "Адамс-Мултон (4-й порядок, по презентации)";

        public void Solve(
            int order,
            double start,
            double end,
            double step,
            List<double> init,
            SymbolicExpression rhs,
            out List<double> xList,
            out List<List<double>> yLists
        )
        {
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            xList = new List<double>(N) { start };
            yLists = new List<List<double>>();
            for (int i = 0; i < order; i++)
            { yLists.Add(new List<double>(N) { init[i] }); }

            // Храним значения y и f (правых частей) на 5 последних шагах
            var yPrev = new List<double[]>();
            yPrev.Add(init.ToArray());
            var fPrev = new List<double[]>();

            // "Разгон" — получаем первые четыре точки с помощью RK4
            double x = start;
            double[] y = init.ToArray();
            for (int k = 1; k < 5; k++)
            {
                double[] k1 = RK4Step(y, order, rhs, x, step, 0.0);
                double[] k2 = RK4Step(y, order, rhs, x, step, 0.5, k1);
                double[] k3 = RK4Step(y, order, rhs, x, step, 0.5, k2);
                double[] k4 = RK4Step(y, order, rhs, x, step, 1.0, k3);
                // Основной шаг метода Рунге-Кутта (RK4)
                for (int i = 0; i < order; i++)
                {
                    y[i] += step / 6.0 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
                }

                // Сохраняем предыдущее значение y
                yPrev.Add(y.ToArray());

                // Увеличиваем x и сохраняем его
                x += step;
                xList.Add(x);

                // Сохраняем значения y для каждого уравнения
                for (int i = 0; i < order; i++)
                {
                    yLists[i].Add(y[i]);
                }
            }

            // Инициализируем fPrev (5 последних точек)
            fPrev.Clear();
            for (int i = 0; i < yPrev.Count; i++)
            {
                double xCurrent = start + i * step;
                fPrev.Add(ComputeRhs(yPrev[i], order, rhs, xCurrent));
            }

            // --- Основной цикл: Адамса-Мултона (4-й порядок) ---
            for (int k = 5; k < N; k++)
            {
                double xNew = start + k * step;
                double[] yPred = new double[order]; // Предиктор (можно использовать Bashforth для прогноза)
                for (int i = 0; i < order; i++)
                {
                    yPred[i] = yPrev[4][i]
                        + step * (
                            1901.0 / 720.0 * fPrev[4][i]
                          - 1387.0 / 360.0 * fPrev[3][i]
                          + 109.0  / 30.0 * fPrev[2][i]
                          - 637.0  / 360.0 * fPrev[1][i]
                          + 251.0  / 720.0 * fPrev[0][i]
                        );
                }

                // Теперь вычисляем f(x_{k+1}, yPred) для коррекции по Мултону
                double[] fPred = ComputeRhs(yPred, order, rhs, xNew);

                double[] yNew = new double[order];
                for (int i = 0; i < order; i++)
                {
                    yNew[i] = yPrev[4][i]
                        + step * (
                            (251.0 / 720.0) * fPred[i]
                          + (646.0 / 720.0) * fPrev[4][i]
                          - (264.0 / 720.0) * fPrev[3][i]
                          + (106.0 / 720.0) * fPrev[2][i]
                          - (19.0  / 720.0) * fPrev[1][i]
                        );
                }

                yPrev.RemoveAt(0);
                yPrev.Add(yNew);

                fPrev.RemoveAt(0);
                fPrev.Add(ComputeRhs(yNew, order, rhs, xNew));

                xList.Add(xNew);
                for (int i = 0; i < order; i++)
                { yLists[i].Add(yNew[i]); }
            }
        }

        /// <summary>
        /// Вычисляет массив правых частей для системы ОДУ с передачей x.
        /// </summary>
        private static double[] ComputeRhs(double[] y, int order, SymbolicExpression rhs, double x)
        {
            var vars = new Dictionary<string, FloatingPoint>();
            for (int i = 0; i < order; i++)
            {
                vars[$"y{i}"] = y[i];
                vars["x"] = x;
            }
            double[] f = new double[order];
            for (int i = 0; i < order - 1; i++)
            {
                f[i] = y[i + 1];
                f[order - 1] = rhs.Evaluate(vars).RealValue;
            }
            return f;
        }

        /// <summary>
        /// Один этап RK4 для метода разгона (универсально для любого порядка).
        /// </summary>
        private static double[] RK4Step(double[] y, int order, SymbolicExpression rhs, double x, double step, double a, double[] prevK = null)
        {
            double[] yTemp = new double[order];
            if (prevK == null)
            {
                for (int i = 0; i < order; i++)
                { yTemp[i] = y[i]; }
            }
            else
            {
                for (int i = 0; i < order; i++)
                { yTemp[i] = y[i] + a * step * prevK[i]; }
            }

            var vars = new Dictionary<string, FloatingPoint>();
            for (int i = 0; i < order; i++)
            {
                vars[$"y{i}"] = yTemp[i];
                vars["x"] = x + a * step;
            }
            double[] k = new double[order];
            for (int i = 0; i < order - 1; i++)
            {
                k[i] = yTemp[i + 1];
                k[order - 1] = rhs.Evaluate(vars).RealValue;
            }
            return k;
        }
    }
}
