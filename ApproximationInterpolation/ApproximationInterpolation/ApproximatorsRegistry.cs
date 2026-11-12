namespace ApproximationInterpolation;

public static class ApproximatorsRegistry
{
    public static List<IApproximator> AllApproximators = new()
    {
        new LeastSquaresPolynomialApproximator(1),
        new TaylorApproximator("x", 1),// "заглушка", параметры будут подставляться из UI
        new ChebyshevApproximator("x", 1, -1, 1),
        new FourierApproximator("x", 1)
    };
}