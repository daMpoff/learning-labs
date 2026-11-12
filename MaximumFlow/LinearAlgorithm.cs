namespace MaximumFlow;

public class LinearAlgorithm : IMaximumFlowSolve
{
    public double GetMaximumFlow(Graph graph, int source, int sink)
    {
        // Создаем целевую функцию и ограничения
        var (objectiveFunction, systemLimits) = CreateObjectiveFunctionAndConstraints(graph, source, sink);

        // Решаем задачу с использованием симплекс-метода
        var simplexMethod = new ArtificialBasisMethod.ArtificialBasisMethod();
        var results = simplexMethod.Calculation(objectiveFunction, systemLimits);
        return results[0]; // Возвращаем значение целевой функции
    }

    private (double[] objectiveFunction, double[,] systemLimits) CreateObjectiveFunctionAndConstraints(Graph graph,
        int source, int sink)
    {
        int edgeCount = 0;
        foreach (var vertex in graph.Vertices)
        {
            edgeCount += vertex.Edges.Count;
        }

        // Целевая функция
        double[] objectiveFunction = new double[edgeCount + 1];
        int objIndex = 0;
        foreach (var vertex in graph.Vertices)
        {
            foreach (var edge in vertex.Edges)
            {
                if (vertex.Id == source)
                {
                    objectiveFunction[objIndex] = 1; // Максимизируем поток из источника
                }
                else
                {
                    objectiveFunction[objIndex] = 0;
                }

                objIndex++;
            }
        }

        objectiveFunction[^1] = 1; // Максимизация функции

        // Ограничения
        int constraintCount = graph.Vertices.Count + edgeCount; // Пропускные способности + сохранение потока
        double[,] systemLimits = new double[constraintCount, edgeCount + 1];
        int constraintIndex = 0;

        // Пропускные способности
        objIndex = 0;
        foreach (var vertex in graph.Vertices)
        {
            foreach (var edge in vertex.Edges)
            {
                systemLimits[constraintIndex, 0] = 1; // <=
                systemLimits[constraintIndex, objIndex + 1] = 1;
                systemLimits[constraintIndex, edgeCount] = edge.Capacity;
                constraintIndex++;
                objIndex++;
            }
        }

        // Сохранение потока
        foreach (var vertex in graph.Vertices)
        {
            if (vertex.Id != source && vertex.Id != sink)
            {
                systemLimits[constraintIndex, 0] = 0; // =
                objIndex = 0; // Сбрасываем индекс для корректного заполнения
                foreach (var edge in vertex.Edges)
                {
                    if (edge.To == vertex.Id)
                    {
                        systemLimits[constraintIndex, objIndex + 1] = -1;
                    }
                    else
                    {
                        systemLimits[constraintIndex, objIndex + 1] = 1;
                    }

                    objIndex++;
                }

                constraintIndex++;
            }
        }

        return (objectiveFunction, systemLimits);
    }
}