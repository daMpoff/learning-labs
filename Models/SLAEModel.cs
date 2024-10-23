namespace SLAE.Models
{
    public class SLAEModel
    {
        public double[,] Coefficients { get; set; }  // Матрица коэффициентов (rows x columns)
        public double[] Constants { get; set; }      // Вектор свободных членов (rows)
        public int RowsCount { get; set; }           // Количество уравнений (строк)
        public int VariablesCount { get; set; }      // Количество переменных (столбцов)

        public SLAEModel(int rowsCount, int variablesCount)
        {
            RowsCount = rowsCount;
            VariablesCount = variablesCount;
            Coefficients = new double[rowsCount, variablesCount];
            Constants = new double[rowsCount];
        }
    }
}
