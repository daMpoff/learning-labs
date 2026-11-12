namespace MaximumFlow;

public interface IMaximumFlowSolve
{
    double GetMaximumFlow(Graph graph, int source, int sink);
}