using SLAE.Interfaces;
using SLAE.Models;

namespace SLAE.Solvers
{
    public class GaussianSolver : ISLAESolver
    {
        public double[] Solve(SLAEModel slae)
        {
            // Реализация метода Гаусса для решения СЛАУ, тестовая
            return new double[slae.VariablesCount];
        }
    }
}
