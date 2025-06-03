namespace Differentiation;

public class IterativeEulerSolver : IODESolver
{
    public string Name => "Эйлер с итерациями";

    public void Solve(
        int order,
        double start,
        double end,
        double step,
        List<double> init,
        MathNet.Symbolics.SymbolicExpression rhs,
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

        var vars = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
        int numIterations = 4; // по презентации: обычно 3-4 итерации

        for (int k = 1; k < N; k++)
        {
            // --- Predictor: обычный Эйлер ---
            double[] yIter = new double[order];
            for (int i = 0; i < order - 1; i++)
                yIter[i] = y[i] + step * y[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            double f = rhs.Evaluate(vars).RealValue;
            yIter[order - 1] = y[order - 1] + step * f;

            // --- Итерационный процесс (корректор) ---
            for (int iter = 1; iter < numIterations; iter++)
            {
                double[] yPrev = (double[])yIter.Clone();

                // Для первых переменных: обычная формула Коши-Эйлера, но с yPrev
                for (int i = 0; i < order - 1; i++)
                    yIter[i] = y[i] + step / 2.0 * (y[i + 1] + yPrev[i + 1]);

                // Последняя переменная: уточняем правую часть по новой yIter
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = yPrev[i];
                double fBar = rhs.Evaluate(vars).RealValue;
                for (int i = 0; i < order; i++)
                    vars[$"y{i}"] = y[i];
                double fOld = rhs.Evaluate(vars).RealValue;
                yIter[order - 1] = y[order - 1] + step / 2.0 * (fOld + fBar);
            }

            // --- Финальное значение ---
            for (int i = 0; i < order; i++)
                yLists[i].Add(yIter[i]);
            t += step;
            tList.Add(t);
            Array.Copy(yIter, y, order);
        }
    }
}
