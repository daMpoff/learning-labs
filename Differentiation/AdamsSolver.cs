using MathNet.Symbolics;

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
            out List<double> xList,
            out List<List<double>> yLists
        )
        {
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            xList = new List<double>(N) { start };
            yLists = new List<List<double>>();
            for (int i = 0; i < order; i++)
                yLists.Add(new List<double>(N) { init[i] });

            // Храним предыдущие 4 y
            var yPrev = new List<double[]>();
            yPrev.Add(init.ToArray());

            // Храним значения f для 4 последних шагов
            var fPrev = new List<double[]>();

            // --- 1. Первые три шага — методом Рунге-Кутта 4 ---
            double x = start;
            double[] y = init.ToArray();
            for (int k = 1; k < 4; k++)
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

            // --- 2. Инициализация fPrev по найденным yPrev ---
            fPrev.Clear();
            for (int i = 0; i < yPrev.Count; i++)
            {
                double xCurrent = start + i * step;
                fPrev.Add(ComputeRhs(yPrev[i], order, rhs, xCurrent));
            }

            // --- 3. Основной цикл Адамса ---
            for (int k = 4; k < N; k++)
            {
                double[] yNew = new double[order];
                for (int i = 0; i < order; i++)
                {
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
        /// Вычисляет массив правых частей для системы ОДУ с передачей x (или t)
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
        /// Один этап RK4 для метода разгона
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
