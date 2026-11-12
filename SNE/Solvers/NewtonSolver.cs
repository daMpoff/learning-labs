using MathNet.Symbolics;
using SNE.Interfaces;
using SNE.Utils;

namespace SNE.Solvers;

public class NewtonSolver : IIterativeMethod
{
    public string Name { get; } = "Метод Ньютона";
    public bool IsTwoFormNeed { get; set; } = false;

    public (Dictionary<string, FloatingPoint>, int) Solve(IEnumerable<SymbolicExpression> equations,
        Dictionary<string, FloatingPoint> initialValues, double tolerance, int maxIterations)
    {
        var variables = equations.First().CollectVariables();
        var currentValues = new Dictionary<string, FloatingPoint>(initialValues);
        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Создаем копию текущих значений для проверки сходимости
            var previousValues = new Dictionary<string, FloatingPoint>(currentValues);

            // Вычисляем матрицу Якоби и вектор функций
            SquareMatrix jacobiMatrix = SquareMatrix.CreateJacobiMatrix(equations, currentValues);
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
        }

        throw new Exception("Метод не сошелся за указанное количество итераций.");
    }
}