using System;
using System.Reflection;
namespace Lab_2_1_2
{
    struct floorCoverings
    {
        public string Type;
        public int Quality;
        public int NoiseSuppression;
        public int Durability;
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            floorCoverings t = new floorCoverings();
            t.Quality = 5;
            t.Type = "Premium";
            t.NoiseSuppression = 32;
            t.Durability = 10;
            PrintStruct(t);
        }
        static void PrintStruct(floorCoverings t)
        {
            Type structType = typeof(floorCoverings);
            FieldInfo[] fields = structType.GetFields();
            foreach (var field in fields)
            {
                Console.WriteLine($"{field.Name} - {field.GetValue(t).ToString()}");
            }
        }
    }
}