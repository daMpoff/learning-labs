namespace Differentiation;

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
            double[] yNext = new double[order];
            for (int i = 0; i < order - 1; i++)
                yNext[i] = y[i] + step * y[i + 1];

            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            double f = rhs.Evaluate(vars).RealValue;
            yNext[order - 1] = y[order - 1] + step * f;

            for (int i = 0; i < order; i++)
                yLists[i].Add(yNext[i]);
            t += step;
            tList.Add(t);
            Array.Copy(yNext, y, order);
        }
    }
}
