using MathNet.Symbolics;
using SNE.Interfaces;

namespace SNE.Solvers;

public class SeidelSolver : IIterativeMethod
{
    public string Name => "Метод Зейделя";

    public Dictionary<string, FloatingPoint> Solve(IEnumerable<SymbolicExpression> equations, Dictionary<string, FloatingPoint> initialValues, double tolerance, int maxIterations)
    {
        
        var variables = equations.First().CollectVariables();
        var currentValues = new Dictionary<string, FloatingPoint>(initialValues);

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            // Создаем копию текущих значений для проверки сходимости
            var previousValues = new Dictionary<string, FloatingPoint>(currentValues);

            // Обновляем значения переменных с использованием самых последних доступных значений
            for (int i = 0; i < variables.Count(); i++)
            {
                string variableName = variables.ElementAt(i).ToString();
                currentValues[variableName] = equations.ElementAt(i).Evaluate(currentValues);
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
                //Console.WriteLine($"Сходимость достигнута за {iter + 1} итераций.");
                return currentValues;
            }
        }

        Console.WriteLine("Метод не сошелся за указанное количество итераций.");
        return null; // Возвращаем null, если метод не сошелся
    }
}