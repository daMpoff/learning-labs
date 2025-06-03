namespace Differentiation;

public class AdamsBashforthSolver : IODESolver
{
    public string Name => "Адамс-Башфорт (4/5 шагов, по презентации)";

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

        var vars = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();

        // Сохраняем значения y и f на 5 последних шагах
        var yPrev = new List<double[]>();
        yPrev.Add(init.ToArray());
        var fPrev = new List<double[]>();

        // "Разгон" — получаем первые четыре точки с помощью RK4
        double t = start;
        double[] y = init.ToArray();
        for (int k = 1; k < 5; k++)
        {
            // RK4-один шаг
            double[] k1 = new double[order];
            double[] k2 = new double[order];
            double[] k3 = new double[order];
            double[] k4 = new double[order];

            for (int i = 0; i < order - 1; i++)
                k1[i] = y[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            k1[order - 1] = rhs.Evaluate(vars).RealValue;

            double[] y2 = new double[order];
            for (int i = 0; i < order; i++)
                y2[i] = y[i] + (step / 2.0) * k1[i];
            for (int i = 0; i < order - 1; i++)
                k2[i] = y2[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y2[i];
            k2[order - 1] = rhs.Evaluate(vars).RealValue;

            double[] y3 = new double[order];
            for (int i = 0; i < order; i++)
                y3[i] = y[i] + (step / 2.0) * k2[i];
            for (int i = 0; i < order - 1; i++)
                k3[i] = y3[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y3[i];
            k3[order - 1] = rhs.Evaluate(vars).RealValue;

            double[] y4 = new double[order];
            for (int i = 0; i < order; i++)
                y4[i] = y[i] + step * k3[i];
            for (int i = 0; i < order - 1; i++)
                k4[i] = y4[i + 1];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y4[i];
            k4[order - 1] = rhs.Evaluate(vars).RealValue;

            for (int i = 0; i < order; i++)
                y[i] += step / 6.0 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);

            // Сохраняем для разностной схемы
            yPrev.Add(y.ToArray());

            // Запоминаем t и y
            t += step;
            tList.Add(t);
            for (int i = 0; i < order; i++)
                yLists[i].Add(y[i]);
        }

        // Инициализируем fPrev (5 последних точек)
        foreach (var yp in yPrev)
        {
            double[] f = new double[order];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = yp[i];
            for (int i = 0; i < order; i++)
                f[i] = rhs.Evaluate(vars).RealValue;
            fPrev.Add(f);
        }

        // --- Основной цикл: Адамса-Башфорта (5 шагов) ---
        for (int k = 5; k < N; k++)
        {
            double[] yNew = new double[order];
            for (int i = 0; i < order; i++)
            {
                // Формула для пятишагового метода (последняя строка на твоём слайде)
                yNew[i] = yPrev[4][i]
                        + step * (
                            1901.0 / 720.0 * fPrev[4][i]
                          - 1387.0 / 360.0 * fPrev[3][i]
                          + 109.0  / 30.0 * fPrev[2][i]
                          - 637.0  / 360.0 * fPrev[1][i]
                          + 251.0  / 720.0 * fPrev[0][i]
                        );
            }

            // Сдвигаем окна хранения назад
            yPrev.RemoveAt(0);
            yPrev.Add(yNew);

            double[] fNew = new double[order];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = yNew[i];
            for (int i = 0; i < order; i++)
                fNew[i] = rhs.Evaluate(vars).RealValue;
            fPrev.RemoveAt(0);
            fPrev.Add(fNew);

            t += step;
            tList.Add(t);
            for (int i = 0; i < order; i++)
                yLists[i].Add(yNew[i]);
        }
    }
}
