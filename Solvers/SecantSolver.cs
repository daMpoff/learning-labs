using System.Windows;
using MathNet.Symbolics;
using SNE.Interfaces;
using SNE.Utils;

namespace SNE.Solvers;

public class SecantSolver : IIterativeMethod
{
    public string Name { get; } = "Метод секущих";
    public bool IsTwoFormNeed { get; set; } = false;

    public (Dictionary<string, FloatingPoint>, int) Solve(IEnumerable<SymbolicExpression> equations, Dictionary<string, FloatingPoint> initialValues, double tolerance, int maxIterations)
    {
        var variables = equations.First().CollectVariables();
        var currentValues = new Dictionary<string, FloatingPoint>(initialValues);

        // Инициализация матрицы Якоби
        SquareMatrix jacobiMatrix = SquareMatrix.CreateJacobiMatrix(equations, currentValues);

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Создаем копию текущих значений для проверки сходимости
            var previousValues = new Dictionary<string, FloatingPoint>(currentValues);

            // Вычисляем вектор функций
            double[] functionValues = new double[variables.Count()];
            for (int i = 0; i < variables.Count(); i++)
            {
                functionValues[i] = -equations.ElementAt(i).Evaluate(currentValues).RealValue;
            }

            // Решаем систему линейных уравнений J * delta = -F
            double[] delta = LinearSystemSolver.SolveLinearSystem(jacobiMatrix, functionValues);

            // Обновляем значения переменных
            for (int i = 0; i < variables.Count(); i++)
            {
                string variableName = variables.ElementAt(i).ToString();
                currentValues[variableName] = FloatingPoint.NewReal(currentValues[variableName].RealValue + delta[i]);
            }

            // Проверка сходимости
            bool converged = true;
            foreach (var variable in variables)
            {
                string variableName = variable.ToString();
                double diff = Math.Abs(currentValues[variableName].RealValue - previousValues[variableName].RealValue);
                if (diff > tolerance)
                {
                    converged = false;
                    break;
                }
            }
            if (converged)
            {
                return (currentValues, iter + 1);
            }
            // Обновляем матрицу секущих
            UpdateSecantMatrix(jacobiMatrix, equations, currentValues, previousValues, delta);
        }
        throw new Exception("Метод не сошелся за указанное количество итераций.");
    }

    private static void UpdateSecantMatrix(SquareMatrix matrix, IEnumerable<SymbolicExpression> equations, Dictionary<string, FloatingPoint> currentValues, Dictionary<string, FloatingPoint> previousValues, double[] delta)
    {
        var variables = equations.First().CollectVariables();
        int n = variables.Count();
        double[,] y = new double[n, 1];
        double[,] s = new double[n, 1];
 
        for (int i = 0; i < n; i++)
        {
            string variableName = variables.ElementAt(i).ToString();
            s[i, 0] = currentValues[variableName].RealValue - previousValues[variableName].RealValue;
            y[i, 0] = equations.ElementAt(i).Evaluate(currentValues).RealValue - equations.ElementAt(i).Evaluate(previousValues).RealValue;
        }
 
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] += (y[i, 0] - SquareMatrix.MatrixVectorMultiply(matrix, s, i)) * delta[j] / SquareMatrix.VectorDotProduct(s, s);
            }
        }
    }
}