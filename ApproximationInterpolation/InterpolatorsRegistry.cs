namespace ApproximationInterpolation;

public static class InterpolatorsRegistry
{
    public static List<IInterpolator> AllInterpolators = new()
    {
        new LinearPiecewiseInterpolator(),
        new QuadraticPiecewiseInterpolator(),
        new CubicSplineInterpolator(),
        new LagrangeInterpolator(),
        new NewtonForwardInterpolator(),
    };
}