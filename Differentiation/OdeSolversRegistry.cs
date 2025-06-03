namespace Differentiation;

public static class OdeSolversRegistry
{
    public static List<IODESolver> AllSolvers = new()
    {
        new EulerSolver(),
        new EulerCauchySolver()
    };
}
