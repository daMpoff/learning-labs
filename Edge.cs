namespace MaximumFlow;

public class Edge
{
    public int To { get; set; }
    public int Capacity { get; set; }
    public int Flow { get; set; }

    public Edge(int to, int capacity)
    {
        To = to;
        Capacity = capacity;
        Flow = 0;
    }
}