using System;
using System.Collections.Generic;

namespace LAB2_2
{
    struct ParkingRecord
    {
        public DateTime dateStart;
        public DateTime dateEnd;
        public string VehicleNumber;
        public string driverName;
        public string driverLastName;
        public string CarModel;
    }
    internal class Program
    {
        static List<ParkingRecord> ParkingList = new List<ParkingRecord>();
        static void Main()
        {
            Console.WriteLine("Добро пожаловать в программу учёта припаркованных машин!");
            Programma();
        }
        static void Programma()
        {
            Console.WriteLine(
                "Выберите действие, нажав на клавиатуре соответсвующую клавишу:\n" +
                "1 - Добавление записи в программу учёта\n" +
                "2 - Редактирование записи в программе учёта\n" +
                "3 - Удаление записи в программе учёта\n" +
                "4 - Поиск записи по индексу\n" +
                "5 - Поиск записей по параметрам\n" +
                "6 - Показ всех записей\n" +
                "Esc - Выход из программы\n");
            ConsoleKey a = Console.ReadKey().Key;
            if (a == ConsoleKey.D1)
            {
                Console.Clear();
                AddRecord();
            }
            else if (a == ConsoleKey.D2)
            {
                Console.Clear();
                EditRecord();
            }
            else if (a == ConsoleKey.D3)
            {
                Console.Clear();
                DeleteRecord();
            }
            else if (a == ConsoleKey.D4)
            {
                Console.Clear();
                IndexSearch();
            }
            else if (a == ConsoleKey.D5)
            {
                Console.Clear();
                ParametricSearch();
            }
            else if (a == ConsoleKey.D6)
            {
                Console.Clear();
                PrintAll();
            }
            else if (a == ConsoleKey.Escape)
            {
                Console.WriteLine("\nСпасибо за использование программы! До новых встреч!");
                Environment.Exit(0);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Была нажата неверная клавиша!");
                Programma();
            }
        }
        static void ParametricSearch()
        {
            Console.WriteLine(
                "Введите необходимые параметры для поиска\n" +
                "1 - Поиск по имени владельца машины\n" +
                "2 - Поиск по фамилии владельца машины\n" +
                "3 - Поиск по модели машины\n" +
                "4 - Поиск по номеру машины\n" +
                "5 - Поиск по времени въезда\n" +
                "6 - Поиск по времени выезда с парковки\n" +
                "ESC - Возравт в главное меню\n");
            ConsoleKey a = Console.ReadKey().Key;
            string str;

            if (a == ConsoleKey.D1)
            {
                Console.Clear();
                Console.WriteLine("Введите интересующее вас имя владельца машины");
                str = Console.ReadLine();
                Console.Clear();
                if (str == null)
                {
                    Console.WriteLine("Вы должны ввести параметры!");
                    ParametricSearch();
                }
                else
                {
                    List<ParkingRecord> SearchParkingList = new List<ParkingRecord>();
                    SearchParkingList.AddRange(ParkingList.FindAll(tmp => tmp.driverName == str));
                    if (SearchParkingList.Count > 0)
                    {
                        for (int i = 0; i < SearchParkingList.Count; ++i)
                        {
                            Console.WriteLine(
                            $"Имя водителя - {SearchParkingList[i].driverName}\n" +
                            $"Фамилия водителя - {SearchParkingList[i].driverLastName}\n" +
                            $"Модель машины - {SearchParkingList[i].CarModel}\n" +
                            $"Номер машины - {SearchParkingList[i].VehicleNumber}\n" +
                            $"Дата приезда - {SearchParkingList[i].dateStart}\n" +
                            $"Дата отъезда - {SearchParkingList[i].dateEnd}\n");
                        }
                        Programma();
                    }
                    else
                    {
                        Console.WriteLine("По заданному параметру не найдено совпадений");
                        ParametricSearch();
                    }
                }
            }
            else if (a == ConsoleKey.D2)
            {
                Console.Clear();
                Console.WriteLine("Введите интересующую вас фамилию владельца машины");
                str = Console.ReadLine();
                Console.Clear();
                if (str == null)
                {
                    Console.WriteLine("Вы должны ввести параметры!");
                    ParametricSearch();
                }
                else
                {
                    List<ParkingRecord> SearchParkingList = new List<ParkingRecord>();
                    SearchParkingList.AddRange(ParkingList.FindAll(tmp => tmp.driverLastName == str));
                    if (SearchParkingList.Count > 0)
                    {
                        for (int i = 0; i < SearchParkingList.Count; ++i)
                        {
                            Console.WriteLine(
                            $"Имя водителя - {SearchParkingList[i].driverName}\n" +
                            $"Фамилия водителя - {SearchParkingList[i].driverLastName}\n" +
                            $"Модель машины -{SearchParkingList[i].CarModel}\n" +
                            $"Номер машины - {SearchParkingList[i].VehicleNumber}\n" +
                            $"Дата приезда - {SearchParkingList[i].dateStart}\n" +
                            $"Дата отъезда - {SearchParkingList[i].dateEnd}\n");
                        }
                        Programma();
                    }
                    else
                    {
                        Console.WriteLine("По заданному параметру не найдено совпадений");
                        ParametricSearch();
                    }
                }
            }
            else if (a == ConsoleKey.D3)
            {
                Console.Clear();
                Console.WriteLine("Введите интересующую вас модель машины");
                str = Console.ReadLine();
                Console.Clear();
                if (str == null)
                {
                    Console.WriteLine("Вы должны ввести параметры!");
                    ParametricSearch();
                }
                else
                {
                    List<ParkingRecord> SearchParkingList = new List<ParkingRecord>();
                    SearchParkingList.AddRange(ParkingList.FindAll(tmp => tmp.CarModel == str));
                    if (SearchParkingList.Count > 0)
                    {
                        for (int i = 0; i < SearchParkingList.Count; ++i)
                        {
                            Console.WriteLine(
                            $"Имя водителя - {SearchParkingList[i].driverName}\n" +
                            $"Фамилия водителя - {SearchParkingList[i].driverLastName}\n" +
                            $"Модель машины - {SearchParkingList[i].CarModel}\n" +
                            $"Номер машины - {SearchParkingList[i].VehicleNumber}\n" +
                            $"Дата приезда - {SearchParkingList[i].dateStart}\n" +
                            $"Дата отъезда - {SearchParkingList[i].dateEnd}\n");
                        }
                        Programma();
                    }
                    else
                    {
                        Console.WriteLine("По заданному параметру не найдено совпадений");
                        ParametricSearch();
                    }
                }
            }
            else if (a == ConsoleKey.D4)
            {
                Console.Clear();
                Console.WriteLine("Введите интересующий вас номер машины");
                str = Console.ReadLine();
                Console.Clear();
                if (str == null)
                {
                    Console.WriteLine("Вы должны ввести параметры!");
                    ParametricSearch();
                }
                else
                {
                    List<ParkingRecord> SearchParkingList = new List<ParkingRecord>();
                    SearchParkingList.AddRange(ParkingList.FindAll(tmp => tmp.VehicleNumber == str));
                    if (SearchParkingList.Count > 0)
                    {
                        for (int i = 0; i < SearchParkingList.Count; ++i)
                        {
                            Console.WriteLine(
                            $"Имя водителя - {SearchParkingList[i].driverName}\n" +
                            $"Фамилия водителя - {SearchParkingList[i].driverLastName}\n" +
                            $"Модель машины - {SearchParkingList[i].CarModel}\n" +
                            $"Номер машины - {SearchParkingList[i].VehicleNumber}\n" +
                            $"Дата приезда - {SearchParkingList[i].dateStart}\n" +
                            $"Дата отъезда - {SearchParkingList[i].dateEnd}\n");
                        }
                        Programma();
                    }
                    else
                    {
                        Console.WriteLine("По заданному параметру не найдено совпадений");
                        ParametricSearch();
                    }
                }
            }
            else if (a == ConsoleKey.D5)
            {
                Console.Clear();
                Console.WriteLine("Введите интересующую вас дату въезда на парковку");
                str = Console.ReadLine();
                DateTime datastart;
                Console.Clear();
                if (str == null)
                {
                    Console.WriteLine("Вы должны ввести параметры!");
                    ParametricSearch();
                }
                if (DateTime.TryParse(str, out datastart))
                {
                    List<ParkingRecord> SearchParkingList = new List<ParkingRecord>();
                    SearchParkingList.AddRange(ParkingList.FindAll(tmp => tmp.dateStart == datastart));
                    if (SearchParkingList.Count > 0)
                    {
                        for (int i = 0; i < SearchParkingList.Count; ++i)
                        {
                            Console.WriteLine(
                            $"Имя водителя - {SearchParkingList[i].driverName}\n" +
                            $"Фамилия водителя - {SearchParkingList[i].driverLastName}\n" +
                            $"Модель машины - {SearchParkingList[i].CarModel}\n" +
                            $"Номер машины - {SearchParkingList[i].VehicleNumber}\n" +
                            $"Дата приезда - {SearchParkingList[i].dateStart}\n" +
                            $"Дата отъезда - {SearchParkingList[i].dateEnd}\n");

                        }
                        Programma();
                    }
                    else
                    {
                        Console.WriteLine("По заданному параметру не найдено совпадений");
                        ParametricSearch();
                    }
                }
            }
            else if (a == ConsoleKey.D6)
            {
                Console.Clear();
                Console.WriteLine("Введите интересующую вас дату выезда с парковки");
                str = Console.ReadLine();
                DateTime endstart;
                Console.Clear();
                if (str == null)
                {
                    Console.WriteLine("Вы должны ввести параметры!");
                    ParametricSearch();
                }
                if (DateTime.TryParse(str, out endstart))
                {
                    List<ParkingRecord> SearchParkingList = new List<ParkingRecord>();
                    SearchParkingList.AddRange(ParkingList.FindAll(tmp => tmp.dateStart == endstart));
                    if (SearchParkingList.Count > 0)
                    {
                        for (int i = 0; i < SearchParkingList.Count; ++i)
                        {
                            Console.WriteLine(
                            $"Имя водителя - {SearchParkingList[i].driverName}\n" +
                            $"Фамилия водителя - {SearchParkingList[i].driverLastName}\n" +
                            $"Модель машины - {SearchParkingList[i].CarModel}\n" +
                            $"Номер машины - {SearchParkingList[i].VehicleNumber}\n" +
                            $"Дата приезда - {SearchParkingList[i].dateStart}\n" +
                            $"Дата отъезда - {SearchParkingList[i].dateEnd}\n");
                        }
                        Programma();
                    }
                    else
                    {
                        Console.WriteLine("По заданному параметру не найдено совпадений");
                        ParametricSearch();
                    }
                }
            }
            else if (a == ConsoleKey.Escape)
            {
                Console.Clear();
                Programma();
            }
            else
            {
                Console.WriteLine("Была нажата неверная клавиша!");
                ParametricSearch();
            }
        }
        static void AddRecord()
        {
            DateTime DateStart;
            DateTime DateEnd;
            Console.WriteLine("Введите дату заезда в формате YEAR/MONTH/DAY HOURS:MIN:SEC\n");
            string StartDate = Console.ReadLine();
            if (DateTime.TryParse(StartDate, out DateStart))
            {
                Console.WriteLine("Время приезда успешно записано!\n");
            }
            else
            {
                Console.WriteLine("Время приезда указано неверно!\n");
                AddRecord();
            }
            Console.WriteLine("Введите дату отъезда в формате YEAR/MONTH/DAY HOURS:MIN:SEC\n");
            string EndDate = Console.ReadLine();
            if (DateTime.TryParse(EndDate, out DateEnd))
            {
                Console.WriteLine("Время отъезда успешно записано!\n");
            }
            else
            {
                Console.WriteLine("Время отъезда указано неверно!\n");
                AddRecord();
            }
            if (DateEnd < DateStart)
            {
                Console.WriteLine("Время отъезда не может быть меньше времени заезда!\n");
                AddRecord();
            }
            Console.WriteLine("Введите имя владельца машины:\n");
            string Name = Console.ReadLine();
            Console.WriteLine("Введите фамилию владельца машины:\n");
            string LastName = Console.ReadLine();
            Console.WriteLine("Введите модель машины:");
            string ModelCar = Console.ReadLine();
            Console.WriteLine("Введите номер машины:");
            string VehicleNumber = Console.ReadLine();
            ParkingList.Add(new ParkingRecord()
            {
                dateStart = DateStart,
                dateEnd = DateEnd,
                driverName = Name,
                driverLastName = LastName,
                CarModel = ModelCar,
                VehicleNumber = VehicleNumber
            });
            Console.Clear();
            Console.WriteLine("Запись прошла успешно!");
            Programma();
        }
        static void DeleteRecord()
        {
            Console.Clear();
            Console.WriteLine("Внимание! Вы вошли в режим удаления записей\n");
            Console.WriteLine("Введите индекс записи которую вы хотите удалить:\n");
            int index = Convert.ToInt32(Console.ReadLine());
            try
            {
                ParkingList.RemoveAt(index);
                Console.Clear();
                Console.WriteLine("Удаление записи было успешно произведено!\n");
                Programma();
            }
            catch
            {
                Console.WriteLine("Записи с указанным индексом нет, либо индекс введён неверно\n");
                Programma();
            }
        }
        static void IndexSearch()
        {
            Console.Clear();
            Console.WriteLine("Введите индекс интересующей вас записи:\n");
            int PoiskIndex = Convert.ToInt32(Console.ReadLine());
            Console.Clear();
            try
            {
                Console.WriteLine(
                $"Имя водителя - {ParkingList[PoiskIndex].driverName}\n" +
                $"Фамилия водителя - {ParkingList[PoiskIndex].driverLastName}\n" +
                $"Модель машины - {ParkingList[PoiskIndex].CarModel}\n" +
                $"Номер машины - {ParkingList[PoiskIndex].VehicleNumber}\n" +
                $"Дата приезда - {ParkingList[PoiskIndex].dateStart}\n" +
                $"Дата отъезда - {ParkingList[PoiskIndex].dateEnd}\n");
                Programma();
            }
            catch
            {
                Console.WriteLine("Запись по данному индексу не существует, либо индекс введён неправильно:\n");
                Programma();
            }
        }
        static void PrintAll()
        {
            if (ParkingList.Count > 0)
            {
                Console.Clear();
                for (int i = 0; i < ParkingList.Count; ++i)
                {
                    Console.WriteLine(
                    $"Имя водителя - {ParkingList[i].driverName}\n" +
                    $"Фамилия водителя - {ParkingList[i].driverLastName}\n" +
                    $"Модель машины - {ParkingList[i].CarModel}\n" +
                    $"Номер машины - {ParkingList[i].VehicleNumber}\n" +
                    $"Дата приезда - {ParkingList[i].dateStart}\n" +
                    $"Дата отъезда - {ParkingList[i].dateEnd}\n");
                }
                Programma();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("В списке нет записей!");
                Programma();
            }
        }
        static void EditRecord()
        {
            Console.Clear();
            Console.WriteLine("Введите индекс записи, которую необходимо отредактировать:");
            int EditIndex = Convert.ToInt32(Console.ReadLine());
            ParkingRecord tmp = ParkingList[EditIndex];
            Console.Clear();
            Console.WriteLine("Какой параметр вы хотите отредактировать?");
            Console.WriteLine("1 - Имя водителя");
            Console.WriteLine("2 - Фамилия водителя");
            Console.WriteLine("3 - Модель машины");
            Console.WriteLine("4 - Номер машины");
            Console.WriteLine("5 - Время заезда");
            Console.WriteLine("6 - Время отъезда");
            Console.WriteLine("ESC - Вернуться в главное меню\n");
            ConsoleKey a = Console.ReadKey().Key;
            if (a == ConsoleKey.D1)
            {
                Console.Clear();
                Console.WriteLine("Введите новое имя владельца машины");
                tmp.driverName = Console.ReadLine();
                ParkingList[EditIndex] = tmp;
                Console.Clear();
                Console.WriteLine("Изменения успешно сохранены!");
                Programma();
            }
            else if (a == ConsoleKey.D2)
            {
                Console.Clear();
                Console.WriteLine("Введите новую фамилию владельца машины");
                tmp.driverLastName = Console.ReadLine();
                ParkingList[EditIndex] = tmp;
                Console.Clear();
                Console.WriteLine("Изменения успешно сохранены!");
                Programma();
            }
            else if (a == ConsoleKey.D3)
            {
                Console.Clear();
                Console.WriteLine("Введите новую модель машины");
                tmp.CarModel = Console.ReadLine();
                ParkingList[EditIndex] = tmp;
                Console.Clear();
                Console.WriteLine("Изменения успешно сохранены!");
                Programma();
            }
            else if (a == ConsoleKey.D4)
            {
                Console.Clear();
                Console.WriteLine("Введите новый номер машины");
                tmp.VehicleNumber = Console.ReadLine();
                ParkingList[EditIndex] = tmp;
                Console.Clear();
                Console.WriteLine("Изменения успешно сохранены!");
                Programma();
            }
            else if (a == ConsoleKey.D5)
            {
                Console.Clear();
                Console.WriteLine("Введите новое время заезда владельца машины");
                DateTime StartTime;
                if (DateTime.TryParse(Console.ReadLine(), out StartTime))
                {
                    tmp.dateStart = StartTime;
                    ParkingList[EditIndex] = tmp;
                    Console.Clear();
                    Console.WriteLine("Изменения успешно сохранены!");
                    Programma();
                }
                else
                {
                    Console.WriteLine("Введенная дата не соответсвует требованиям!");
                    Programma();
                }
            }
            else if (a == ConsoleKey.D6)
            {
                Console.Clear();
                Console.WriteLine("Введите новое время отъезда владельца машины");
                DateTime EndTime;
                if (DateTime.TryParse(Console.ReadLine(), out EndTime))
                {
                    tmp.dateStart = EndTime;
                    ParkingList[EditIndex] = tmp;
                    Programma();
                }
                else
                {
                    Console.WriteLine("Введённое время не соответсвует требованиям");
                    Programma();
                }
            }
            else if (a == ConsoleKey.Escape)
            {
                Console.Clear();
                Programma();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Была нажата неверная клавиша!");
                EditRecord();
            }
        }
    }
}