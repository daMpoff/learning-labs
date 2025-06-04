using MathNet.Symbolics;
using System;
using System.Collections.Generic;

namespace Differentiation
{
    public class AdamsBashforthSolver : IODESolver
    {
        public string Name => "Адамс-Башфорт";

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
                yLists.Add(new List<double>(N) { init[i] });

            // Храним значения y и f на 5 последних шагах
            var yPrev = new List<double[]>();
            yPrev.Add(init.ToArray());
            var fPrev = new List<double[]>();

            // --- "Разгон" RK4 для первых 4 точек ---
            double x = start;
            double[] y = init.ToArray();
            for (int k = 1; k < 5; k++)
            {
                double[] k1 = RK4Step(y, order, rhs, x, step, 0.0);
                double[] k2 = RK4Step(y, order, rhs, x, step, 0.5, k1);
                double[] k3 = RK4Step(y, order, rhs, x, step, 0.5, k2);
                double[] k4 = RK4Step(y, order, rhs, x, step, 1.0, k3);

                for (int i = 0; i < order; i++)
                    y[i] += step / 6.0 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);

                yPrev.Add(y.ToArray());

                x += step;
                xList.Add(x);
                for (int i = 0; i < order; i++)
                    yLists[i].Add(y[i]);
            }

            // --- Инициализация fPrev (5 последних точек) ---
            fPrev.Clear();
            for (int i = 0; i < yPrev.Count; i++)
            {
                double xCurrent = start + i * step;
                fPrev.Add(ComputeRhs(yPrev[i], order, rhs, xCurrent));
            }

            // --- Основной цикл Адамса-Башфорта (5 шагов) ---
            for (int k = 5; k < N; k++)
            {
                double[] yNew = new double[order];
                for (int i = 0; i < order; i++)
                {
                    yNew[i] = yPrev[4][i]
                            + step * (
                                1901.0 / 720.0 * fPrev[4][i]
                              - 1387.0 / 360.0 * fPrev[3][i]
                              + 109.0  / 30.0 * fPrev[2][i]
                              - 637.0  / 360.0 * fPrev[1][i]
                              + 251.0  / 720.0 * fPrev[0][i]
                            );
                }

                yPrev.RemoveAt(0);
                yPrev.Add(yNew);

                double xNew = start + k * step;
                double[] fNew = ComputeRhs(yNew, order, rhs, xNew);

                fPrev.RemoveAt(0);
                fPrev.Add(fNew);

                xList.Add(xNew);
                for (int i = 0; i < order; i++)
                    yLists[i].Add(yNew[i]);
            }
        }

        /// <summary>
        /// Вычисляет массив правых частей для системы ОДУ с передачей x.
        /// </summary>
        private static double[] ComputeRhs(double[] y, int order, SymbolicExpression rhs, double x)
        {
            var vars = new Dictionary<string, FloatingPoint>();
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            vars["x"] = x;
            double[] f = new double[order];
            for (int i = 0; i < order - 1; i++)
                f[i] = y[i + 1];
            f[order - 1] = rhs.Evaluate(vars).RealValue;
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
                    yTemp[i] = y[i];
            }
            else
            {
                for (int i = 0; i < order; i++)
                    yTemp[i] = y[i] + a * step * prevK[i];
            }

            var vars = new Dictionary<string, FloatingPoint>();
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = yTemp[i];
            vars["x"] = x + a * step;
            double[] k = new double[order];
            for (int i = 0; i < order - 1; i++)
                k[i] = yTemp[i + 1];
            k[order - 1] = rhs.Evaluate(vars).RealValue;
            return k;
        }
    }
}
