using MathNet.Symbolics;

namespace Differentiation
{
    public class RungeKutta4Solver : IODESolver
    {
        public string Name => "Рунге-Кутта 4 порядка";

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
            {
                yLists.Add(new List<double>(N) { init[i] });
            }

            double[] y = new double[order];
            init.CopyTo(y);
            double x = start;

            for (int k = 1; k < N; k++)
            {
                // --- k1 ---
                var vars1 = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    vars1[$"y{i}"] = y[i];
                }
                vars1["x"] = x;

                double[] k1 = new double[order];
                for (int i = 0; i < order - 1; i++)
                {
                    k1[i] = y[i + 1];
                }
                k1[order - 1] = rhs.Evaluate(vars1).RealValue;

                // --- k2 ---
                double[] y2 = new double[order];
                for (int i = 0; i < order; i++)
                {
                    y2[i] = y[i] + (step / 2.0) * k1[i];
                }
                var vars2 = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    vars2[$"y{i}"] = y2[i];
                }
                vars2["x"] = x + step / 2.0;

                double[] k2 = new double[order];
                for (int i = 0; i < order - 1; i++)
                {
                    k2[i] = y2[i + 1];
                }
                k2[order - 1] = rhs.Evaluate(vars2).RealValue;

                // --- k3 ---
                double[] y3 = new double[order];
                for (int i = 0; i < order; i++)
                {
                    y3[i] = y[i] + (step / 2.0) * k2[i];
                }
                var vars3 = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    vars3[$"y{i}"] = y3[i];
                }
                vars3["x"] = x + step / 2.0;

                double[] k3 = new double[order];
                for (int i = 0; i < order - 1; i++)
                {
                    k3[i] = y3[i + 1];
                }
                k3[order - 1] = rhs.Evaluate(vars3).RealValue;
                // --- k4 ---
                double[] y4 = new double[order];
                for (int i = 0; i < order; i++)
                {
                    y4[i] = y[i] + step * k3[i];
                }
                var vars4 = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    vars4[$"y{i}"] = y4[i];
                }
                vars4["x"] = x + step;
                double[] k4 = new double[order];
                for (int i = 0; i < order - 1; i++)
                {
                    k4[i] = y4[i + 1];
                }
                k4[order - 1] = rhs.Evaluate(vars4).RealValue;
                // --- обновление ---
                for (int i = 0; i < order; i++)
                {
                    y[i] += step / 6.0 * (k1[i] + 2.0 * k2[i] + 2.0 * k3[i] + k4[i]);
                }
                for (int i = 0; i < order; i++)
                {
                    yLists[i].Add(y[i]);
                }
                x += step;
                xList.Add(x);
            }
        }
    }
}
