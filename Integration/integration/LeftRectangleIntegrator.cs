namespace integration;

public class LeftRectangleIntegrator : IIntegrator
{
    public string Name => "Прямоугольники (левый)";

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Ceiling((b - a) / h);
        double sum = 0;
        for (int i = 0; i < N; i++)
        {
            double x = a + i * h;
            double width = Math.Min(h, b - x);
            sum += f(x) * width;
        }
        return sum;
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Ceiling((b - a) / h);
        for (int i = 0; i < N; i++)
        {
            double x0 = a + i * h;
            double x1 = Math.Min(x0 + h, b);
            double y = f(x0);

            double[] xRect = { x0, x0, x1, x1 };
            double[] yRect = { 0, y, y, 0 };
            plot.Add.Polygon(xRect, yRect);
        }
    }
}
