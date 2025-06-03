using MathNet.Symbolics;

namespace Differentiation;

public class ImprovedEulerSolver : IODESolver
{
    public string Name => "Улучшенный метод Эйлера";

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

        double[] y = new double[order];
        init.CopyTo(y);
        double x = start;

        for (int k = 1; k < N; k++)
        {
            // 1. Вычисляем f(x_i, y_i)
            var vars = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            vars["x"] = x;
            double f = rhs.Evaluate(vars).RealValue;

            // 2. y_{i+1/2} = y_i + (h/2) * f(x_i, y_i)
            double[] y_half = new double[order];
            for (int i = 0; i < order - 1; i++)
                y_half[i] = y[i] + (step / 2.0) * y[i + 1];
            y_half[order - 1] = y[order - 1] + (step / 2.0) * f;

            // 3. f(x_{i+1/2}, y_{i+1/2})
            var vars_half = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
            for (int i = 0; i < order; i++)
                vars_half[$"y{i}"] = y_half[i];
            vars_half["x"] = x + step / 2.0;
            double f_half = rhs.Evaluate(vars_half).RealValue;

            // 4. y_{i+1} = y_i + h * f(x_{i+1/2}, y_{i+1/2})
            double[] y_next = new double[order];
            for (int i = 0; i < order - 1; i++)
                y_next[i] = y[i] + step * y_half[i + 1];
            y_next[order - 1] = y[order - 1] + step * f_half;

            // Обновляем значения
            for (int i = 0; i < order; i++)
            {
                y[i] = y_next[i];
                yLists[i].Add(y[i]);
            }
            x += step;
            xList.Add(x);
        }
    }
}
