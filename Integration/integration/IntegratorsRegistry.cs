namespace integration;

public static class IntegratorsRegistry
{
    public static List<IIntegrator> AllIntegrators = new()
    {
        new LeftRectangleIntegrator(),
        new RightRectangleIntegrator(),
        new MiddleRectangleIntegrator(),
        new TrapezoidIntegrator(),
        new SimpsonIntegrator(),
        new CubicSplineIntegrator(),
        new MonteCarloIntegrator(),
        new MonteCarloGeometricIntegrator(),
        new GaussIntegrator(),
        new ChebyshevIntegrator()
    };
}