using MathNet.Symbolics;

namespace Differentiation
{
    public class EulerCauchySolver : IODESolver
    {
        public string Name => "Эйлер-Коши";
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

            double[] y = new double[order];
            init.CopyTo(y);
            double t = start;
            var vars = new Dictionary<string, FloatingPoint>();

            for (int k = 1; k < N; k++)
            {
                // 1. Predictor: обычный Эйлер
                double[] yBar = new double[order];
                for (int i = 0; i < order - 1; i++)
                    yBar[i] = y[i] + step * y[i + 1];

                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = y[i];
                double f = rhs.Evaluate(vars).RealValue;
                yBar[order - 1] = y[order - 1] + step * f;

                // 2. Corrector: по формуле Коши
                double[] yNext = new double[order];
                // для y0 ... yN-2
                for (int i = 0; i < order - 1; i++)
                    yNext[i] = y[i] + step / 2.0 * (y[i + 1] + yBar[i + 1]);
                // для yN-1
                // f(xi, y) и f(xi+1, yBar)
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = yBar[i];
                double fBar = rhs.Evaluate(vars).RealValue;
                yNext[order - 1] = y[order - 1] + step / 2.0 * (f + fBar);

                for (int i = 0; i < order; i++)
                    yLists[i].Add(yNext[i]);
                t += step;
                tList.Add(t);
                Array.Copy(yNext, y, order);
            }
        }
    }

} 