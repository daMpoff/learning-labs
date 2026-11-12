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
            out List<double> xList,
            out List<List<double>> yLists
        )
        {
            int N = (int)Math.Ceiling((end - start) / step) + 1;
            xList = new List<double>(N) { start };
            yLists = new List<List<double>>();

            // Инициализация списков для каждого уравнения
            for (int i = 0; i < order; i++)
            {
                yLists.Add(new List<double>(N) { init[i] });
            }

            double[] y = new double[order];
            init.CopyTo(y);
            double x = start;

            // Основной цикл по шагам
            for (int k = 1; k < N; k++)
            {
                // 1. Predictor: обычный Эйлер
                double[] yBar = new double[order];
                for (int i = 0; i < order - 1; i++)
                {
                    yBar[i] = y[i] + step * y[i + 1];
                }

                var vars = new Dictionary<string, FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    vars[$"y{i}"] = y[i];
                }
                vars["x"] = x;
                double f = rhs.Evaluate(vars).RealValue;
                yBar[order - 1] = y[order - 1] + step * f;

                // 2. Corrector: по формуле Коши
                double[] yNext = new double[order];
                for (int i = 0; i < order - 1; i++)
                {
                    yNext[i] = y[i] + step / 2.0 * (y[i + 1] + yBar[i + 1]);
                }

                // f(xi, y) и f(xi+1, yBar)
                var varsBar = new Dictionary<string, FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    varsBar[$"y{i}"] = yBar[i];
                }
                varsBar["x"] = x + step;
                double fBar = rhs.Evaluate(varsBar).RealValue;
                yNext[order - 1] = y[order - 1] + step / 2.0 * (f + fBar);

                // Сохраняем новое значение
                for (int i = 0; i < order; i++)
                {
                    yLists[i].Add(yNext[i]);
                }

                x += step;
                xList.Add(x);

                Array.Copy(yNext, y, order);
            }
        }
    }
}
