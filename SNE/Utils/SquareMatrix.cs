using MathNet.Symbolics;

namespace SNE.Utils
{
    public class SquareMatrix
    {
        private double[,] data;

        public double[,] Data => data;
        public int Size { get; set; }

        public double this[int row, int col]
        {
            get
            {
                return row >= 0 && row < data.GetLength(0) && col >= 0 && col < data.GetLength(1)
                    ? data[row, col]
                    : throw new ArgumentOutOfRangeException("Колонка либо строка неправильны!");
            }
            set
            {
                data[row, col] = row >= 0 && row < data.GetLength(0) && col >= 0 && col < data.GetLength(1)
                    ? value
                    : throw new ArgumentOutOfRangeException("Колонка либо строка неправильны!");
            }
        }

        public static SquareMatrix CreateJacobiMatrix(IEnumerable<SymbolicExpression> functions,
            Dictionary<string, FloatingPoint> values)
        {
            var variables = functions.First().CollectVariables();
            int countParams = variables.Count();
            SquareMatrix resultMatrix = new SquareMatrix(countParams);
            for (int i = 0; i < resultMatrix.Size; i++)
            {
                for (int j = 0; j < resultMatrix.Size; j++)
                {
                    double value = functions.ElementAt(i)
                        .Differentiate(variables.ElementAt(j))
                        .Evaluate(values).RealValue;
                    resultMatrix[i, j] = value;
                }
            }

            return resultMatrix;
        }

        public SquareMatrix(int size)
        {
            data = new double[size, size];
            Size = size;
        }

        public double GetNorm()
        {
            double max = 0;

            double rowSum = 0;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    rowSum += Math.Abs(this[j, i]);
                }

                if (rowSum > max) max = rowSum;
            }

            return max;
        }

        public static double VectorDotProduct(double[,] a, double[,] b)
        {
            double result = 0;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                result += a[i, 0] * b[i, 0];
            }

            return result;
        }

        public static double MatrixVectorMultiply(SquareMatrix matrix, double[,] vector, int row)
        {
            double result = 0;
            for (int j = 0; j < matrix.Size; j++)
            {
                result += matrix[row, j] * vector[j, 0];
            }

            return result;
        }
    }
}