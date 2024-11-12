using SLAE.Models;

namespace SLAE.Interfaces
{
    public interface ISLAESolver
    {
        /// <summary>
        /// Название метода, который используется при решении СЛАУ
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Свойство которое показывает, включён ли метод при решении СЛАУ
        /// </summary>
        bool IsEnable { get; set; }
        /// <summary>
        /// Решает систему линейных алгебраических уравнений (СЛАУ) с использованием реализованного алгоритма.
        /// </summary>
        /// <param name="slae">
        /// Объект SLAEModel, содержащий коэффициенты, константы и другие необходимые данные для решения СЛАУ.
        /// </param>
        /// <returns>
        /// Массив вещественных чисел, представляющий решение СЛАУ.
        /// </returns>
        double[] Solve(SLAEModel slae);
    }
}
