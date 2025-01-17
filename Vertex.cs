namespace MaximumFlow;

public class Vertex
{
    public int Id { get; set; }
    public List<Edge> Edges { get; set; }

    public Vertex(int id)
    {
        Id = id;
        Edges = new List<Edge>();
    }

    public void AddEdge(int to, int weight)
    {
        Edges.Add(new Edge(to, weight));
    }
}