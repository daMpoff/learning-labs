using MathNet.Symbolics;

namespace SNE.Utils;

public static class Checker
{
    public static bool CheckNormMatrix(IEnumerable<SymbolicExpression> equations,
        Dictionary<string, FloatingPoint> initialValues)
    {
        // Найдём норму матрицы
        double norm = Math.Abs(SquareMatrix.CreateJacobiMatrix(equations, initialValues).GetNorm());
        return norm >= 1;
    }
}