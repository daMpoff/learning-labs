using MathNet.Symbolics;
using System;
using System.Collections.Generic;

namespace Differentiation
{
    public class AdamsBashforthSolver : IODESolver
    {
        public string Name => "Адамс-Башфорт (4/5 шагов, по презентации)";

        public void Solve(
            int order,
            double start,
            double end,
            double step,
            List<double> init,
            SymbolicExpression rhs,
            out List<double> tList,
            out List<List<double>> yLists
        )
        {
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            tList = new List<double>(N) { start };
            yLists = new List<List<double>>();
            for (int i = 0; i < order; i++)
                yLists.Add(new List<double>(N) { init[i] });

            var vars = new Dictionary<string, FloatingPoint>();

            // Сохраняем значения y и f на 5 последних шагах
            var yPrev = new List<double[]>();
            yPrev.Add(init.ToArray());
            var fPrev = new List<double[]>();

            // "Разгон" — получаем первые четыре точки с помощью RK4
            double t = start;
            double[] y = init.ToArray();
            for (int k = 1; k < 5; k++)
            {
                double[] k1, k2, k3, k4;

                ComputeRhs(y, order, rhs, vars, out k1);

                double[] y2 = new double[order];
                for (int i = 0; i < order; i++)
                    y2[i] = y[i] + (step / 2.0) * k1[i];
                ComputeRhs(y2, order, rhs, vars, out k2);

                double[] y3 = new double[order];
                for (int i = 0; i < order; i++)
                    y3[i] = y[i] + (step / 2.0) * k2[i];
                ComputeRhs(y3, order, rhs, vars, out k3);

                double[] y4 = new double[order];
                for (int i = 0; i < order; i++)
                    y4[i] = y[i] + step * k3[i];
                ComputeRhs(y4, order, rhs, vars, out k4);

                for (int i = 0; i < order; i++)
                    y[i] += step / 6.0 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);

                yPrev.Add(y.ToArray());

                t += step;
                tList.Add(t);
                for (int i = 0; i < order; i++)
                    yLists[i].Add(y[i]);
            }

            // Инициализируем fPrev (5 последних точек)
            foreach (var yp in yPrev)
            {
                double[] f;
                ComputeRhs(yp, order, rhs, vars, out f);
                fPrev.Add(f);
            }

            // --- Основной цикл: Адамса-Башфорта (5 шагов) ---
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

                double[] fNew;
                ComputeRhs(yNew, order, rhs, vars, out fNew);
                fPrev.RemoveAt(0);
                fPrev.Add(fNew);

                t += step;
                tList.Add(t);
                for (int i = 0; i < order; i++)
                    yLists[i].Add(yNew[i]);
            }
        }

        /// <summary>
        /// Универсальная функция правых частей для любой системы ОДУ любого порядка:
        /// для всех кроме последней компоненты — просто y[i+1], для последней — rhs.
        /// </summary>
        private static void ComputeRhs(
            double[] y, int order, SymbolicExpression rhs, Dictionary<string, FloatingPoint> vars, out double[] f
        )
        {
            f = new double[order];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            for (int i = 0; i < order - 1; i++)
                f[i] = y[i + 1];
            f[order - 1] = rhs.Evaluate(vars).RealValue;
        }
    }
}
