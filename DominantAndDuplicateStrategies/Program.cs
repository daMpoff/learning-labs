using System;

namespace DominantAndDuplicateStrategies
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите количество строк матрицы:");
            int rows = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите количество столбцов матрицы:");
            int columns = int.Parse(Console.ReadLine());

            PaymentMatrix paymentMatrix = new PaymentMatrix(rows, columns);

            Console.WriteLine("Введите элементы матрицы:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write($"Элемент [{i},{j}]: ");
                    int value = int.Parse(Console.ReadLine());
                    paymentMatrix.ModifyElement(i, j, value);
                }
            }

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Отобразить матрицу");
                Console.WriteLine("2. Удалить доминирующие стратегии");
                Console.WriteLine("3. Удалить дублирующиеся стратегии");
                Console.WriteLine("4. Найти седловую точку");
                Console.WriteLine("5. Добавить строку");
                Console.WriteLine("6. Добавить столбец");
                Console.WriteLine("7. Выйти");
                Console.Write("Выберите действие: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        paymentMatrix.Display();
                        break;
                    case 2:
                        paymentMatrix.EliminateDominantStrategies();
                        break;
                    case 3:
                        paymentMatrix.PurgeDuplicateStrategies();
                        break;
                    case 4:
                        paymentMatrix.LocateSaddlePoint();
                        break;
                    case 5:
                        AddRow(paymentMatrix);
                        break;
                    case 6:
                        AddColumn(paymentMatrix);
                        break;
                    case 7:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void AddRow(PaymentMatrix paymentMatrix)
        {
            int columns = paymentMatrix.GetColumnCount();
            int[] newRow = new int[columns];

            Console.WriteLine("Введите элементы новой строки:");
            for (int j = 0; j < columns; j++)
            {
                Console.Write($"Элемент [{j}]: ");
                newRow[j] = int.Parse(Console.ReadLine());
            }

            paymentMatrix.AppendRow(newRow);
        }

        static void AddColumn(PaymentMatrix paymentMatrix)
        {
            int rows = paymentMatrix.GetRowCount();
            int[] newColumn = new int[rows];

            Console.WriteLine("Введите элементы нового столбца:");
            for (int i = 0; i < rows; i++)
            {
                Console.Write($"Элемент [{i}]: ");
                newColumn[i] = int.Parse(Console.ReadLine());
            }

            paymentMatrix.AppendColumn(newColumn);
        }
    }
}