namespace DominantAndDuplicateStrategies
{
    public class PaymentMatrix
    {
        private int[,] matrix;

        public PaymentMatrix(int rows, int columns)
        {
            matrix = new int[rows, columns];
        }

        public PaymentMatrix(int[,] matrix)
        {
            this.matrix = matrix;
        }

        public void EliminateDominantStrategies()
        {
            bool haveChanges;
            do
            {
                haveChanges = false;
                haveChanges |= RemoveDominantRows();
                haveChanges |= RemoveDominantColumns();
            } while (haveChanges);
        }

        private bool RemoveDominantRows()
        {
            int rows = matrix.GetLength(0);
            bool haveChanges = false;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (i != j && IsRowDominant(i, j))
                    {
                        matrix = ExcludeRow(j);
                        haveChanges = true;
                        break;
                    }
                }

                if (haveChanges) break;
            }

            return haveChanges;
        }

        private bool RemoveDominantColumns()
        {
            int columns = matrix.GetLength(1);
            bool haveChanges = false;

            for (int j = 0; j < columns; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    if (i != j && IsColumnDominant(i, j))
                    {
                        matrix = ExcludeColumn(j);
                        haveChanges = true;
                        break;
                    }
                }

                if (haveChanges) break;
            }

            return haveChanges;
        }

        private bool IsColumnDominant(int targetColumnIndex, int stepColumnIndex)
        {
            int rows = matrix.GetLength(0);

            for (int i = 0; i < rows; i++)
            {
                if (matrix[i, targetColumnIndex] > matrix[i, stepColumnIndex])
                {
                    return false;
                }
            }

            return true;
        }

        private int[,] ExcludeColumn(int columnIndex)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1) - 1;
            int[,] reducedMatrix = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                int reducedColumn = 0;
                for (int j = 0; j < columns + 1; j++)
                {
                    if (j != columnIndex)
                    {
                        reducedMatrix[i, reducedColumn] = matrix[i, j];
                        reducedColumn++;
                    }
                }
            }

            return reducedMatrix;
        }

        private bool IsRowDominant(int targetRowIndex, int stepRowIndex)
        {
            int columns = matrix.GetLength(1);

            for (int j = 0; j < columns; j++)
            {
                if (matrix[targetRowIndex, j] < matrix[stepRowIndex, j])
                {
                    return false;
                }
            }

            return true;
        }

        private int[,] ExcludeRow(int rowIndex)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            int[,] reducedMatrix = new int[rows - 1, columns];
            int newRow = 0;

            for (int i = 0; i < rows; i++)
            {
                if (i != rowIndex)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        reducedMatrix[newRow, j] = matrix[i, j];
                    }

                    newRow++;
                }
            }

            return reducedMatrix;
        }

        public bool LocateSaddlePoint()
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            int[,] operationMatrix = new int[rows + 1, columns + 1];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    operationMatrix[i, j] = matrix[i, j];
                }
            }

            int maxRowMinValue = int.MinValue;
            int maxRowMinStrategy = 0;
            for (int i = 0; i < rows; i++)
            {
                int rowMinValue = operationMatrix[i, 0];
                for (int j = 1; j < columns; j++)
                {
                    if (operationMatrix[i, j] < rowMinValue)
                    {
                        rowMinValue = operationMatrix[i, j];
                    }
                }

                operationMatrix[i, columns] = rowMinValue;

                if (rowMinValue > maxRowMinValue)
                {
                    maxRowMinValue = rowMinValue;
                    maxRowMinStrategy = i;
                }
            }

            int minColumnMaxValue = int.MaxValue;
            int minColumnMaxStrategy = 0;
            for (int j = 0; j < columns; j++)
            {
                int columnMaxValue = operationMatrix[0, j];
                for (int i = 1; i < rows; i++)
                {
                    if (operationMatrix[i, j] > columnMaxValue)
                    {
                        columnMaxValue = operationMatrix[i, j];
                    }
                }

                operationMatrix[rows, j] = columnMaxValue;

                if (columnMaxValue < minColumnMaxValue)
                {
                    minColumnMaxValue = columnMaxValue;
                    minColumnMaxStrategy = j;
                }
            }

            if (maxRowMinValue == minColumnMaxValue)
            {
                Console.WriteLine($"Нижняя цена игры: {maxRowMinValue};\nВерхняя цена игры: {minColumnMaxValue};\n" +
                                  $"Седловая точка ({maxRowMinStrategy}, {minColumnMaxStrategy}) указывает решение на пару альтернатив:\n" +
                                  $"(A{maxRowMinStrategy}, B{minColumnMaxStrategy}).\n" +
                                  $"Цена игры равна: {operationMatrix[maxRowMinStrategy, minColumnMaxStrategy]}");
                return true;
            }

            Console.WriteLine(
                $"Нижняя цена игры: {maxRowMinValue};\nВерхняя цена игры: {minColumnMaxValue};\nСедловая точка отсутствует!");
            return false;
        }

        public void PurgeDuplicateStrategies()
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            List<int> uniqueRows = new List<int>();
            List<int> uniqueColumns = new List<int>();

            for (int i = 0; i < rows; i++)
            {
                if (!uniqueRows.Exists(row => IsRowEqual(i, row)))
                {
                    uniqueRows.Add(i);
                }
            }

            for (int j = 0; j < columns; j++)
            {
                if (!uniqueColumns.Exists(column => IsColumnEqual(j, column)))
                {
                    uniqueColumns.Add(j);
                }
            }

            int[,] uniqueMatrix = new int[uniqueRows.Count, uniqueColumns.Count];
            for (int i = 0; i < uniqueRows.Count; i++)
            {
                for (int j = 0; j < uniqueColumns.Count; j++)
                {
                    uniqueMatrix[i, j] = matrix[uniqueRows[i], uniqueColumns[j]];
                }
            }

            matrix = uniqueMatrix;
        }

        private bool IsRowEqual(int row1, int row2)
        {
            int columns = matrix.GetLength(1);
            for (int j = 0; j < columns; j++)
            {
                if (matrix[row1, j] != matrix[row2, j])
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsColumnEqual(int column1, int column2)
        {
            int rows = matrix.GetLength(0);
            for (int i = 0; i < rows; i++)
            {
                if (matrix[i, column1] != matrix[i, column2])
                {
                    return false;
                }
            }

            return true;
        }

        public void AppendColumn(int[] column)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            int[,] newMatrix = new int[rows, columns + 1];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    newMatrix[i, j] = matrix[i, j];
                }

                newMatrix[i, columns] = column[i];
            }

            matrix = newMatrix;
        }

        public void AppendRow(int[] row)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            int[,] newMatrix = new int[rows + 1, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    newMatrix[i, j] = matrix[i, j];
                }
            }

            for (int j = 0; j < columns; j++)
            {
                newMatrix[rows, j] = row[j];
            }

            matrix = newMatrix;
        }

        public void ModifyElement(int i, int j, int value)
        {
            matrix[i, j] = value;
        }

        public void Display()
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            Console.WriteLine($"\nПлатежная матрица:\n" +
                              $"Размер: {rows} x {columns}\n");
            for (int temp = 0; temp < matrix.GetLength(1); temp++)
            {
                Console.Write($" B{temp} ");
            }
            Console.WriteLine();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write($" A{i} ");
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($" {matrix[i, j]}");
                }
                Console.WriteLine();
            }
        }
        public int GetRowCount()
        {
            return matrix.GetLength(0);
        }

        public int GetColumnCount()
        {
            return matrix.GetLength(1);
        }
    }
}