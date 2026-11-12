namespace Differentiation;

public interface IODESolver
{
    string Name { get; }
    // Решение задачи Коши для системы ОДУ
    // order - порядок, step - шаг, init - начальные, rhs - правая часть, t0/x0 - старт, tN/xN - конец
    void Solve(
        int order,
        double start,
        double end,
        double step,
        List<double> init,
        MathNet.Symbolics.SymbolicExpression rhs,
        out List<double> tList,
        out List<List<double>> yLists
    );
}
