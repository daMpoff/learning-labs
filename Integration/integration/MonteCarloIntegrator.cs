namespace integration;

public class MonteCarloIntegrator : IIntegrator
{
    public string Name => "Монте-Карло (среднее знач.)";
    private static readonly Random rnd = new();

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int n = Math.Max(1000, (int)((b - a) / h));
        double sum = 0;
        for (int i = 0; i < n; i++)
        {
            double x = a + rnd.NextDouble() * (b - a);
            sum += f(x);
        }
        return (b - a) * sum / n;
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        int n = Math.Min(500, Math.Max(100, (int)((b - a) / h)));
        double[] xs = new double[n];
        double[] ys = new double[n];
        for (int i = 0; i < n; i++)
        {
            xs[i] = a + rnd.NextDouble() * (b - a);
            ys[i] = f(xs[i]);
        }
        plot.Add.ScatterPoints(xs, ys);
    }
}
