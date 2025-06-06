namespace integration;

public interface IIntegrator
{
    string Name { get; }
    double Integrate(Func<double, double> f, double a, double b, double h);
    void Visualize(ScottPlot.Plot plot, Func<double, double> f, double a, double b, double h);
}
