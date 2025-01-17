namespace MaximumFlow;

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
        Vertices[from].AddEdge(to, weight);
        Vertices[to].AddEdge(from, 0);// Добавляем возвратное ребро с 0 потоком
    }
}