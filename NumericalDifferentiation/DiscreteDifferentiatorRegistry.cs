namespace NumericalDifferentiation;

public static class DiscreteDifferentiatorRegistry
{
    public static readonly List<IDiscreteDifferentiator> All = new()
    {
        new LinearDifferentiator(),
        new QuadraticDifferentiator(),
        new CubicDifferentiator(),
        new NewtonDifferentiator(),
        new UndeterminedCoefficientsDifferentiator(),
        new RungeDifferentiator()
    };
}