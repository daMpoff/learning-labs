namespace OOP_2_1
{
    public abstract class Auto
    {
        public string Name { get; set; }
        public abstract void Move();
        public Auto(string name)
        {
            Name = name;
        }
    }
}
