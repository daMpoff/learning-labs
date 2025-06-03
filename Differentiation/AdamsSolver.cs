namespace Differentiation;

public class AdamsSolver : IODESolver
{
    public string Name => "Адамс (разностный, 4 узла, по презентации)";

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

        // Храним предыдущие 4 y (y[i-3], y[i-2], y[i-1], y[i])
        var yPrev = new List<double[]>();
        yPrev.Add(init.ToArray());

        var vars = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();

        // Храним значения f = f(x, y) для 4 последних шагов (f[i-3], f[i-2], f[i-1], f[i])
        var fPrev = new List<double[]>();

        // Первые три шага "разгоним" методом Рунге-Кутта 4
        double t = start;
        double[] y = init.ToArray();
        for (int k = 1; k < 4; k++)
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

        // Инициализируем fPrev (теперь у нас 4 точки)
        foreach (var yp in yPrev)
        {
            double[] f = new double[order];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = yp[i];
            for (int i = 0; i < order; i++)
                f[i] = rhs.Evaluate(vars).RealValue;
            fPrev.Add(f);
        }

        // --- Основной цикл: разностная формула Адамса (по презентации) ---
        for (int k = 4; k < N; k++)
        {
            double[] yNew = new double[order];
            for (int i = 0; i < order; i++)
            {
                // Разности по презентации:
                double fi = fPrev[3][i];
                double d1 = fi - fPrev[2][i];
                double d2 = fi - 2 * fPrev[2][i] + fPrev[1][i];
                double d3 = fi - 3 * fPrev[2][i] + 3 * fPrev[1][i] - fPrev[0][i];

                yNew[i] = yPrev[3][i]
                        + step * fi
                        + (step * step / 2.0) * d1
                        + (5 * Math.Pow(step, 3) / 12.0) * d2
                        + (3 * Math.Pow(step, 4) / 8.0) * d3;
            }

            // Сдвигаем списки для новых шагов
            yPrev.RemoveAt(0);
            yPrev.Add(yNew);

            // Считаем f_i для нового шага
            double[] fNew = new double[order];
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = yNew[i];
            for (int i = 0; i < order; i++)
                fNew[i] = rhs.Evaluate(vars).RealValue;

            fPrev.RemoveAt(0);
            fPrev.Add(fNew);

            // Добавляем в списки результатов
            t += step;
            tList.Add(t);
            for (int i = 0; i < order; i++)
                yLists[i].Add(yNew[i]);
        }
    }
}
