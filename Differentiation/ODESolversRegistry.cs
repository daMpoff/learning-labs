namespace Differentiation;

public static class ODESolversRegistry
{
    public static List<IODESolver> AllSolvers = new()
    {
        new EulerSolver(),
        new EulerCauchySolver(),
        new IterativeEulerSolver(),
        new RungeKuttaSolver(),
        new RungeKutta3Solver(),
        new AdamsSolver(),
        new AdamsBashforthSolver(),
        new AdamsMoultonSolver()
    };
}
