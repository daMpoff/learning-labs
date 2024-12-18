using MathNet.Symbolics;
using SNE.Interfaces;

namespace SNE.Solvers;

public class SimpleIterationSolver : IIterativeMethod
{
    public string Name { get; } = "Метод простых итераций";
    public bool IsTwoFormNeed { get; set; } = true;
    public (Dictionary<string, FloatingPoint>, int) Solve(IEnumerable<SymbolicExpression> equations, Dictionary<string, FloatingPoint> initialValues, double tolerance, int maxIterations)
    {
        // Получаем переменные из первого уравнения
        var variables = equations.First().CollectVariables();

        // Создаем копию начальных значений для текущих значений переменных
        var currentValues = new Dictionary<string, FloatingPoint>(initialValues);

        // Создаем копию начальных значений для следующих значений переменных
        var nextValues = new Dictionary<string, FloatingPoint>(initialValues);

        // Основной цикл итераций
        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Внутренний цикл для обновления значений переменных
            for (int i = 0; i < variables.Count(); i++)
            {
                // Получаем имя текущей переменной
                string variableName = variables.ElementAt(i).ToString();

                // Вычисляем новое значение переменной с использованием текущих значений
                nextValues[variableName] = equations.ElementAt(i).Evaluate(currentValues);
            }

            // Проверка сходимости
            bool converged = true;

            // Проходим по всем переменным и проверяем разницу между текущими и следующими значениями
            foreach (var variable in variables)
            {
                string variableName = variable.ToString();

                // Вычисляем абсолютную разницу между текущим и следующим значением переменной
                double diff = Math.Abs(nextValues[variableName].RealValue - currentValues[variableName].RealValue);

                // Если разница больше заданной точности, сходимость не достигнута
                if (diff > tolerance)
                {
                    converged = false;
                    break;
                }
            }

            // Если сходимость достигнута, возвращаем результаты и завершаем метод
            if (converged)
            {
                return (nextValues, iter + 1);
            }
            // Обновляем текущие значения переменных для следующей итерации
            currentValues = new Dictionary<string, FloatingPoint>(nextValues);
        }

        // Если метод не сошелся за указанное количество итераций, выводим сообщение
        throw new Exception("Метод не сошелся за указанное количество итераций.");
    }
}