namespace integration;

public class RightRectangleIntegrator : IIntegrator
{
    public string Name => "Прямоугольники (правый)";

    public double Integrate(Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Ceiling((b - a) / h);
        double sum = 0;
        for (int i = 1; i <= N; i++)
        {
            double x = a + i * h;
            double left = x - h;
            double width = Math.Min(h, b - left);
            sum += f(x) * width;
        }
        return sum;
    }

    public void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h)
    {
        int N = (int)Math.Ceiling((b - a) / h);
        for (int i = 1; i <= N; i++)
        {
            double x1 = a + i * h;
            double x0 = Math.Max(a, x1 - h);
            double y = f(x1);

            double[] xRect = { x0, x0, x1, x1 };
            double[] yRect = { 0, y, y, 0 };
            plot.Add.Polygon(xRect, yRect);
        }
    }
}
