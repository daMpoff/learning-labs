namespace Differentiation;

public class RungeKutta3Solver : IODESolver
{
    public string Name => "Рунге-Кутта 3";

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
            double[] y2 = new double[order];
            for (int i = 0; i < order; i++)
                y2[i] = y[i] + (step / 3.0) * k1[i];
            double[] k2 = new double[order];
            for (int i = 0; i < order - 1; i++)
                k2[i] = y2[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y2[i];
            k2[order - 1] = rhs.Evaluate(vars).RealValue;

            // --- k3 ---
            double[] y3 = new double[order];
            for (int i = 0; i < order; i++)
                y3[i] = y[i] + (2.0 * step / 3.0) * k2[i];
            double[] k3 = new double[order];
            for (int i = 0; i < order - 1; i++)
                k3[i] = y3[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y3[i];
            k3[order - 1] = rhs.Evaluate(vars).RealValue;

            // --- обновление ---
            for (int i = 0; i < order; i++)
                y[i] += step * (1.0 / 4.0 * k1[i] + 3.0 / 4.0 * k3[i]);

            for (int i = 0; i < order; i++)
                yLists[i].Add(y[i]);
            t += step;
            tList.Add(t);
        }
    }
}
