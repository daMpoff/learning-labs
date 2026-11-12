namespace integration;

public class TrapezoidIntegrator : IIntegrator
{
    public string Name => "Трапеции";

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Ceiling((b - a) / h);
        double sum = 0;
        double xPrev = a, yPrev = f(a);
        for (int i = 1; i <= N; i++)
        {
            double x = Math.Min(a + i * h, b);
            double y = f(x);
            double hi = x - xPrev;
            sum += hi * (yPrev + y) / 2.0;
            xPrev = x;
            yPrev = y;
        }
        return sum;
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Ceiling((b - a) / h);

        double xPrev = a, yPrev = f(a);
        for (int i = 1; i <= N; i++)
        {
            double x = Math.Min(a + i * h, b);
            double y = f(x);

            double[] xTrap = { xPrev, xPrev, x, x };
            double[] yTrap = { 0, yPrev, y, 0 };
            plot.Add.Polygon(xTrap, yTrap);
            xPrev = x;
            yPrev = y;
        }
    }
}
