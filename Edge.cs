namespace ShortestPathInGraph;

public class Edge
{
    public int To { get; set; }
    public int Weight { get; set; }

    public Edge(int to, int weight)
    {
        To = to;
        Weight = weight;
    }
}