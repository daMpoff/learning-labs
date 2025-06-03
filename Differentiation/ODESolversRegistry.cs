namespace Differentiation;

public static class ODESolversRegistry
{
    public static List<IODESolver> AllSolvers = new()
    {
        new EulerSolver(),
        new EulerCauchySolver(),
        new IterativeEulerSolver(),
        new ImprovedEulerSolver(),
        new RungeKuttaSolver(),
        new RungeKutta3Solver(),
        new RungeKutta4Solver(),
        new AdamsSolver(),
        new AdamsBashforthSolver(),
        new AdamsMoultonSolver()
    };
}
