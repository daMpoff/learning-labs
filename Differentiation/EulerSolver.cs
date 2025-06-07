namespace Differentiation
{
    public class EulerSolver : IODESolver
    {
        public string Name => "Эйлер";

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

            // Инициализация списков для каждого уравнения
            for (int i = 0; i < order; i++)
            {
                yLists.Add(new List<double>(N) { init[i] });
            }

            double[] y = new double[order];
            init.CopyTo(y);
            double x = start;

            // Основной шаговый цикл
            for (int k = 1; k < N; k++)
            {
                double[] yNext = new double[order];

                // y0' = y1, y1' = y2, ..., y{n-2}' = y{n-1}
                for (int i = 0; i < order - 1; i++)
                {
                    yNext[i] = y[i] + step * y[i + 1];
                }

                // Словарь переменных для правой части: y0, y1, ..., x
                var vars = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                {
                    vars[$"y{i}"] = y[i];
                }
                vars["x"] = x;

                // Правая часть только для старшей производной
                double f = rhs.Evaluate(vars).RealValue;
                yNext[order - 1] = y[order - 1] + step * f;

                // Записать результат
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
