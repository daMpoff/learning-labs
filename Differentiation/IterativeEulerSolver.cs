namespace Differentiation;

public class IterativeEulerSolver : IODESolver
{
    // Имя для ComboBox и отображения
    public string Name => "Эйлер с итерациями";

    // Основной метод решения
    public void Solve(
        int order,
        double start,
        double end,
        double step,
        List<double> init,
        MathNet.Symbolics.SymbolicExpression rhs,
        out List<double> xList,
        out List<List<double>> yLists
    )
    {
        // Количество шагов сетки
        int N = (int)Math.Ceiling((end - start) / step) + 1;
        xList = new List<double>(N) { start };    // Массив x
        yLists = new List<List<double>>();
        // Создаём списки значений для каждой переменной y0, y1, ...
        for (int i = 0; i < order; i++)
            yLists.Add(new List<double>(N) { init[i] });

        double[] y = new double[order];           // Значения yi на текущем шаге
        init.CopyTo(y);
        double x = start;

        int numIterations = 4; // Обычно делают 3-4 итерации (см. презентацию)

        // Основной цикл по шагам сетки
        for (int k = 1; k < N; k++)
        {
            // ===== 1. Первое приближение: обычный Эйлер (predictor) =====
            double[] yIter = new double[order];
            // Для первых компонент: yi+1^(0) = yi + h*yi+1
            for (int i = 0; i < order - 1; i++)
                yIter[i] = y[i] + step * y[i + 1];

            // Для последней: yi+1^(0) = yi + h*f(xi, yi)
            var vars = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
            for (int i = 0; i < order; i++)
                vars[$"y{i}"] = y[i];
            vars["x"] = x;
            double f = rhs.Evaluate(vars).RealValue;
            yIter[order - 1] = y[order - 1] + step * f;

            // ===== 2. Итерационный процесс уточнения (corrector) =====
            for (int iter = 1; iter < numIterations; iter++)
            {
                double[] yPrev = (double[])yIter.Clone(); // Сохраняем предыдущее приближение

                // Для первых компонент: уточняем как среднее (Cauchy)
                for (int i = 0; i < order - 1; i++)
                    yIter[i] = y[i] + step / 2.0 * (y[i + 1] + yPrev[i + 1]);

                // Для последней: среднее значение правой части f по xi, yi и xi+1, y_{i+1}^{(k-1)}
                var varsBar = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                    varsBar[$"y{i}"] = yPrev[i];
                varsBar["x"] = x + step;
                double fBar = rhs.Evaluate(varsBar).RealValue; // f(x_{i+1}, y_{i+1}^{(k-1)})

                var varsOld = new Dictionary<string, MathNet.Symbolics.FloatingPoint>();
                for (int i = 0; i < order; i++)
                    varsOld[$"y{i}"] = y[i];
                varsOld["x"] = x;
                double fOld = rhs.Evaluate(varsOld).RealValue; // f(x_i, y_i)

                yIter[order - 1] = y[order - 1] + step / 2.0 * (fOld + fBar);
            }

            // ===== 3. Запись результатов и переход к следующему шагу =====
            for (int i = 0; i < order; i++)
                yLists[i].Add(yIter[i]);
            x += step;
            xList.Add(x);
            Array.Copy(yIter, y, order); // Запоминаем текущее решение для следующего шага
        }
    }
}
