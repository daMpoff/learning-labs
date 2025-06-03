using MathNet.Symbolics;
using System;
using System.Collections.Generic;

namespace Differentiation
{
    public class AdamsSolver : IODESolver
    {
        public string Name => "Адамс (разностный, 4 узла, по презентации)";

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

            // Храним предыдущие 4 y (для метода Адамса)
            var yPrev = new List<double[]>();
            yPrev.Add(init.ToArray());

            // Храним значения f (dy/dt) для 4 последних шагов
            var fPrev = new List<double[]>();

            var vars = new Dictionary<string, FloatingPoint>();

            // --- 1. Первые три шага — методом Рунге-Кутта 4
            double t = start;
            double[] y = init.ToArray();
            for (int k = 1; k < 4; k++)
            {
                double[] k1 = new double[order];
                double[] k2 = new double[order];
                double[] k3 = new double[order];
                double[] k4 = new double[order];

                // k1
                ComputeRhs(y, order, rhs, vars, out k1);

                // k2
                double[] y2 = new double[order];
                for (int i = 0; i < order; i++)
                    y2[i] = y[i] + (step / 2.0) * k1[i];
                ComputeRhs(y2, order, rhs, vars, out k2);

                // k3
                double[] y3 = new double[order];
                for (int i = 0; i < order; i++)
                    y3[i] = y[i] + (step / 2.0) * k2[i];
                ComputeRhs(y3, order, rhs, vars, out k3);

                // k4
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

            // --- 2. Инициализация fPrev по уже найденным yPrev
            foreach (var yp in yPrev)
            {
                double[] f = new double[order];
                ComputeRhs(yp, order, rhs, vars, out f);
                fPrev.Add(f);
            }

            // --- 3. Основной цикл метода Адамса
            for (int k = 4; k < N; k++)
            {
                double[] yNew = new double[order];
                for (int i = 0; i < order; i++)
                {
                    // Разностная формула для каждой компоненты!
                    double fi = fPrev[3][i];
                    double d1 = fi - fPrev[2][i];
                    double d2 = fi - 2 * fPrev[2][i] + fPrev[1][i];
                    double d3 = fi - 3 * fPrev[2][i] + 3 * fPrev[1][i] - fPrev[0][i];

                    yNew[i] = yPrev[3][i]
                            + step * fi
                            + (step * step / 2.0) * d1
                            + (5 * Math.Pow(step, 3) / 12.0) * d2
                            + (3 * Math.Pow(step, 4) / 8.0) * d3;
                }

                yPrev.RemoveAt(0);
                yPrev.Add(yNew);

                double[] fNew = new double[order];
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
        /// Вычисляет массив правых частей для системы ОДУ
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
