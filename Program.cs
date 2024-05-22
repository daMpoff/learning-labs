using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace OOP_2_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            int rndKey = random.Next(1, 10000);//создали число-ключ для названия файла с исключением;

            FileStream fs; // Открыли поток для записи и чтения файла
            BinaryFormatter bf = new BinaryFormatter(); // объект для сериализации исключения

            Van van = new Van("Mercedes-Acros", 10, 10, 10, 1);
            Cargo cargo = new Cargo("Мотор", 10, 10, 10, 10);
            Cargo cargo1 = new Cargo("Микросхема", 1, 5, 1, 1);

            van.PrintCargo();//Вывод грузов внутри
            try
            {
                van.LoadCargo(cargo);//Загружаем первый груз, всё хорошо
                van.LoadCargo(cargo1);//Возникнет исключение, потому что груз не поместится, и дальше мы обработаем его в блоке catch
                van.PrintCargo();
            }
            catch (WeightException wex)
            {
                Console.WriteLine(wex.Message + wex.Cargo.Weight);
            }
            catch (DimensionException dex)
            {
                fs = new FileStream($"{rndKey}.dex", FileMode.Create);//создали файл
                bf.Serialize(fs, dex);//сериализовали в него исключение, с помощью сериализатора
                fs.Close();//закрыли файл
                van.PrintCargo();//опять вывели все грузы на экран
                fs = new FileStream($"{rndKey}.dex", FileMode.Open);//открыли файл для дальнейшего использования
                DimensionException dimensionException = (DimensionException)bf.Deserialize(fs);//десериализовали исключение и создали на его основе экземпляр исключения
                fs.Close();//закрыли файл
            }
        }
    }
}