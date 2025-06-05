using MathNet.Numerics.LinearAlgebra;

namespace ApproximationInterpolation;

public class LeastSquaresPolynomialInterpolator : IInterpolator
{
    public string Name => $"МНК-полином (степень {Degree})";

    private int Degree; // степень полинома
    private Vector<double> coeffs;

    public LeastSquaresPolynomialInterpolator(int degree)
    {
        Degree = degree;
    }

    public void SetPoints(List<(double x, double y)> points)
    {
        int n = points.Count;
        int m = Degree + 1;

        // Формируем матрицу X (размер n x m)
        var X = Matrix<double>.Build.Dense(n, m, (i, j) => Math.Pow(points[i].x, j));

        // Формируем вектор Y (размер n)
        var Y = Vector<double>.Build.Dense(n, i => points[i].y);

        // Решаем (X^T X)A = X^T Y
        var Xt = X.Transpose();
        var XtX = Xt * X;
        var XtY = Xt * Y;

        // coeffs = (X^T X)^-1 * X^T Y
        coeffs = XtX.Solve(XtY);
    }

    public double Interpolate(double x)
    {
        double result = 0;
        for (int j = 0; j < coeffs.Count; j++)
            result += coeffs[j] * Math.Pow(x, j);
        return result;
    }
}