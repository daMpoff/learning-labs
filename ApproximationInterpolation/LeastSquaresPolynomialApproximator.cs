using MathNet.Numerics.LinearAlgebra;

namespace ApproximationInterpolation;

public class LeastSquaresPolynomialApproximator : IApproximator
{
    public string Name => $"МНК-полином";
    public int Degree { get; private set; }
    private Vector<double> coeffs;

    public LeastSquaresPolynomialApproximator(int degree)
    {
        Degree = degree;
    }

    public void SetPoints(List<(double x, double y)> points)
    {
        int n = points.Count, m = Degree + 1;
        var X = Matrix<double>.Build.Dense(n, m, (i, j) => Math.Pow(points[i].x, j));
        var Y = Vector<double>.Build.Dense(n, i => points[i].y);
        var Xt = X.Transpose();
        coeffs = Xt * X == null ? null : (Xt * X).Solve(Xt * Y);
    }

    public double Approximate(double x)
    {
        double result = 0;
        for (int j = 0; j < coeffs.Count; j++)
            result += coeffs[j] * Math.Pow(x, j);
        return result;
    }
}