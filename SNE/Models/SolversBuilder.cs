using SNE.Interfaces;
using SNE.Solvers;

namespace SNE.Models;

public static class SolversBuilder
{
    public static List<IIterativeMethod> BuildSolvers()
    {
        var solvers = new List<IIterativeMethod> {new SimpleIterationSolver(), new SeidelSolver(), new NewtonSolver(), new ModifiedNewtonSolver(), new SecantSolver()};
        return solvers;
    }
}