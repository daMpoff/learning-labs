namespace MaximumFlow;

public interface IMaximumFlowSolve
{
    public double GetMaximumFlow(Graph graph, int source, int sink);
}