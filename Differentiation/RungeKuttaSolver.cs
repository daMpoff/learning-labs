namespace Differentiation;

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

        for (int k = 1; k < N; k++)
        {
            // --- k1 ---
            double[] k1 = new double[order];
            for (int i = 0; i < order - 1; i++)
                k1[i] = y[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            k1[order - 1] = rhs.Evaluate(vars).RealValue;

            // --- k2 ---
            double[] yTemp = new double[order];
            for (int i = 0; i < order; i++)
                yTemp[i] = y[i] + step / 2.0 * k1[i];
            double[] k2 = new double[order];
            for (int i = 0; i < order - 1; i++)
                k2[i] = yTemp[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = yTemp[i];
            k2[order - 1] = rhs.Evaluate(vars).RealValue;

            // --- итоговое обновление ---
            for (int i = 0; i < order; i++)
                y[i] += step * k2[i];

            for (int i = 0; i < order; i++)
                yLists[i].Add(y[i]);
            t += step;
            tList.Add(t);
        }
    }
}
