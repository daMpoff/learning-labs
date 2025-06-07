namespace ApproximationInterpolation
{
    using MathNet.Symbolics;
    using Expr = MathNet.Symbolics.SymbolicExpression;

    public class TaylorApproximator : IApproximator
    {
        public string Name => $"Тейлор (f(x)={FunctionString}, ст. {Degree})";
        public string FunctionString { get; private set; }
        public int Degree { get; private set; }
        private double[] coeffs;

        public TaylorApproximator(string functionString, int degree)
        {
            FunctionString = functionString;
            Degree = degree;
            coeffs = GenerateTaylorCoefficients(functionString, degree);
        }

        private double[] GenerateTaylorCoefficients(string functionString, int degree)
        {
            var x = Expr.Variable("x");
            var expr = Expr.Parse(functionString);
            var coeffs = new double[degree + 1];
            var deriv = expr;
            for (int k = 0; k <= degree; k++)
            {
                var val = deriv.Evaluate(new Dictionary<string, FloatingPoint> { { "x", 0.0 } }).RealValue;
                coeffs[k] = val / Factorial(k);
                if (k < degree)
                {
                    deriv = deriv.Differentiate("x").Expand();
                }
            }
            return coeffs;
        }

        public void SetPoints(List<(double x, double y)> points) { /* Не используется */ }

        public double Approximate(double x)
        {
            double sum = 0;
            for (int k = 0; k <= Degree; k++)
            {
                sum += coeffs[k] * Math.Pow(x, k);
            }
            return sum;
        }

        private double Factorial(int n)
        {
            double res = 1;
            for (int i = 2; i <= n; i++) res *= i;
            return res;
        }
    }

}