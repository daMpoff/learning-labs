namespace Differentiation
{
    public class RungeKuttaSolver : IODESolver
    {
        public string Name => "Рунге-Кутта";

        public void Solve(
            int order,
            double start,
            double end,
            double step,
            List<double> init,
            MathNet.Symbolics.SymbolicExpression rhs,
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
                double[] yTemp = new double[order];
                for (int i = 0; i < order; i++)
                {
                    yTemp[i] = y[i] + step / 2.0 * k1[i];
                }
                var vars2 = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    vars2[$"y{i}"] = yTemp[i];
                }
                vars2["x"] = x + step / 2.0;

                double[] k2 = new double[order];
                for (int i = 0; i < order - 1; i++)
                {
                    k2[i] = yTemp[i + 1];
                }
                k2[order - 1] = rhs.Evaluate(vars2).RealValue;

                // --- итоговое обновление ---
                for (int i = 0; i < order; i++)
                {
                    y[i] += step * k2[i];
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
