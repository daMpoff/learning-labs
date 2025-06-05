namespace ApproximationInterpolation
{
    public class TaylorInterpolator : IInterpolator
    {
        public string Name => $"Тейлор (степень {Degree}, {FunctionName}, x₀={CenterPoint:F2})";
        private int Degree;
        private Func<double, double>[] derivatives;
        private double[] coeffs;
        private string FunctionName;
        private double CenterPoint;

        public TaylorInterpolator(Func<double, double>[] derivatives, string funcName, int degree, double centerPoint = 0.0)
        {
            this.derivatives = derivatives;
            this.FunctionName = funcName;
            this.Degree = degree;
            this.CenterPoint = centerPoint;

            coeffs = new double[Degree + 1];
            for (int k = 0; k <= Degree; k++)
            {
                // Производная функции в точке centerPoint, делим на факториал
                coeffs[k] = derivatives[k](centerPoint) / Factorial(k);
            }
        }

        public void SetPoints(List<(double x, double y)> points)
        {
            // Ряд Тейлора не использует экспериментальные точки, можно оставить пустым.
        }

        public double Interpolate(double x)
        {
            double sum = 0;
            double xMinusX0 = x - CenterPoint;
            for (int k = 0; k <= Degree; k++)
                sum += coeffs[k] * Math.Pow(xMinusX0, k);
            return sum;
        }

        private double Factorial(int n)
        {
            double res = 1;
            for (int i = 2; i <= n; i++)
                res *= i;
            return res;
        }
    }
}