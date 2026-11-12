namespace ShortestPathInGraph;

public class Graph
{
    public List<Vertex> Vertices { get; set; }

    public Graph(int vertexCount)
    {
        Vertices = new List<Vertex>();
        for (int i = 0; i < vertexCount; i++)
        {
            Vertices.Add(new Vertex(i));
        }
    }

    public void AddEdge(int from, int to, int weight)
    {
        Vertices[from - 1].AddEdge(to - 1, weight);
        Vertices[to - 1].AddEdge(from - 1, weight); // Если граф неориентированный, у нас добавится также обратное направление
    }
}