using System;
using System.Collections.Generic;

namespace OOP_2_1
{
    internal class Van : Auto
    {
        private bool isTurnOn;
        private bool isMove;
        private int height, width, length, weight, maxWeight;
        private List<Cargo> cargoList;
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value > 0)
                {
                    height = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Аргумент должен быть больше 0!");
                }
            }
        }
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value > 0)
                {
                    width = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Аргумент должен быть больше 0!");
                }
            }
        }
        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value > 0)
                {
                    length = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Аргумент должен быть больше 0!");
                }
            }
        }
        public int Weight
        {
            get
            {
                if (cargoList.Count > 0)
                {
                    int weight = 0;
                    foreach (Cargo cargo in cargoList)
                    {
                        weight += cargo.Weight;
                    }
                    return weight;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int Volume
        {
            get
            {
                if (cargoList.Count > 0)
                {
                    int volume = 0;
                    foreach (Cargo cargo in cargoList)
                    {
                        volume += cargo.Volume;
                    }
                    return volume;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int MaxVolume
        {
            get
            {
                return height * width * length;
            }
        }
        public int MaxWeight
        {
            get
            {
                return maxWeight;
            }
            set
            {
                if (value > 0)
                {
                    maxWeight = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Аргумент должен быть больше 0!");
                }
            }
        }
        public void EngineSwitch()
        {
            if (isTurnOn)
            {
                isTurnOn = false;
            }
            else
            {
                isTurnOn = true;
            }
        }
        public override void Move()
        {
            if (isTurnOn && !isMove)
            {
                Console.WriteLine($"{Name} начал движение!");
                isMove = true;
            }
            else if (isTurnOn && isMove)
            {
                Console.WriteLine($"{Name} закончил движение!");
                isMove = false;
            }
            else
            {
                Console.WriteLine($"У {Name} выключен двигатель, включите его!");
            }
        }
        public void LoadCargo(Cargo cargo)
        {
            if (((Volume + cargo.Volume) <= MaxVolume) && ((Weight + cargo.Weight) <= MaxWeight))
            {
                cargoList.Add(cargo);
            }
            else if ((Volume + cargo.Volume) > MaxVolume)
            {
                throw new DimensionException("Переданный груз не помещается! Превышение объёма!", cargo);
            }
            else if ((Weight + cargo.Weight) > MaxWeight)
            {
                throw new WeightException("Переданный груз не помещается! Превышение объёма!", cargo);
            }
        }
        public void UploadCargo()
        {
            if (cargoList.Count > 0)
            {
                Cargo cargo = cargoList[cargoList.Count - 1];
                cargoList.Remove(cargo);
                Console.WriteLine($"Товар {cargo.Name} был выгружен!");
            }
        }
        public void PrintCargo()
        {
            if (cargoList.Count > 0)
            {
                int i = 1;
                foreach (Cargo cargo in cargoList)
                {
                    Console.WriteLine($"Товар №{i} {cargo.Name} имеет объём {cargo.Volume}");
                    i++;
                }
            }
            else
            {
                Console.WriteLine($"В транспорте {Name} нет груза!");
            }
        }
        public Van(string name, int vanHeight, int vanWidth, int vanLength, int maxWeight)
            : base(name)
        {
            cargoList = new List<Cargo>();
            this.Height = vanHeight;
            this.Width = vanWidth;
            this.Length = vanLength;
            this.MaxWeight = maxWeight;
        }
    }
}