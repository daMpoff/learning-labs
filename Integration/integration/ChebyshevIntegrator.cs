namespace integration;

public class ChebyshevIntegrator : IIntegrator
{
    public string Name => "Чебышёв";
    public int Order { get; set; } = 5; // по умолчанию

    // Табличные узлы Чебышёва (https://ru.wikipedia.org/wiki/Чебышёвские_узлы)
    public static double[] GetChebyshevRoots(int n)
    {
        // Только поддерживаемые значения по твоему слайду (иначе можно сгенерировать по формуле)
        return n switch
        {
            2 => new[] { 0.70710678, -0.70710678 },
            3 => new[] { 0.8660254, 0, -0.8660254 },
            4 => new[] { 0.9238795, 0.3826834, -0.3826834, -0.9238795 },
            5 => new[] { 0.9510565, 0.5877853, 0, -0.5877853, -0.9510565 },
            6 => new[] { 0.9659258, 0.7071068, 0.2588190, -0.2588190, -0.7071068, -0.9659258 },
            7 => new[] { 0.9749279, 0.7818315, 0.4338837, 0, -0.4338837, -0.7818315, -0.9749279 },
            9 => new[] { 0.9848078, 0.8660254, 0.6427876, 0.3420201, 0, -0.3420201, -0.6427876, -0.8660254, -0.9848078 },
            _ => throw new NotImplementedException($"Для порядка n={n} нет табличных узлов. Разрешены только n=2,3,4,5,6,7,9."),
        };
    }

    // Все веса равны 2/n по формуле (см. слайд)
    public static double[] GetChebyshevWeights(int n)
    {
        return Enumerable.Repeat(2.0 / n, n).ToArray();
    }

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int n = Order;
        var t = GetChebyshevRoots(n);
        var A = GetChebyshevWeights(n);

        double sum = 0;
        // Формула перехода (19): x_i = (a+b)/2 + (b-a)/2 * t_i
        double xm = (a + b) / 2.0;
        double xr = (b - a) / 2.0;

        for (int i = 0; i < n; i++)
        {
            double xi = xm + xr * t[i];
            sum += A[i] * f(xi);
        }
        return xr * sum; // Формула (20) — главный результат
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        int points = 500;
        double[] xs = Enumerable.Range(0, points).Select(i => a + (b - a) * i / (points - 1)).ToArray();
        double[] ys = xs.Select(f).ToArray();
        plot.Add.Scatter(xs, ys, color: ScottPlot.Colors.Orange);

        // Показываем узлы (отметим точки формулы)
        int n = Order;
        var t = GetChebyshevRoots(n);
        double xm = (a + b) / 2.0, xr = (b - a) / 2.0;
        double[] xNodes = t.Select(ti => xm + xr * ti).ToArray();
        double[] yNodes = xNodes.Select(f).ToArray();
        plot.Add.ScatterPoints(xNodes, yNodes, color: ScottPlot.Colors.OrangeRed);
    }
}