namespace SLAE.Models
{
    public class SLAEModel
    {
        public double[,] Coefficients { get; set; }
        public double[] Constants { get; set; }
        public int RowsCount { get; set; }
        public int VariablesCount { get; set; }

        public SLAEModel(int rowsCount, int variablesCount)
        {
            RowsCount = rowsCount;
            VariablesCount = variablesCount;
            Coefficients = new double[rowsCount, variablesCount];
            Constants = new double[rowsCount];
        }
    }
}