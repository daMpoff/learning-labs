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
        new LeastSquaresPolynomialInterpolator(1), // линейный МНК
        new LeastSquaresPolynomialInterpolator(2), // квадратичный МНК
        new LeastSquaresPolynomialInterpolator(3), // кубический МНК
        // Примеры рядов Тейлора для некоторых функций
        new TaylorInterpolator(
            new Func<double, double>[] 
            { 
                x => Math.Sin(x), 
                x => Math.Cos(x), 
                x => -Math.Sin(x), 
                x => -Math.Cos(x), 
                x => Math.Sin(x), 
                x => Math.Cos(x) 
            }, 
            "sin(x)", 
            5
        ),
        new TaylorInterpolator(
            new Func<double, double>[] 
            { 
                x => Math.Cos(x), 
                x => -Math.Sin(x), 
                x => -Math.Cos(x), 
                x => Math.Sin(x), 
                x => Math.Cos(x), 
                x => -Math.Sin(x) 
            }, 
            "cos(x)", 
            5
        ),
        new TaylorInterpolator(
            new Func<double, double>[] 
            { 
                x => Math.Exp(x), 
                x => Math.Exp(x), 
                x => Math.Exp(x), 
                x => Math.Exp(x), 
                x => Math.Exp(x), 
                x => Math.Exp(x) 
            }, 
            "exp(x)", 
            5
        )
    };
}