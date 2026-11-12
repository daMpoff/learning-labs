using System;

namespace OOP_2_2
{
    internal class Program
    {
        static void Main()
        {
            DoublyLinkedList<int> numbers = new DoublyLinkedList<int>();
            numbers.Add(10);
            numbers.Add(11);
            Node<int> node = new Node<int>(109);
            numbers.Add(node);
            Console.WriteLine(numbers.IndexOfNext(10));//получить индекс следующего элемента искомого поля, чьё информационное поле равно 10
            Console.WriteLine(numbers.IndexOf(12));//получить индекс элемента с информационным полем 12
            Console.WriteLine(numbers.Count);//количество элементов внутри списка
            Console.WriteLine(numbers.Min().Value);//минимальное значение из списка
            Console.WriteLine(numbers.Max().Value);//максимальное значение из списка
            Console.WriteLine(numbers.Edit(10, 200));
            foreach (var s in numbers)//перебор элементов внутри списка
            {
                Console.WriteLine(s);
            }
            foreach (var s in numbers.BackEnumerator())//перебор элементов внутри списка, но начиная с конца
            {
                Console.WriteLine(s);
            }
            numbers.Clear();//очистка списка
        }
    }
}
