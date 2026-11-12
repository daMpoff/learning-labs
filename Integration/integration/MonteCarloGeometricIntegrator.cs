namespace integration;

public class MonteCarloGeometricIntegrator : IIntegrator
{
    public string Name => "Монте-Карло (геометр.)";
    private static readonly Random rnd = new();

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int n = (int)((b - a) / h);

        // 1. Оцениваем min и max функции для построения прямоугольника
        int m = 400;
        double[] xsGrid = Enumerable.Range(0, m).Select(i => a + (b - a) * i / (m - 1)).ToArray();
        double minY = xsGrid.Select(f).Min();
        double maxY = xsGrid.Select(f).Max();

        // 2. Генерируем точки в прямоугольнике [a, b] × [minY, maxY]
        int hitsAbove = 0;
        int hitsBelow = 0;
        for (int i = 0; i < n; i++)
        {
            double x = a + rnd.NextDouble() * (b - a);
            double y = minY + rnd.NextDouble() * (maxY - minY);
            double fx = f(x);

            if (y > 0 && y <= fx)
                hitsAbove++;
            else if (y < 0 && y >= fx)
                hitsBelow++;
        }

        double area = (b - a) * (maxY - minY);

        // Площадь над осью минус площадь под осью
        double result = area * (hitsAbove - hitsBelow) / n;
        return result;
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        // 1. Оцениваем min и max функции для построения прямоугольника
        int n = (int)((b - a) / h);
        int m = 400;
        double[] xsGrid = Enumerable.Range(0, m).Select(i => a + (b - a) * i / (m - 1)).ToArray();
        double minY = xsGrid.Select(f).Min();
        double maxY = xsGrid.Select(f).Max();

        // 2. Генерируем точки
        double[] xs = new double[n];
        double[] ys = new double[n];
        bool[] isAbove = new bool[n];
        bool[] isBelow = new bool[n];

        for (int i = 0; i < n; i++)
        {
            xs[i] = a + rnd.NextDouble() * (b - a);
            ys[i] = minY + rnd.NextDouble() * (maxY - minY);
            double fx = f(xs[i]);
            isAbove[i] = (ys[i] > 0 && ys[i] <= fx);
            isBelow[i] = (ys[i] < 0 && ys[i] >= fx);
        }

        // График функции
        double[] ysFunc = xsGrid.Select(f).ToArray();
        plot.Add.Scatter(xsGrid, ysFunc);

        // Прямоугольник области
        plot.Add.Polygon(
            new[] { a, a, b, b },
            new[] { minY, maxY, maxY, minY }
        );

        // Попадания над осью (зелёные)
        var xsA = xs.Where((_, i) => isAbove[i]).ToArray();
        var ysA = ys.Where((_, i) => isAbove[i]).ToArray();
        plot.Add.ScatterPoints(xsA, ysA, color: ScottPlot.Colors.Green);

        // Попадания под осью (красные)
        var xsB = xs.Where((_, i) => isBelow[i]).ToArray();
        var ysB = ys.Where((_, i) => isBelow[i]).ToArray();
        plot.Add.ScatterPoints(xsB, ysB, color: ScottPlot.Colors.Red);
    }
}

