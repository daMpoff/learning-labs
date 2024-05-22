using System;

namespace OOP_2_1
{
    public class Cargo
    {
        private string name;
        private int height, width, length, weight;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    name = value;
                }
                else
                {
                    throw new ArgumentException($"Название должно быть больше одного символа, и не состоять из пробелов!");
                }
            }
        }

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
        public int Volume
        {
            get
            {
                return height * width * length;
            }
        }
        public int Weight
        {
            get
            {
                return weight;
            }
            set
            {
                if (value > 0)
                {
                    weight = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Аргумент должен быть больше 0!");
                }
            }
        }
        public Cargo(string name, int height, int width, int length, int weight)
        {
            this.Name = name;
            this.Height = height;
            this.Width = width;
            this.Length = length;
            this.Weight = weight;
        }
    }
}