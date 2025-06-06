using MathNet.Symbolics;

namespace ApproximationInterpolation;

public class ChebyshevApproximator : IApproximator
{
    public string Name => $"Чебышёв (ст. {Degree})";
    public int Degree { get; }
    public double A { get; }
    public double B { get; }
    public string FunctionString { get; }
    private double[] Coeffs;

    public ChebyshevApproximator(string function, int degree, double a, double b)
    {
        Degree = degree;
        A = a;
        B = b;
        FunctionString = function;
        Coeffs = ComputeCoefficients(function, degree, a, b);
    }

    public void SetPoints(List<(double x, double y)> points)
    {
        // Не используется: функция задаётся явно, аппроксимация по формуле
    }

    private double[] ComputeCoefficients(string func, int n, double a, double b)
    {
        var expr = Infix.ParseOrThrow(func);
        double[] c = new double[n];
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int k = 0; k < n; k++)
            {
                double xk = Math.Cos(Math.PI * (k + 0.5) / n);
                double tk = (b + a) / 2 + (b - a) / 2 * xk;
                var val = Evaluate.Evaluate(new Dictionary<string, FloatingPoint> { ["x"] = tk }, expr).RealValue;
                double Ti_xk = Math.Cos(i * Math.Acos(xk));
                sum += val * Ti_xk;
            }
            c[i] = 2.0 / n * sum;
        }
        return c;
    }

    private double ToChebyshevX(double x)
    {
        return (2 * x - (A + B)) / (B - A);
    }

    public double Approximate(double x)
    {
        double xx = ToChebyshevX(x);
        double sum = 0;
        for (int i = 0; i < Degree; i++)
            sum += Coeffs[i] * Math.Cos(i * Math.Acos(xx));
        sum -= Coeffs[0] / 2.0;
        return sum;
    }
}